namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobPaymentResultDto
    {
        public string IntentionId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
