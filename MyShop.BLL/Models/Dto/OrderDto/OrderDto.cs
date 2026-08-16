using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.OrderDto
{
    public class OrderDto
    {
        public int Id { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public List<OrderItemDto> Items { get; set; } = [];
    }
}
