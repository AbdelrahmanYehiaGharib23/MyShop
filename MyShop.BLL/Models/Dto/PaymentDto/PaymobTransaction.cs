namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobTransaction
    {
        public long Id { get; set; }
        public long AmountCents { get; set; }
        public bool Success { get; set; }
        public bool Pending { get; set; }
        public bool IsRefunded { get; set; }
        public bool IsVoided { get; set; }
        public bool ErrorOccured { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int IntegrationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasParentTransaction { get; set; }
        public bool Is3DSecure { get; set; }
        public bool IsAuth { get; set; }
        public bool IsCapture { get; set; }
        public bool IsStandalonePayment { get; set; }
        public long Owner { get; set; }
        public PaymobOrder Order { get; set; } = new();
        public PaymobSourceData SourceData { get; set; } = new();
    }
}
