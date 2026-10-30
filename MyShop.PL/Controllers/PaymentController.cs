using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.PaymentDto;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.OrderServices;
using MyShop.BLL.Services.PaymentServices;
using MyShop.DAL.Entities.Enum;
using MyShop.PL.ViewModels;
using Stripe;
using System.Security.Claims;

namespace MyShop.PL.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymobPaymentGateway _paymobPaymentGateway;
        private readonly PaymobSettings _paymobSettings;

        public PaymentController(
            IPaymentService paymentService,
            IOrderService orderService,
            ICartService cartService,
            IOptions<StripeSettings> stripeSettings,
            IOptions<PaymobSettings> paymobSettings,
            ILogger<PaymentController> logger,
            IPaymobPaymentGateway paymobPaymentGateway)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _cartService = cartService;
            _stripeSettings = stripeSettings.Value;
            _paymobSettings = paymobSettings.Value;
            _logger = logger;
            _paymobPaymentGateway = paymobPaymentGateway;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            CreateOrUpdatePaymentIntent(int orderId)
        {
            if (orderId <= 0)
                return NotFound(new { message = "Order not found." });

            var order = await _orderService.GetOrderForCustomerAsync(orderId);

            if (order is null)
                return NotFound(new
                {
                    message = "Order not found."
                });

            if (order.Status == OrderStatus.Paid)
                return Ok(new
                {
                    completed = true,
                    redirectUrl = Url.Action(nameof(Status), "Payment", new { orderId = order.Id })
                });

            if (order.Status != OrderStatus.Pending)
                return Conflict(new { message = "This order is no longer awaiting payment." });

            try
            {
                var result = await _paymentService.CreateOrUpdatePaymentIntentAsync(
                    order.Id,
                    ConvertEgpToUsd(order.TotalPrice),
                    "usd");
                return Ok(result);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe could not create a payment intent for order {OrderId}.", order.Id);
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Stripe could not prepare the payment. Please try again."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Stripe payment settings are invalid for order {OrderId}.", order.Id);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    message = "Stripe is temporarily unavailable. Please try another payment method or try again later."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(
            int orderId,
            string paymentIntentId)
        {
            if (orderId <= 0 ||
                string.IsNullOrWhiteSpace(paymentIntentId))
            {
                return BadRequest(new
                {
                    message = "Invalid payment confirmation request."
                });
            }

            var currentUserId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var order =
                await GetPendingOrderOrCompletedPaymentAsync(
                    orderId,
                    paymentIntentId,
                    currentUserId);

            if (order is null)
            {
                return NotFound(new
                {
                    message = "Order not found."
                });
            }

            if (order.Status == OrderStatus.Paid)
            {
                _cartService.ClearCart();

                TempData["SuccessMessage"] =
                    "Payment successful. Order placed and confirmation email sent.";

                return Ok(new
                {
                    redirectUrl = Url.Action(nameof(Status), "Payment", new { orderId = order.Id })
                });
            }

            bool paymentSucceeded;
            try
            {
                paymentSucceeded = await _paymentService.ConfirmPaymentSucceededAsync(
                    order.Id,
                    paymentIntentId,
                    ConvertEgpToUsd(order.TotalPrice),
                    "usd");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe could not verify payment for order {OrderId}.", order.Id);
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Stripe could not verify the payment yet. Check your order status before trying again."
                });
            }

            if (!paymentSucceeded)
            {
                return BadRequest(new
                {
                    message = "Payment was not confirmed by Stripe."
                });
            }

            await _orderService.MarkAsPaidAsync(order.Id);
            _cartService.ClearCart();

            TempData["SuccessMessage"] =
                "Payment successful. Order placed and confirmation email sent.";

            return Ok(new
            {
                redirectUrl = Url.Action(nameof(Status), "Payment", new { orderId = order.Id })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Status(int orderId)
        {
            if (orderId <= 0)
                return NotFound();

            var order = await _orderService.GetOrderForCustomerAsync(orderId);
            if (order is null)
                return NotFound();

            return View(new PaymentStatusViewModel
            {
                OrderId = order.Id,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                ItemCount = order.OrderItems.Sum(item => item.Quantity),
                Status = order.Status
            });
        }

        [HttpGet]
        public async Task<IActionResult> StatusCheck(int orderId)
        {
            Response.Headers.CacheControl = "no-store, no-cache";
            if (orderId <= 0)
                return NotFound();

            var order = await _orderService.GetOrderForCustomerAsync(orderId);
            if (order is null)
                return NotFound();

            return Json(new
            {
                status = order.Status.ToString().ToLowerInvariant(),
                total = order.TotalPrice,
                itemCount = order.OrderItems.Sum(item => item.Quantity)
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Webhook()
        {
            if (string.IsNullOrWhiteSpace(_stripeSettings.WebhookSecret))
            {
                _logger.LogWarning(
                    "Stripe webhook rejected because Stripe:WebhookSecret is not configured.");

                return BadRequest(new
                {
                    message = "Stripe webhook secret is not configured."
                });
            }

            var json =
                await new StreamReader(
                    HttpContext.Request.Body)
                    .ReadToEndAsync();

            var signature =
                Request.Headers["Stripe-Signature"]
                    .ToString();

            if (string.IsNullOrWhiteSpace(signature))
            {
                _logger.LogWarning(
                    "Stripe webhook rejected because Stripe-Signature header is missing.");

                return BadRequest(new
                {
                    message = "Stripe-Signature header is missing."
                });
            }

            try
            {
                var stripeEvent =
                    EventUtility.ConstructEvent(
                        json,
                        signature,
                        _stripeSettings.WebhookSecret,
                        throwOnApiVersionMismatch: false);

                _logger.LogInformation(
                    "Stripe webhook accepted event {EventId} with type {EventType}.",
                    stripeEvent.Id,
                    stripeEvent.Type);

                if (stripeEvent.Type ==
                    EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent =
                        stripeEvent.Data.Object
                            as PaymentIntent;

                    if (paymentIntent is null)
                        return Ok();

                    _logger.LogInformation(
                        "Stripe webhook received succeeded payment intent {PaymentIntentId}.",
                        paymentIntent.Id);

                    var order =
                        await _orderService
                            .GetOrderByPaymentIntentIdAsync(
                                paymentIntent.Id);

                    if (order is null)
                        return Ok();

                    if (order.Status == OrderStatus.Paid)
                        return Ok();

                    var expectedUsdMinorUnits = (long)Math.Round(
                        ConvertEgpToUsd(order.TotalPrice) * 100m,
                        MidpointRounding.AwayFromZero);

                    if (!string.Equals(paymentIntent.Currency, "usd", StringComparison.OrdinalIgnoreCase)
                        || paymentIntent.AmountReceived != expectedUsdMinorUnits)
                    {
                        _logger.LogWarning(
                            "Stripe webhook payment {PaymentIntentId} did not match expected amount/currency for order {OrderId}.",
                            paymentIntent.Id,
                            order.Id);
                        return Ok();
                    }

                    await _orderService
                        .MarkAsPaidAsync(order.Id);
                }

                else if (stripeEvent.Type ==
                         EventTypes.PaymentIntentPaymentFailed)
                {
                    var paymentIntent =
                        stripeEvent.Data.Object
                            as PaymentIntent;

                    if (paymentIntent is null)
                        return Ok();

                    _logger.LogWarning(
                        "Stripe webhook received failed payment intent {PaymentIntentId}.",
                        paymentIntent.Id);

                    var order =
                        await _orderService
                            .GetOrderByPaymentIntentIdAsync(
                                paymentIntent.Id);

                    if (order is not null)
                    {
                        // A declined attempt does not cancel the order. Keeping it
                        // pending lets the customer correct the card and retry.
                        _logger.LogInformation(
                            "Order {OrderId} remains pending after a failed Stripe payment attempt.",
                            order.Id);
                    }
                }

                return Ok(new
                {
                    received = true
                });
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Stripe webhook signature validation failed.");

                return BadRequest(new
                {
                    message = "Stripe webhook signature validation failed."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Stripe webhook processing failed.");

                return BadRequest(new
                {
                    message = "Stripe webhook processing failed."
                });
            }
        }

        private async Task<MyShop.DAL.Entities.Order?>
            GetPendingOrderOrCompletedPaymentAsync(
                int orderId,
                string paymentIntentId,
                string? currentUserId)
        {
            try
            {
                return await _orderService
                    .GetOrderForPaymentAsync(orderId);
            }
            catch (InvalidOperationException)
            {
                var order =
                    await _orderService
                        .GetOrderByPaymentIntentIdAsync(
                            paymentIntentId);

                if (order is null ||
                    order.Id != orderId ||
                    order.UserId != currentUserId ||
                    order.Status != OrderStatus.Paid)
                    throw;

                return order;
            }
        }

        private decimal ConvertEgpToUsd(decimal amountInEgp)
        {
            if (_stripeSettings.EgpPerUsd <= 0)
                throw new InvalidOperationException(
                    "Stripe: EgpPerUsd must be a positive exchange rate.");

            return Math.Round(
                amountInEgp / _stripeSettings.EgpPerUsd,
                2,
                MidpointRounding.AwayFromZero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
      CreatePaymobPayment(int orderId)
        {
            if (orderId <= 0)
                return NotFound(new { message = "Order not found." });

            var order = await _orderService.GetOrderForCustomerAsync(orderId);

            if (order is null)
            {
                return NotFound(new
                {
                    message = "Order not found."
                });
            }

            if (order.Status == OrderStatus.Paid)
                return Ok(new
                {
                    completed = true,
                    redirectUrl = Url.Action(nameof(Status), "Payment", new { orderId = order.Id })
                });

            if (order.Status != OrderStatus.Pending)
                return Conflict(new { message = "This order is no longer awaiting payment." });

            if (!Uri.TryCreate(
                    _paymobSettings.CallbackBaseUrl,
                    UriKind.Absolute,
                    out var callbackBaseUri)
                || callbackBaseUri.Scheme != Uri.UriSchemeHttps)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    message = "Paymob is not ready: configure Paymob:CallbackBaseUrl with your public HTTPS URL."
                });
            }

            try
            {
                var result =
                    await _paymobPaymentGateway
                        .CreatePaymentIntentionAsync(
                            order.Id,
                            order.TotalPrice,
                            order.CustomerEmail,
                            order.Phone,
                            new Uri(callbackBaseUri, "/Payment/PaymobWebhook").AbsoluteUri,
                            new Uri(callbackBaseUri, $"/Payment/PaymobReturn?orderId={order.Id}").AbsoluteUri);

                return Ok(result);
            }
            catch (Exception ex) when (
                ex is HttpRequestException
                or InvalidOperationException
                or ArgumentException)
            {
                _logger.LogError(ex, "Could not create Paymob intention for order {OrderId}.", order.Id);
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Paymob could not prepare the payment. Check its configuration or try again."
                });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> PaymobWebhook()
        {
            if (string.IsNullOrWhiteSpace(_paymobSettings.HmacSecret))
            {
                _logger.LogError("Paymob webhook rejected because Paymob:HmacSecret is not configured.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            using var bodyReader = new StreamReader(Request.Body);
            var rawBody = await bodyReader.ReadToEndAsync();
            var receivedHmac = Request.Query["hmac"].ToString();

            if (!IsValidPaymobHmac(rawBody, receivedHmac))
            {
                _logger.LogWarning("Paymob webhook rejected because HMAC validation failed.");
                return Unauthorized();
            }

            PaymobTransactionCallback? callback;
            try
            {
                callback = System.Text.Json.JsonSerializer.Deserialize<PaymobTransactionCallback>(
                    rawBody,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogWarning(ex, "Paymob webhook body could not be parsed.");
                return BadRequest();
            }

            if (callback?.Obj is null || callback.Type != "TRANSACTION")
                return BadRequest();

            var transaction = callback.Obj;

            if (!transaction.Success ||
                transaction.Pending ||
                transaction.ErrorOccured ||
                transaction.IsRefunded ||
                transaction.IsVoided)
            {
                return Ok();
            }

            var localOrderId =
                transaction.Order?.MerchantOrderId;

            if (!int.TryParse(localOrderId, out var orderId))
            {
                _logger.LogWarning("Paymob callback transaction {TransactionId} has no valid merchant order reference.", transaction.Id);
                return Ok();
            }

            var order =
                await _orderService
                    .GetOrderForPaymobCallbackAsync(orderId);

            if (order is null)
                return Ok();

            if (order.Status == OrderStatus.Paid)
                return Ok();

            var expectedAmount =
                (long)Math.Round(
                    order.TotalPrice * 100,
                    MidpointRounding.AwayFromZero);

            if (transaction.AmountCents != expectedAmount)
                return BadRequest();

            if (_paymobSettings.IntegrationId > 0
                && transaction.IntegrationId != _paymobSettings.IntegrationId)
            {
                _logger.LogWarning(
                    "Paymob webhook integration {ReceivedIntegrationId} did not match configured integration for order {OrderId}.",
                    transaction.IntegrationId,
                    order.Id);
                return BadRequest();
            }

            if (!string.Equals(
                transaction.Currency,
                "EGP",
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            await _orderService
                .MarkAsPaidAsync(order.Id);

            return Ok(new
            {
                received = true
            });
        }

        [HttpGet]
        public IActionResult PaymobReturn(int? orderId)
        {
            if (!orderId.HasValue || orderId.Value <= 0)
            {
                TempData["Message"] = "Paymob returned you to the store. Check your orders for the latest payment status.";
                return RedirectToAction("MyOrders", "Orders");
            }

            return RedirectToAction(nameof(Status), new { orderId = orderId.Value });
        }

        private bool IsValidPaymobHmac(string rawBody, string receivedHmac)
        {
            if (string.IsNullOrWhiteSpace(receivedHmac))
                return false;

            try
            {
                using var document = System.Text.Json.JsonDocument.Parse(rawBody);
                var transaction = document.RootElement.GetProperty("obj");

                var values = new[]
                {
                    ReadHmacValue(transaction, "amount_cents"),
                    ReadHmacValue(transaction, "created_at"),
                    ReadHmacValue(transaction, "currency"),
                    ReadHmacValue(transaction, "error_occured"),
                    ReadHmacValue(transaction, "has_parent_transaction"),
                    ReadHmacValue(transaction, "id"),
                    ReadHmacValue(transaction, "integration_id"),
                    ReadHmacValue(transaction, "is_3d_secure"),
                    ReadHmacValue(transaction, "is_auth"),
                    ReadHmacValue(transaction, "is_capture"),
                    ReadHmacValue(transaction, "is_refunded"),
                    ReadHmacValue(transaction, "is_standalone_payment"),
                    ReadHmacValue(transaction, "is_voided"),
                    ReadHmacValue(transaction.GetProperty("order"), "id"),
                    ReadHmacValue(transaction, "owner"),
                    ReadHmacValue(transaction, "pending"),
                    ReadHmacValue(transaction.GetProperty("source_data"), "pan"),
                    ReadHmacValue(transaction.GetProperty("source_data"), "sub_type"),
                    ReadHmacValue(transaction.GetProperty("source_data"), "type"),
                    ReadHmacValue(transaction, "success")
                };

                using var hmac = new System.Security.Cryptography.HMACSHA512(
                    System.Text.Encoding.UTF8.GetBytes(_paymobSettings.HmacSecret));
                var expected = hmac.ComputeHash(
                    System.Text.Encoding.UTF8.GetBytes(string.Concat(values)));
                var received = Convert.FromHexString(receivedHmac);

                return received.Length == expected.Length
                    && System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                        received,
                        expected);
            }
            catch (Exception ex) when (
                ex is System.Text.Json.JsonException
                or InvalidOperationException
                or KeyNotFoundException
                or FormatException)
            {
                return false;
            }
        }

        private static string ReadHmacValue(
            System.Text.Json.JsonElement element,
            string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var value)
                || value.ValueKind is System.Text.Json.JsonValueKind.Null
                    or System.Text.Json.JsonValueKind.Undefined)
                return string.Empty;

            return value.ValueKind == System.Text.Json.JsonValueKind.String
                ? value.GetString() ?? string.Empty
                : value.ToString().ToLowerInvariant();
        }
    }
}
