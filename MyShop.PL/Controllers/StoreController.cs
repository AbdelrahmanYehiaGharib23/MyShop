using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyShop.BLL.Services.CategoryServices;
using MyShop.BLL.Services.FavoriteServices;
using MyShop.BLL.Services.ProductServices;
using MyShop.BLL.Services.ReviewServices;
using MyShop.DAL.Presistence.Specifications;
using MyShop.PL.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace MyShop.PL.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly IFavoriteService _favoriteService;

        public StoreController(IProductService productService, ICategoryService categoryService, IReviewService reviewService, IFavoriteService favoriteService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sort, int? categoryId, int pageIndex = 1, int pageSize = 9)
        {
            var specParams = new ProductSpecParams
            {
                Search = searchTerm,
                Sort = sort,
                CategoryId = categoryId > 0 ? categoryId : null,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var products = await _productService.SearchProductsAsync(specParams);

            if (products.Count > 0 && products.Data.Count == 0 && specParams.PageIndex > 1)
            {
                specParams.PageIndex = (int)Math.Ceiling(products.Count / (double)specParams.PageSize);
                products = await _productService.SearchProductsAsync(specParams);
            }

            var reviews = await _reviewService.GetReviewSummariesAsync(products.Data.Select(product => product.Id));
            foreach (var product in products.Data)
            {
                if (reviews.TryGetValue(product.Id, out var summary))
                {
                    product.AverageRating = summary.AverageRating;
                    product.ReviewCount = summary.ReviewCount;
                }
            }

            var model = new ShopViewModel
            {
                SearchTerm = specParams.Search,
                Sort = specParams.Sort,
                CategoryId = specParams.CategoryId,
                PageIndex = products.PageIndex,
                PageSize = products.PageSize,
                Count = products.Count,
                Products = products.Data,
                Categories = await _categoryService.GetCategoryAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            ViewData["ReviewSummary"] = await _reviewService.GetReviewSummaryAsync(id);

            return View(product);
        }

        public IActionResult Favorites()
        {
            return View();
        }

        [HttpGet("/robots.txt")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult Robots()
        {
            var origin = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var content = string.Join('\n',
                "User-agent: *",
                "Allow: /",
                "Disallow: /Account/",
                "Disallow: /Cart/",
                "Disallow: /Orders/",
                "Disallow: /Review/",
                $"Sitemap: {origin}/sitemap.xml");

            return Content(content, "text/plain", Encoding.UTF8);
        }

        [HttpGet("/sitemap.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Sitemap()
        {
            XNamespace sitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var urls = new List<string>();
            var storeUrl = Url.Action("Index", "Store", null, Request.Scheme);
            if (!string.IsNullOrWhiteSpace(storeUrl))
                urls.Add(storeUrl);

            foreach (var category in await _categoryService.GetCategoryAsync())
            {
                var categoryUrl = Url.Action("Index", "Store", new { categoryId = category.Id }, Request.Scheme);
                if (!string.IsNullOrWhiteSpace(categoryUrl))
                    urls.Add(categoryUrl);
            }

            foreach (var product in await _productService.GetProductAsync())
            {
                var productUrl = Url.Action("Details", "Store", new { id = product.Id }, Request.Scheme);
                if (!string.IsNullOrWhiteSpace(productUrl))
                    urls.Add(productUrl);
            }

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(sitemapNamespace + "urlset",
                    urls.Distinct(StringComparer.OrdinalIgnoreCase).Select(url =>
                        new XElement(sitemapNamespace + "url",
                            new XElement(sitemapNamespace + "loc", url)))));

            return Content(document.ToString(SaveOptions.DisableFormatting), "application/xml", Encoding.UTF8);
        }

        [HttpGet]
        public async Task<IActionResult> FavoritesData()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Ok(new { favorites = Array.Empty<object>() });

            var favorites = await _favoriteService.GetCurrentFavoritesAsync();
            var reviews = await _reviewService.GetReviewSummariesAsync(favorites.Select(product => product.Id));
            foreach (var product in favorites)
            {
                if (reviews.TryGetValue(product.Id, out var summary))
                {
                    product.AverageRating = summary.AverageRating;
                    product.ReviewCount = summary.ReviewCount;
                }
            }

            return Ok(new { favorites });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteProductRequest request)
        {
            if (!ModelState.IsValid || request is null || request.ProductId <= 0)
                return BadRequest(new { message = "Choose a valid product." });

            try
            {
                await _favoriteService.AddAsync(request.ProductId);
                return Ok(new { success = true });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "This product is no longer available." });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite([FromBody] FavoriteProductRequest request)
        {
            if (!ModelState.IsValid || request is null || request.ProductId <= 0)
                return BadRequest(new { message = "Choose a valid product." });

            await _favoriteService.RemoveAsync(request.ProductId);
            return Ok(new { success = true });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncFavorites([FromBody] FavoriteSyncRequest request)
        {
            if (!ModelState.IsValid || request is null || request.ProductIds is null || request.ProductIds.Count > 100)
                return BadRequest(new { message = "The favorites list is invalid or too large." });

            await _favoriteService.SyncAsync(request.ProductIds);
            return Ok(new { favorites = await _favoriteService.GetCurrentFavoritesAsync() });
        }
    }

    public sealed class FavoriteProductRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }

    public sealed class FavoriteSyncRequest
    {
        public List<int> ProductIds { get; set; } = [];
    }
}
