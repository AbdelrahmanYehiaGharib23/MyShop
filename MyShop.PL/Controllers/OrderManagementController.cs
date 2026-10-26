using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Services.OrderServices;
using MyShop.DAL.Entities.Enum;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [AutoValidateAntiforgeryToken]
    public sealed class OrderManagementController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderManagementController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersForAdminAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var order = await _orderService.GetOrderForAdminAsync(id);
            if (order is null)
                return NotFound();

            return View(new OrderManagementDetailsVM
            {
                Order = order,
                FulfillmentStatus = order.FulfillmentStatus,
                CourierName = order.CourierName,
                TrackingNumber = order.TrackingNumber,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFulfillment(
            int id,
            OrderManagementDetailsVM model)
        {
            if (id <= 0 || id != model.Order.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _orderService.UpdateOrderFulfillmentAsync(
                        id,
                        model.FulfillmentStatus,
                        model.CourierName,
                        model.TrackingNumber,
                        model.EstimatedDeliveryDate);

                    TempData["SuccessMessage"] = $"Order #{id} fulfillment was updated.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }

            var order = await _orderService.GetOrderForAdminAsync(id);
            if (order is null)
                return NotFound();

            model.Order = order;
            return View("Details", model);
        }
    }
}
