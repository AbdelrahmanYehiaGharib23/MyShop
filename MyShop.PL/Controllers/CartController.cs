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
        private readonly StripeSettings _stripeSettings;

        public CartController(
            ICartService cartService,
            IOrderService orderService,
            IOptions<StripeSettings> stripeOptions)
        {
            _cartService = cartService;
            _orderService = orderService;
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
        public async Task<IActionResult> AddToCart(
            int productId,
            int quantity = 1)
        {
            await _cartService.AddToCart(
                productId,
                quantity);

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
        public async Task<IActionResult> Checkout(
            int? orderId = null)
        {
            var cartItems = _cartService.GetCart();

            var model = new CheckoutViewModel
            {
                OrderId = orderId
            };

            ViewBag.StripePublishableKey =
                _stripeSettings.PublishableKey;

            if (orderId.HasValue)
            {
                var order =
                    await _orderService
                        .GetOrderForPaymentAsync(
                            orderId.Value);

                if (order is null)
                    return NotFound();

                model.Items = order.OrderItems.Select(item => new MyShop.DAL.Entities.CartItem
                {
                    ProductId = item.ProductId ?? 0,
                    ProductName = item.ProductName,
                    ImageUrl = item.ProductImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList();
                model.TotalPrice = order.TotalPrice;
                model.Address = order.Address;
                model.Phone = order.Phone;
            }
            else
            {
                if (cartItems is null || !cartItems.Any())
                    return RedirectToAction(nameof(Index));

                model.Items = cartItems;
                model.TotalPrice = _cartService.GetTotalPrice();
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            CheckoutViewModel model)
        {
            var cartItems = _cartService.GetCart();

            if (model.OrderId.HasValue)
            {
                return RedirectToAction(
                    nameof(Checkout),
                    new { orderId = model.OrderId.Value });
            }

            if (cartItems is null || !cartItems.Any())
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
            {
                model.Items = cartItems;
                model.TotalPrice =
                    _cartService.GetTotalPrice();

                ViewBag.StripePublishableKey =
                    _stripeSettings.PublishableKey;

                return View(model);
            }

            var dto = new CreateOrderDto
            {
                Address = model.Address,
                Phone = model.Phone
            };

            var orderId =
                await _orderService
                    .CreatePendingOrderAsync(dto);

            _cartService.ClearCart();

            return RedirectToAction(
                nameof(Checkout),
                new { orderId });
        }
    }
}
