using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class CartItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
