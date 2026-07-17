using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.BLL.Services.AttachmentServices;
using MyShop.BLL.Services.CategoryServices;
using MyShop.BLL.Services.ProductServices;
using MyShop.DAL.Presistence.Data.DbInitializer;

namespace MyShop.PL.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IAttachmentServices _attachmentServices;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService,IAttachmentServices attachmentServices, IWebHostEnvironment webHostEnvironment,IMapper mapper,ILogger<ProductController> logger,ICategoryService categoryService)
        {
            _productService = productService;
             _attachmentServices = attachmentServices;
            _environment = webHostEnvironment;
            _mapper = mapper;
            _logger = logger;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductAsync();
            return View(products);
        }


        //I will fetch the category data associated with the product for display.
        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        {
            var categories = await _categoryService.GetCategoryAsync();

            return categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }
       
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductVM productVM = new ProductVM
            {
                CategoryList = await GetCategorySelectListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (productVM.Image != null)
                    {
                        productVM.ImageUrl = await _attachmentServices.UploadAsync(productVM.Image, "Products");

                        if (productVM.ImageUrl == null)
                        {
                            ModelState.AddModelError(nameof(productVM.Image), "Invalid image. Only JPG, JPEG and PNG files up to 2 MB are allowed.");
                            productVM.CategoryList = await GetCategorySelectListAsync();

                            return View(productVM);
                        }
                    }
                    var productDto = _mapper.Map<CreateProductDto>(productVM);
                    int result = await _productService.CreateProductAsync(productDto);
                    if (result > 0)
                    {
                        TempData["Message"] = $"Product {productVM.Name} created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        TempData["Message"] = $"Failed to create Produt {productVM.Name}";
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }
            productVM.CategoryList = await GetCategorySelectListAsync();
            return View(productVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var product = await _productService.GetProductByIdAsync(id.Value);

            if (product == null)
                return NotFound();

            var productVM = _mapper.Map<ProductVM>(product);

            productVM.CategoryList = await GetCategorySelectListAsync();

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id,ProductVM productVM)
        {
            if (id != productVM.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    if (productVM.Image != null)
                    {
                        productVM.ImageUrl = await _attachmentServices.UploadAsync(productVM.Image, "Products");

                        if (productVM.ImageUrl == null)
                        {
                            ModelState.AddModelError(nameof(productVM.Image),
                                "Invalid image. Only JPG, JPEG and PNG files up to 2 MB are allowed.");
                            productVM.CategoryList = await GetCategorySelectListAsync();

                            return View(productVM);
                        }
                    }

                    var updateDto = _mapper.Map<UpdateProductDto>(productVM);
                    var result =await _productService.UpdateProductAsync(updateDto);
                    if (result > 0) return RedirectToAction(nameof(Index));
                    ModelState.AddModelError(string.Empty, "Failed to update product");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                    {
                        _logger.LogError(ex.Message);
                        return View("ErrorView", ex);
                    }
                }
            }
            productVM.CategoryList = await GetCategorySelectListAsync();
            return View(productVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var product = await _productService.GetProductByIdAsync(id.Value);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == 0) return BadRequest();

            try
            {
                bool deleted = await _productService.DeleteProductAsync(id);
                if (deleted)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully";
                    _logger.LogInformation($"Product deleted: ID={id}");
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to delete product");
                var product = await _productService.GetProductByIdAsync(id);
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return View("ErrorView", ex);
            }
        }


    }
}
