using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.DAL.Entities;
using MyShop.DAL.Entities.Enum;

namespace MyShop.BLL.Services.OrderServices
{
    public interface IOrderService
    {
        Task<int> CreatePendingOrderAsync(CreateOrderDto dto);

        Task<Order?> GetOrderForPaymentAsync(int orderId);

        Task<Order?> GetOrderForCustomerAsync(int orderId);

        // Use only for authenticated and HMAC-validated provider callbacks.
        Task<Order?> GetOrderForPaymobCallbackAsync(int orderId);

        Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);

        Task MarkAsPaidAsync(int orderId);

        Task MarkAsFailedAsync(int orderId);

        Task<IEnumerable<OrderDto>> GetMyOrdersAsync();

        Task<IEnumerable<AdminOrderDto>> GetOrdersForAdminAsync();

        Task<AdminOrderDto?> GetOrderForAdminAsync(int orderId);

        Task UpdateOrderFulfillmentAsync(
            int orderId,
            OrderFulfillmentStatus fulfillmentStatus,
            string? courierName,
            string? trackingNumber,
            DateOnly? estimatedDeliveryDate);
    }
}
