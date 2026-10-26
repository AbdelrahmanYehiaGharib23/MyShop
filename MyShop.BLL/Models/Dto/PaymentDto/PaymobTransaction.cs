using System.Text.Json.Serialization;

namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobTransaction
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("error_occured")]
        public bool ErrorOccured { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("integration_id")]
        public int IntegrationId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonPropertyName("is_3d_secure")]
        public bool Is3DSecure { get; set; }

        [JsonPropertyName("is_auth")]
        public bool IsAuth { get; set; }

        [JsonPropertyName("is_capture")]
        public bool IsCapture { get; set; }

        [JsonPropertyName("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonPropertyName("owner")]
        public long Owner { get; set; }

        [JsonPropertyName("order")]
        public PaymobOrder Order { get; set; } = new();

        [JsonPropertyName("source_data")]
        public PaymobSourceData SourceData { get; set; } = new();
    }
}
