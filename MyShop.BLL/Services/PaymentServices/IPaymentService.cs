using MyShop.BLL.Models.Dto.OrderDto;

namespace MyShop.BLL.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<OrderPaymentResultDto> CreateOrUpdatePaymentIntentAsync(
            int orderId,
            decimal amount,
            string currency = "egp");

        Task<bool> ConfirmPaymentSucceededAsync(
            int orderId,
            string paymentIntentId,
            decimal amount,
            string currency = "egp");
    }
}
