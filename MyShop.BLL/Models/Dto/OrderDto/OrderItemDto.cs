using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.OrderDto
{
    public class OrderItemDto
    {
        public int? ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ProductImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}
