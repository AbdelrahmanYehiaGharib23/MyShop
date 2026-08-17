using Microsoft.Extensions.Options;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.PaymentDto;
using Stripe;

namespace MyShop.BLL.Services.PaymentServices
{
    public class StripePaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;

        public StripePaymentService(IOptions<StripeSettings> stripeOptions)
        {
            _stripeSettings = stripeOptions.Value;
        }

        public async Task<PaymentIntentResultDto> CreatePaymentIntentAsync(
            decimal amount,
            string currency = "usd")
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            {
                throw new InvalidOperationException("Stripe secret key is not configured.");
            }

            var amountInCents = decimal.ToInt64(decimal.Round(amount * 100, 0, MidpointRounding.AwayFromZero));

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(
                options,
                new RequestOptions
                {
                    ApiKey = _stripeSettings.SecretKey
                });

            return new PaymentIntentResultDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };
        }

        public async Task<bool> IsPaymentIntentSucceededAsync(
            string paymentIntentId,
            decimal expectedAmount,
            string currency = "usd")
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId) || expectedAmount <= 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            {
                throw new InvalidOperationException("Stripe secret key is not configured.");
            }

            var expectedAmountInCents = decimal.ToInt64(decimal.Round(expectedAmount * 100, 0, MidpointRounding.AwayFromZero));
            var service = new PaymentIntentService();

            try
            {
                var paymentIntent = await service.GetAsync(
                    paymentIntentId,
                    requestOptions: new RequestOptions
                    {
                        ApiKey = _stripeSettings.SecretKey
                    });

                return paymentIntent.Status == "succeeded"
                    && paymentIntent.Amount == expectedAmountInCents
                    && string.Equals(paymentIntent.Currency, currency, StringComparison.OrdinalIgnoreCase);
            }
            catch (StripeException)
            {
                return false;
            }
        }
    }
}
