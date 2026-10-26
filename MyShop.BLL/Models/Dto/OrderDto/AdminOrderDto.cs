using MyShop.DAL.Entities.Enum;

namespace MyShop.BLL.Models.Dto.OrderDto
{
    public sealed class AdminOrderDto
    {
        public int Id { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public OrderFulfillmentStatus FulfillmentStatus { get; set; }

        public string? CourierName { get; set; }

        public string? TrackingNumber { get; set; }

        public DateOnly? EstimatedDeliveryDate { get; set; }

        public List<OrderItemDto> Items { get; set; } = [];
    }
}
