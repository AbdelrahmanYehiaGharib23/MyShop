using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MyShop.BLL.Models.Dto.CategoryDto;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private const string CategoryCacheKey = "CategoryList";
        public CategoryService(IUnitOfWork unitOfWork,IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<int> CreateCategoryAsync(CreateCategoryDto category)
        {
            var categoryToCreate = _mapper.Map<Category>(category);
            _unitOfWork.CategoryRepository.Add(categoryToCreate);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
                _memoryCache.Remove(CategoryCacheKey);

            return result;

        }
        public async Task<IEnumerable<CategoryDto>> GetCategoryAsync()
        {
            if (!_memoryCache.TryGetValue(CategoryCacheKey, out IEnumerable<CategoryDto>? categories))
            {
                var category = await _unitOfWork.CategoryRepository.GetAllAsync();

                categories = _mapper.Map<IEnumerable<CategoryDto>>(category);

                _memoryCache.Set(
                    CategoryCacheKey,
                    categories,
                    TimeSpan.FromMinutes(30));
            }

            return categories!;
        }

        public async Task<CategoryDetailsDto?> GetCategoryByIdAsync(int id)
        {
            var category =await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null) return null;
            return _mapper.Map<CategoryDetailsDto>(category);
        }

        public async Task<int> UpdateCategoryAsync(UpdateCategoryDto category)
        {
            var categoryUpdate = _mapper.Map<Category>(category);
            _unitOfWork.CategoryRepository.Update(categoryUpdate);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
                _memoryCache.Remove(CategoryCacheKey);

            return result;
        }
        public async Task<bool> DeleteCategoryAsync(int? id)
        {
            if (id == null) return false;
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id.Value);
            if (category is null) return false;
            _unitOfWork.CategoryRepository.Remove(category);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                _memoryCache.Remove(CategoryCacheKey);
                return true;
            }

            return false;
        }
    }
}
