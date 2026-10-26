using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.OrderDto
{
    public class OrderPaymentResultDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;
    }
}
