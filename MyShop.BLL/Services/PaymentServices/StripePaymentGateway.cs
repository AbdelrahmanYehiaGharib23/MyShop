using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.OrderDto;
using Stripe;

namespace MyShop.BLL.Services.PaymentServices
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeSettings _stripeSettings;

        public StripePaymentGateway(
            IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        public async Task<OrderPaymentResultDto> CreatePaymentIntentAsync(
            int orderId,
            decimal amount,
            string currency)
        {
            var stripeClient =
                new StripeClient(_stripeSettings.SecretKey);

            var options = new PaymentIntentCreateOptions
            {
                Amount = ToMinorUnits(amount),
                Currency = currency,

                Metadata = new Dictionary<string, string>
                {
                    ["order_id"] = orderId.ToString()
                }
            };

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = $"order-{orderId}-{currency.ToLowerInvariant()}"
            };

            var paymentIntent =
                await stripeClient.V1.PaymentIntents.CreateAsync(
                    options,
                    requestOptions);

            return new OrderPaymentResultDto
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret!
            };
        }

        public async Task<OrderPaymentResultDto>
            UpdatePaymentIntentAsync(
                string paymentIntentId,
                decimal amount,
                string currency)
        {
            var stripeClient =
                new StripeClient(_stripeSettings.SecretKey);

            var paymentIntents = stripeClient.V1.PaymentIntents;
            var currentIntent =
                await paymentIntents.GetAsync(paymentIntentId);

            if (!string.Equals(
                currentIntent.Currency,
                currency,
                StringComparison.OrdinalIgnoreCase))
            {
                if (!currentIntent.Metadata.TryGetValue(
                    "order_id",
                    out var orderIdValue)
                    || !int.TryParse(orderIdValue, out var orderId))
                {
                    throw new InvalidOperationException(
                        "The existing Stripe payment is not linked to an order and cannot be replaced safely.");
                }

                if (currentIntent.Status is "requires_payment_method"
                    or "requires_confirmation"
                    or "requires_action"
                    or "requires_capture")
                {
                    await paymentIntents.CancelAsync(paymentIntentId);
                }
                else if (currentIntent.Status != "canceled")
                {
                    throw new InvalidOperationException(
                        "The previous payment is already processing. Check the order status before starting another payment.");
                }

                return await CreatePaymentIntentAsync(
                    orderId,
                    amount,
                    currency);
            }

            var options = new PaymentIntentUpdateOptions
            {
                Amount = ToMinorUnits(amount)
            };

            var paymentIntent =
                await paymentIntents.UpdateAsync(
                    paymentIntentId,
                    options);

            return new OrderPaymentResultDto
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret!
            };
        }

        public async Task<bool> IsPaymentIntentSucceededAsync(
            string paymentIntentId,
            int orderId,
            decimal amount,
            string currency)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId) ||
                orderId <= 0 ||
                amount <= 0)
                return false;

            var stripeClient =
                new StripeClient(_stripeSettings.SecretKey);

            var paymentIntent =
                await stripeClient.V1.PaymentIntents.GetAsync(
                    paymentIntentId);

            var expectedAmount =
                ToMinorUnits(amount);

            var hasOrderMetadata =
                paymentIntent.Metadata.TryGetValue(
                    "order_id",
                    out var metadataOrderId)
                && metadataOrderId == orderId.ToString();

            return paymentIntent.Status == "succeeded"
                && paymentIntent.Amount == expectedAmount
                && string.Equals(
                    paymentIntent.Currency,
                    currency,
                    StringComparison.OrdinalIgnoreCase)
                && hasOrderMetadata;
        }

        private static long ToMinorUnits(decimal amount)
        {
            return (long)Math.Round(
                amount * 100,
                MidpointRounding.AwayFromZero);
        }
    }
}
