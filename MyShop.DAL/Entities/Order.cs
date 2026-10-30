using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Entities.Enum;

namespace MyShop.DAL.Entities
{
    public class Order:BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string? PaymentIntentId { get; set; }

        public OrderStatus Status { get; set; }

        public OrderFulfillmentStatus FulfillmentStatus { get; set; }

        public string? CourierName { get; set; }

        public string? TrackingNumber { get; set; }

        public DateOnly? EstimatedDeliveryDate { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;

        public virtual ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
        public string? PaymobIntentionId { get; set; }

        public string? PaymobOrderId { get; set; }

        // Reused when the browser retries the initial checkout POST.
        public string? CheckoutRequestId { get; set; }
    }
}
