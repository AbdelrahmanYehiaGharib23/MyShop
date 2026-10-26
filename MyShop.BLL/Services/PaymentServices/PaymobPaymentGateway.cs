using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.PaymentDto;

namespace MyShop.BLL.Services.PaymentServices
{
    public class PaymobPaymentGateway : IPaymobPaymentGateway
    {
        private readonly PaymobSettings _settings;
        private readonly HttpClient _httpClient;

        public PaymobPaymentGateway(
            IOptions<PaymobSettings> settings,
            HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
        }

        public async Task<PaymobPaymentResultDto>
            CreatePaymentIntentionAsync(
                int orderId,
                decimal amount,
                string customerEmail,
                string phone,
                string notificationUrl,
                string redirectionUrl)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            if (amount <= 0)
                throw new ArgumentException("Invalid amount.");

            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentException("Customer email is required.");

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Customer phone is required.");

            if (!Uri.TryCreate(notificationUrl, UriKind.Absolute, out var notificationUri)
                || notificationUri.Scheme != Uri.UriSchemeHttps
                || !Uri.TryCreate(redirectionUrl, UriKind.Absolute, out var redirectionUri)
                || redirectionUri.Scheme != Uri.UriSchemeHttps)
            {
                throw new InvalidOperationException(
                    "Paymob requires public HTTPS notification and redirection URLs. Configure Paymob:CallbackBaseUrl with your HTTPS tunnel/domain.");
            }

            var amountInCents =
                ToMinorUnits(amount);

            var payload = new
            {
                amount = amountInCents,

                currency = "EGP",

                payment_methods = new[]
                {
                    _settings.IntegrationId
                },

                items = new[]
                {
                    new
                    {
                        name = $"MyShop Order #{orderId}",
                        amount = amountInCents,
                        description = $"Payment for order #{orderId}",
                        quantity = 1
                    }
                },

                billing_data = new
                {
                    apartment = "NA",
                    first_name = "Customer",
                    last_name = "MyShop",
                    street = "NA",
                    building = "NA",
                    phone_number = NormalizeEgyptianPhone(phone),
                    city = "NA",
                    country = "EGY",
                    state = "NA",
                    email = customerEmail,
                    floor = "NA",
                    postal_code = "NA"
                },

                special_reference = orderId.ToString(),

                expiration = 3600,

                notification_url = notificationUri.AbsoluteUri,
                redirection_url = redirectionUri.AbsoluteUri
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://accept.paymob.com/v1/intention/");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Token",
                    _settings.SecretKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response =
                await _httpClient.SendAsync(request);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Paymob error: {responseContent}");
            }

            var result =
                JsonSerializer.Deserialize<PaymobIntentionResponse>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result is null ||
                string.IsNullOrWhiteSpace(result.Id) ||
                string.IsNullOrWhiteSpace(result.ClientSecret))
            {
                throw new InvalidOperationException(
                    "Invalid Paymob response.");
            }

            var checkoutUrl =
                "https://accept.paymob.com/unifiedcheckout/" +
                $"?publicKey={Uri.EscapeDataString(_settings.PublicKey)}" +
                $"&clientSecret={Uri.EscapeDataString(result.ClientSecret)}";

            return new PaymobPaymentResultDto
            {
                IntentionId = result.Id,
                ClientSecret = result.ClientSecret,
                CheckoutUrl = checkoutUrl
            };
        }

        private static long ToMinorUnits(decimal amount)
        {
            return (long)Math.Round(
                amount * 100,
                MidpointRounding.AwayFromZero);
        }

        private static string NormalizeEgyptianPhone(string phone)
        {
            var normalized = new string(phone
                .Where(char.IsDigit)
                .ToArray());

            if (normalized.StartsWith("00", StringComparison.Ordinal))
                normalized = normalized[2..];

            if (normalized.StartsWith("20", StringComparison.Ordinal))
                return $"+{normalized}";

            return $"+20{normalized.TrimStart('0')}";
        }

        private class PaymobIntentionResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("client_secret")]
            public string ClientSecret { get; set; } =
                string.Empty;
        }
    }
}
