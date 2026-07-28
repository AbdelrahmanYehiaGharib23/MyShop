using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.CartServices
{
    public interface ICartService
    {
        List<CartItem> GetCart();

        Task AddToCart(int productId, int quantity = 1);

        void RemoveFromCart(int productId);

        void IncreaseQuantity(int productId);

        void DecreaseQuantity(int productId);

        void ClearCart();

        decimal GetTotalPrice();

        int GetTotalItems();
    }
}
