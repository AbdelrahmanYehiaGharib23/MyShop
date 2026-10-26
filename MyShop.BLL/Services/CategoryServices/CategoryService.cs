using AutoMapper;
using MyShop.BLL.Models.Dto.CategoryDto;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;
using Microsoft.Extensions.Caching.Hybrid;

namespace MyShop.BLL.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;
        private const string CategoryCacheKey = "CategoryList";
        public CategoryService( IUnitOfWork unitOfWork,IMapper mapper,HybridCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<int> CreateCategoryAsync(CreateCategoryDto category)
        {
            var categoryToCreate = _mapper.Map<Category>(category);
            _unitOfWork.CategoryRepository.Add(categoryToCreate);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
                await _cache.RemoveAsync(CategoryCacheKey);

            return result;

        }
        public async Task<IEnumerable<CategoryDto>> GetCategoryAsync()
        {
            return await _cache.GetOrCreateAsync(
                CategoryCacheKey,
                async cancellationToken =>
                {
                    var categoriesFromDb =
                        await _unitOfWork.CategoryRepository.GetAllAsync();

                    return _mapper.Map<IEnumerable<CategoryDto>>(categoriesFromDb);
                });
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
                await _cache.RemoveAsync(CategoryCacheKey);

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
                await _cache.RemoveAsync(CategoryCacheKey);
                return true;
            }

            return false;
        }
    }
}
