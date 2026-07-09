using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MyShop.DAL.Entities.IdentityEntity
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
