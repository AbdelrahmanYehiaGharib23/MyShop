using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.BLL.Services.AttachmentServices;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentServices _attachmentServices;

        public ProductService(IUnitOfWork unitOfWork,IMapper map,IAttachmentServices attachmentServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = map;
            _attachmentServices = attachmentServices;
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
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.Id);

            if (existingProduct == null)
                return 0;

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                if (!string.IsNullOrWhiteSpace(existingProduct.ImageUrl))
                {
                    await _attachmentServices.DeleteAsync(existingProduct.ImageUrl);
                }

                existingProduct.ImageUrl = product.ImageUrl;
            }

            _unitOfWork.ProductRepository.Update(existingProduct);

            return await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeleteProductAsync(int? id)
        {
            if (id == null) return false;
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id.Value);
            if (product is null) return false;
            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
               await _attachmentServices.DeleteAsync(product.ImageUrl);
            }
            _unitOfWork.ProductRepository.Remove(product);
            return await _unitOfWork.CompleteAsync() > 0;

        }

    
    }
}
