using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.EmailTemplates;

namespace MyShop.BLL.Services.IdentityServices
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string OrderConfirmation(
            int orderId,
            decimal totalPrice,
            string address,
            string phone)
        {
            return OrderConfirmationTemplate.Render(
                orderId,
                totalPrice,
                address,
                phone);
        }
    }
}

