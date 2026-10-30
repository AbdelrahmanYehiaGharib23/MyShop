using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.OrderServices;
using MyShop.BLL.Services.PaymentServices;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly StripeSettings _stripeSettings;

        public CartController(
            ICartService cartService,
            IOrderService orderService,
            IOptions<StripeSettings> stripeOptions)
        {
            _cartService = cartService;
            _orderService = orderService;
            _stripeSettings = stripeOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            if (await _cartService.RefreshCartAsync())
                TempData["Message"] = "Prices changed or a product was removed. Review the updated cart before checkout.";

            var model = new CartViewModel
            {
                Items = _cartService.GetCart(),
                TotalItems = _cartService.GetTotalItems(),
                TotalPrice = _cartService.GetTotalPrice()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(
            int productId,
            int quantity = 1)
        {
            try
            {
                await _cartService.AddToCart(productId, quantity);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { message = "Choose a valid product quantity." });

                TempData["Message"] = "Choose a valid product quantity.";
                return RedirectToAction("Index", "Store");
            }
            catch (KeyNotFoundException)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return NotFound(new { message = "This product is no longer available." });

                TempData["Message"] = "This product is no longer available.";
                return RedirectToAction("Index", "Store");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Ok(new { productId, quantity });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Increase(int productId)
        {
            _cartService.IncreaseQuantity(productId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrease(int productId)
        {
            _cartService.DecreaseQuantity(productId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cartService.ClearCart();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout(
            int? orderId = null)
        {
            if (!orderId.HasValue && await _cartService.RefreshCartAsync())
                TempData["Message"] = "Prices changed or a product was removed. Review the updated order total.";

            var cartItems = _cartService.GetCart();

            var model = new CheckoutViewModel
            {
                OrderId = orderId,
                CheckoutRequestId = Guid.NewGuid().ToString("N")
            };

            ViewBag.StripePublishableKey =
                _stripeSettings.PublishableKey;

            if (orderId.HasValue)
            {
                MyShop.DAL.Entities.Order? order;
                try
                {
                    order = await _orderService.GetOrderForPaymentAsync(orderId.Value);
                }
                catch (UnauthorizedAccessException)
                {
                    return NotFound();
                }
                catch (InvalidOperationException)
                {
                    TempData["Message"] = "This order is no longer awaiting payment.";
                    return RedirectToAction("MyOrders", "Orders");
                }

                if (order is null)
                    return NotFound();

                model.Items = order.OrderItems.Select(item => new MyShop.DAL.Entities.CartItem
                {
                    ProductId = item.ProductId ?? 0,
                    ProductName = item.ProductName,
                    ImageUrl = item.ProductImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList();
                model.TotalPrice = order.TotalPrice;
                model.Address = order.Address;
                model.Phone = order.Phone;
            }
            else
            {
                if (cartItems is null || !cartItems.Any())
                    return RedirectToAction(nameof(Index));

                model.Items = cartItems;
                model.TotalPrice = _cartService.GetTotalPrice();
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            CheckoutViewModel model)
        {
            var cartChanged = await _cartService.RefreshCartAsync();
            var cartItems = _cartService.GetCart();

            if (model.OrderId.HasValue)
            {
                return RedirectToAction(
                    nameof(Checkout),
                    new { orderId = model.OrderId.Value });
            }

            if (cartItems is null || !cartItems.Any())
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
            {
                model.Items = cartItems;
                model.TotalPrice =
                    _cartService.GetTotalPrice();

                ViewBag.StripePublishableKey =
                    _stripeSettings.PublishableKey;

                return View(model);
            }

            if (cartChanged)
            {
                model.Items = cartItems;
                model.TotalPrice = _cartService.GetTotalPrice();
                ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
                TempData["Message"] = "Prices changed or a product was removed. Review the updated order total and continue again.";
                return View(model);
            }

            var dto = new CreateOrderDto
            {
                Address = model.Address,
                Phone = model.Phone,
                CheckoutRequestId = model.CheckoutRequestId
            };

            int orderId;
            try
            {
                orderId = await _orderService.CreatePendingOrderAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }

            _cartService.ClearCart();

            return RedirectToAction(
                nameof(Checkout),
                new { orderId });
        }
    }
}
