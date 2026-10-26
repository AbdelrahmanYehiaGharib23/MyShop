using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.OrderServices
{
    public interface IOrderService
    {
        Task<int> CreatePendingOrderAsync(CreateOrderDto dto);

        Task<Order?> GetOrderForPaymentAsync(int orderId);

        // Use only for authenticated and HMAC-validated provider callbacks.
        Task<Order?> GetOrderForPaymobCallbackAsync(int orderId);

        Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);

        Task MarkAsPaidAsync(int orderId);

        Task MarkAsFailedAsync(int orderId);

        Task<IEnumerable<OrderDto>> GetMyOrdersAsync();
    }
}
