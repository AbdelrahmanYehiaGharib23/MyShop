using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Services.CurrentUserServices
{
    public interface ICurrentUserService
    {
        string? GetCurrentUserId();
        string? GetCurrentUserEmail();
    }
}
