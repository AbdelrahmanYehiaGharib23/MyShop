using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using MyShop.BLL.Models.Dto.CategoryDto;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateCategoryAsync(CreateCategoryDto category)
        {
            var categoryToCreate = _mapper.Map<Category>(category);
            _unitOfWork.CategoryRepository.Add(categoryToCreate);
            return await _unitOfWork.CompleteAsync();

        }
        public async Task<IEnumerable<CategoryDto>> GetCategoryAsync()
        {
            var category = await _unitOfWork.CategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(category);

        }

        public async Task<CategoryDetailsDto?> GetCategoryByIdAsync(int id)
        {
            var category =await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null) return null;
            return _mapper.Map<CategoryDetailsDto>(category);
        }

        public Task<int> UpdateCategoryAsync(UpdateCategoryDto category)
        {
            var categoryUpdate = _mapper.Map<Category>(category);
            _unitOfWork.CategoryRepository.Update(categoryUpdate);
            return _unitOfWork.CompleteAsync();
        }
        public async Task<bool> DeleteCategoryAsync(int? id)
        {
            if (id == null) return false;
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id.Value);
            if (category is null) return false;
            _unitOfWork.CategoryRepository.Remove(category);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
