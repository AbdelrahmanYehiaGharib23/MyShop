using System.Text.Json.Serialization;

namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobSourceData
    {
        [JsonPropertyName("pan")]
        public string Pan { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("sub_type")]
        public string SubType { get; set; } = string.Empty;
    }
}
