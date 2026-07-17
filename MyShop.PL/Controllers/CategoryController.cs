using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Models.Dto.CategoryDto;
using MyShop.BLL.Services.CategoryServices;
using MyShop.PL.ViewModels;

namespace MyShop.PL.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public CategoryController( ILogger<CategoryController> logger, ICategoryService categoryServic, IMapper mapper, IWebHostEnvironment environment)
        {
            _logger = logger;
            _categoryService = categoryServic;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Categories = await _categoryService.GetCategoryAsync();
            return View(Categories);
        }

        

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoryDto = _mapper.Map<CreateCategoryDto>(categoryVM);
                    int result = await _categoryService.CreateCategoryAsync(categoryDto);
                    if (result > 0)
                    {
                        TempData["Message"] = $"Category {categoryVM.Name} created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        TempData["Message"] = $"Failed to create category {categoryVM.Name}";
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }
            return View(categoryVM);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category is null) return NotFound();
            var categoryVm = _mapper.Map<CategoryVM>(category);
            return View(categoryVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, CategoryVM categoryVM)
        {
            if (id != categoryVM.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var updateDto = _mapper.Map<UpdateCategoryDto>(categoryVM);
                    var result = await _categoryService.UpdateCategoryAsync(updateDto);
                    if (result > 0) return RedirectToAction(nameof(Index));
                    ModelState.AddModelError(string.Empty, "Failed to update category");
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
            return View(categoryVM);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == 0) return BadRequest();

            try
            {
                bool deleted = await _categoryService.DeleteCategoryAsync(id);
                if (deleted)
                {
                    TempData["SuccessMessage"] = "Category deleted successfully";
                    _logger.LogInformation($"Category deleted: ID={id}");
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to delete category");
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return View("ErrorView", ex);
            }
        }
    }
}
