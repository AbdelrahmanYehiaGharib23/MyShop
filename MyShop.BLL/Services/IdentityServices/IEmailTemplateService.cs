using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Services.IdentityServices
{
    public interface IEmailTemplateService
    {
      
            string OrderConfirmation(int orderId,decimal totalPrice,string address,string phone);
      
    }
}
