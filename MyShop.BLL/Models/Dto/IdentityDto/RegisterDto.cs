using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.IdentityDto
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;
    }
}
