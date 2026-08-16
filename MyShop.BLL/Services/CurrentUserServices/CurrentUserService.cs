using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MyShop.BLL.Services.CurrentUserServices
{
    public class CurrentUserService:ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public string? GetCurrentUserEmail()
        {
            return _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.Email)?
                .Value;
        }

    }
}
