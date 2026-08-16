using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.OrderDto
{
    public class CreateOrderDto
    {
        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
