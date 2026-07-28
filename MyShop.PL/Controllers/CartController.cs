using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Services.CartServices;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        public IActionResult Index()
        {
            var model = new CartViewModel
            {
                Items = _cartService.GetCart(),
                TotalItems = _cartService.GetTotalItems(),
                TotalPrice = _cartService.GetTotalPrice()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId,int quantity=1)
        {
            await _cartService.AddToCart(productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Increase(int productId)
        {
            _cartService.IncreaseQuantity(productId);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Decrease(int productId)
        {
            _cartService.DecreaseQuantity(productId);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();

            return RedirectToAction(nameof(Index));
        }
    }
}
