using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.OrderServices;
using MyShop.BLL.Services.PaymentServices;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly StripeSettings _stripeSettings;

        public CartController(
            ICartService cartService,
            IOrderService orderService,
            IPaymentService paymentService,
            IOptions<StripeSettings> stripeOptions)
        {
            _cartService = cartService;
            _orderService = orderService;
            _paymentService = paymentService;
            _stripeSettings = stripeOptions.Value;
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
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
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
            var cartItems = _cartService.GetCart();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Items = cartItems;
                model.TotalPrice = _cartService.GetTotalPrice();

                return View(model);
            }

            var totalPrice = _cartService.GetTotalPrice();
            var stripeIsConfigured =
                !string.IsNullOrWhiteSpace(_stripeSettings.PublishableKey)
                && !string.IsNullOrWhiteSpace(_stripeSettings.SecretKey);

            if (stripeIsConfigured)
            {
                if (string.IsNullOrWhiteSpace(model.PaymentIntentId))
                {
                    ModelState.AddModelError(string.Empty, "Please complete the Stripe payment before placing your order.");
                    model.Items = cartItems;
                    model.TotalPrice = totalPrice;

                    return View(model);
                }

                var paymentSucceeded = await _paymentService.IsPaymentIntentSucceededAsync(
                    model.PaymentIntentId,
                    totalPrice);

                if (!paymentSucceeded)
                {
                    ModelState.AddModelError(string.Empty, "Payment was not confirmed by Stripe. Please try again.");
                    model.Items = cartItems;
                    model.TotalPrice = totalPrice;

                    return View(model);
                }
            }

            var dto = new CreateOrderDto
            {
                Address = model.Address,
                Phone = model.Phone
            };

            await _orderService.CreateOrderAsync(dto);

            _cartService.ClearCart();
            TempData["SuccessMessage"] = stripeIsConfigured
                ? "Payment successful. Order placed and confirmation email sent."
                : "Order placed successfully.";

            return RedirectToAction("MyOrders", "Orders");
        }
    }
}
