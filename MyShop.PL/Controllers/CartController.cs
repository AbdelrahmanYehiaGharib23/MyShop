using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.OrderServices;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService,IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
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
        [ValidateAntiForgeryToken]
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

        //Checkout
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cartItems = _cartService.GetCart();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new CheckoutViewModel
            {
                Items = cartItems,
                TotalPrice = _cartService.GetTotalPrice()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Items = _cartService.GetCart();
                model.TotalPrice = _cartService.GetTotalPrice();

                return View(model);
            }

            var dto = new CreateOrderDto
            {
                Address = model.Address,
                Phone = model.Phone
            };

            await _orderService.CreateOrderAsync(dto);

            // Clear cart after successful order creation
            _cartService.ClearCart();
            TempData["SuccessMessage"] = "Order placed successfully.";

            return RedirectToAction("MyOrders", "Orders");
        }


    }
}
