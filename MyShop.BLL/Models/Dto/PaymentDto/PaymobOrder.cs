using System.Text.Json.Serialization;

namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobOrder
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("merchant_order_id")]
        public string? MerchantOrderId { get; set; }
    }
}
