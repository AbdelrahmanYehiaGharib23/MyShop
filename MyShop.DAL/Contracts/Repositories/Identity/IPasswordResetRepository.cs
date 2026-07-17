using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Entities.IdentityEntity;

namespace MyShop.DAL.Contracts.Repositories.Identity
{
    public interface IPasswordResetRepository
    {
        Task AddTokenAsync(PasswordResetTokens token);
        Task InvalidateActiveTokensAsync(string userId);
        Task<PasswordResetTokens?> GetValidOtpTokenAsync(string userId, string otpHash);
        Task<PasswordResetTokens?> GetValidResetTokenAsync(string tokenHash);
        Task UpdateTokenAsync(PasswordResetTokens token);
    }
}
