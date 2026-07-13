using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.CategoryDto;

namespace MyShop.BLL.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoryAsync();
        Task<CategoryDetailsDto?> GetCategoryByIdAsync(int id);
        Task<int> CreateCategoryAsync(CreateCategoryDto category);
        Task<int> UpdateCategoryAsync(UpdateCategoryDto category);
        Task<bool> DeleteCategoryAsync(int? id);
    }
}
