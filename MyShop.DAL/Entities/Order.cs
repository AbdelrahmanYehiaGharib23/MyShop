using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class Order:BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
