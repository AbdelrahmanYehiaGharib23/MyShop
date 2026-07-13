using System.ComponentModel.DataAnnotations;

namespace MyShop.PL.ViewModels.Identity
{
    public class LoginVM
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
