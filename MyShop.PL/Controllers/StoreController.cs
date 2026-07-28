using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Services.ProductServices;
using MyShop.DAL.Presistence.Specifications;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductService _productService;

        public StoreController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sort, int pageIndex = 1, int pageSize = 9)
        {
            var specParams = new ProductSpecParams
            {
                Search = searchTerm,
                Sort = sort,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var products = await _productService.SearchProductsAsync(specParams);

            if (products.Count > 0 && products.Data.Count == 0 && specParams.PageIndex > 1)
            {
                specParams.PageIndex = (int)Math.Ceiling(products.Count / (double)specParams.PageSize);
                products = await _productService.SearchProductsAsync(specParams);
            }

            var model = new ShopViewModel
            {
                SearchTerm = specParams.Search,
                Sort = specParams.Sort,
                PageIndex = products.PageIndex,
                PageSize = products.PageSize,
                Count = products.Count,
                Products = products.Data
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
