using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Common
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = null!;
        public string PublishableKey { get; set; } = null!;
    }
}
