using MyShop.BLL.Models.Dto.PaymentDto;

namespace MyShop.BLL.Services.PaymentServices
{
    public interface IPaymobPaymentGateway
    {
        Task<PaymobPaymentResultDto>
            CreatePaymentIntentionAsync(
                int orderId,
                decimal amount,
                string customerEmail,
                string phone,
                string notificationUrl,
                string redirectionUrl);
    }
}
