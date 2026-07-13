using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork,IMapper map)
        {
            _unitOfWork = unitOfWork;
            _mapper = map;
        }
        public async Task<int> CreateProductAsync(CreateProductDto product)
        {
            var productToCreate = _mapper.Map<Product>(product);
            _unitOfWork.ProductRepository.Add(productToCreate);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<ProductDto>> GetProductAsync()
        {
            var products =await _unitOfWork.ProductRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
        {
            var product =await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product is null) return null;
            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task<int> UpdateProductAsync(UpdateProductDto product)
        {
            var UpdateProduct =_mapper.Map<Product>(product);
            _unitOfWork.ProductRepository.Update(UpdateProduct);
            return await _unitOfWork.CompleteAsync();

        }

        public async Task<bool> DeleteProductAsync(int? id)
        {
            if (id == null) return false;
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id.Value);
            if (product is null) return false;
            _unitOfWork.ProductRepository.Remove(product);
            return await _unitOfWork.CompleteAsync() > 0;

        }

    
    }
}
