using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.CartServices
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IHttpContextAccessor httpContextAccessor,IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);

            _httpContextAccessor.HttpContext!.Session.SetString(CartSessionKey, cartJson);
        }
        public List<CartItem> GetCart()
        {
            var carttJson = _httpContextAccessor.HttpContext!.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(carttJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(carttJson)??new List<CartItem>();
        }

        public async Task AddToCart(int productId, int quantity = 1)
        {
            var cart = GetCart();
            var product =await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if(product is null)
                throw new KeyNotFoundException("Product not found.");
            var cartItem = cart.FirstOrDefault(C => C.ProductId == productId);
            if(cartItem is not null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl=product.ImageUrl,
                    Price = product.Price,
                    Quantity=quantity
                });
            }

           SaveCart(cart);
        }
        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(C => C.ProductId == productId);
            if (cartItem is null) return;
            cart.Remove(cartItem);
            SaveCart(cart);
        }
        public void IncreaseQuantity(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(C => C.ProductId == productId);
            if (cartItem is null) return;
            cartItem.Quantity++;
            SaveCart(cart);
        }
        public void DecreaseQuantity(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(C => C.ProductId == productId);
            if (cartItem is null) return;
            cartItem.Quantity--;
            if (cartItem.Quantity <= 0)
            {
                cart.Remove(cartItem);
            }
            SaveCart(cart);
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext!.Session.Remove(CartSessionKey);
        }
        public decimal GetTotalPrice()
        {
            var cart = GetCart();
            return cart.Sum(C=>C.Price*C.Quantity);
        }

        public int GetTotalItems()
        {
            var cart = GetCart();
            return cart.Sum(C => C.Quantity);
        }
    }
}
