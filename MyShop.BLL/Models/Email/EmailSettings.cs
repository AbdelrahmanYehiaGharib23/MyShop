using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Email
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;

        public int Port { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
