using System.ComponentModel.DataAnnotations;

namespace MyShop.PL.ViewModels.Identity
{
    public class ProfileRequestViewModel
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
