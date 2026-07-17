using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Services.IdentityServices
{
    public interface IPasswordResetManager
    {
        Task SendOtpAsync(string email);
        Task<string> VerifyOtpAsync(string email, string otp);
        Task ResetPasswordAsync(string token, string newPassword);
    }
}
