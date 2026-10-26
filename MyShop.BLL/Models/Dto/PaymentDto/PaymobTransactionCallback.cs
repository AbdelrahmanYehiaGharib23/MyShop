namespace MyShop.BLL.Models.Dto.PaymentDto
{
    public class PaymobTransactionCallback
    {
        public string Type { get; set; } = string.Empty;
        public PaymobTransaction Obj { get; set; } = new();
    }
}
