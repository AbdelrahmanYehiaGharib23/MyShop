namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymentIntentResultDto
    {
        public string ClientSecret { get; set; } = string.Empty;

        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
