using System.ComponentModel.DataAnnotations;
using MyShop.DAL.Entities;

namespace MyShop.PL.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> Items { get; set; } = [];

        public decimal TotalPrice { get; set; }

        // Delivery Information
        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string? PaymentIntentId { get; set; }
    }
}
