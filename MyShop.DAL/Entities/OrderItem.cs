using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class OrderItem:BaseEntity
    {
        public int OrderId { get; set; }

        public virtual Order Order { get; set; } = null!;

        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ProductImageUrl { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}
