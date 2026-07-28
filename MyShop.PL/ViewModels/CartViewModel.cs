using MyShop.DAL.Entities;

namespace MyShop.PL.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = [];

        public decimal TotalPrice { get; set; }

        public int TotalItems { get; set; }
    }
}
