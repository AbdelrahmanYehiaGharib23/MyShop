using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Common
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;

        public string PublishableKey { get; set; } = string.Empty;

        public string WebhookSecret { get; set; } = string.Empty;

        public decimal EgpPerUsd { get; set; } = 51.83m;
    }
}
