using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.PaymentServices;

namespace MyShop.PL.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;

        public PaymentController(
            IPaymentService paymentService,
            ICartService cartService)
        {
            _paymentService = paymentService;
            _cartService = cartService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentIntent()
        {
            var cart = _cartService.GetCart();

            if (cart.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Your cart is empty."
                });
            }

            var total = _cartService.GetTotalPrice();

            if (total <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid cart total."
                });
            }

            var paymentIntent =
                await _paymentService.CreatePaymentIntentAsync(total);

            return Ok(new
            {
                clientSecret = paymentIntent.ClientSecret,
                paymentIntentId = paymentIntent.PaymentIntentId
            });
        }
    }
}
