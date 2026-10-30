using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.PL.ViewModels.Identity;

namespace MyShop.PL.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = StatusCodes.Status500InternalServerError
            });
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ErrorStatus(int statusCode)
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                StatusCode = statusCode
            });
        }
    }
}
