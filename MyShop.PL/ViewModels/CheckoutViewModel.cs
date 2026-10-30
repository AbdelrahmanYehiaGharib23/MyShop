using System.ComponentModel.DataAnnotations;
using MyShop.DAL.Entities;

namespace MyShop.PL.ViewModels
{
    public class CheckoutViewModel
    {
        public int? OrderId { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32)]
        [RegularExpression("^[a-fA-F0-9]{32}$")]
        public string CheckoutRequestId { get; set; } = string.Empty;

        public List<CartItem> Items { get; set; } = [];

        public decimal TotalPrice { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}
