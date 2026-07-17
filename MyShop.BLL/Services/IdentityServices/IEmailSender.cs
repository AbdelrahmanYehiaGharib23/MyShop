using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Services.IdentityServices
{
    public interface IEmailSender
    {
        /// <summary>
        /// Send an email asynchronously.
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="htmlMessage">Email HTML message body</param>
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
