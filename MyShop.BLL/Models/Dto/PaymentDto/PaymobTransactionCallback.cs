using System.Text.Json.Serialization;

namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobTransactionCallback
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("obj")]
        public PaymobTransaction Obj { get; set; } = new();
    }
}
