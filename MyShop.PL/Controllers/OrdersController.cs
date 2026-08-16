using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Services.OrderServices;

namespace MyShop.PL.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync();

            return View(orders);
        }
    }
}

