using AutoMapper;
using Microsoft.Extensions.Caching.Hybrid;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.BLL.Services.AttachmentServices;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;
using MyShop.DAL.Presistence.Specifications;

namespace MyShop.BLL.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentServices _attachmentServices;
        private readonly HybridCache _cache;

        private const string ProductsListCacheKey = "products:list";
        private const string ProductsListTag = "products:list";
        private const string ProductTagPrefix = "product:";

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper,IAttachmentServices attachmentServices, HybridCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentServices = attachmentServices;
            _cache = cache;
        }

        public async Task<int> CreateProductAsync(CreateProductDto product)
        {
            var productToCreate = _mapper.Map<Product>(product);

            _unitOfWork.ProductRepository.Add(productToCreate);

            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _cache.RemoveByTagAsync(ProductsListTag);
            }

            return result;
        }

        public async Task<IEnumerable<ProductDto>> GetProductAsync()
        {
            return await _cache.GetOrCreateAsync(
                ProductsListCacheKey,
                async cancellationToken =>
                {
                    var spec = new ProductWithCategorySpecification();

                    var products = await _unitOfWork.ProductRepository
                        .GetAllWithSpecAsync(spec);

                    return _mapper.Map<IEnumerable<ProductDto>>(products);
                },
                tags: new[] { ProductsListTag });
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
        {
            var cacheKey = $"product:{id}";
            var productTag = $"{ProductTagPrefix}{id}";

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {
                    var spec = new ProductByIdSpecification(id);

                    var product = await _unitOfWork.ProductRepository
                        .GetEntityWithSpecAsync(spec);

                    if (product is null)
                        return null;

                    return _mapper.Map<ProductDetailsDto>(product);
                },
                tags: new[] { productTag });
        }

        public async Task<int> UpdateProductAsync(UpdateProductDto product)
        {
            var existingProduct = await _unitOfWork.ProductRepository
                .GetByIdAsync(product.Id);

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

            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _cache.RemoveByTagAsync(ProductsListTag);
                await _cache.RemoveByTagAsync($"{ProductTagPrefix}{product.Id}");
            }

            return result;
        }

        public async Task<bool> DeleteProductAsync(int? id)
        {
            if (id == null)
                return false;

            var product = await _unitOfWork.ProductRepository
                .GetByIdAsync(id.Value);

            if (product is null)
                return false;

            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                await _attachmentServices.DeleteAsync(product.ImageUrl);
            }

            _unitOfWork.ProductRepository.Remove(product);

            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _cache.RemoveByTagAsync(ProductsListTag);
                await _cache.RemoveByTagAsync($"{ProductTagPrefix}{id.Value}");

                return true;
            }

            return false;
        }

        public async Task<Pagination<ProductDto>> SearchProductsAsync(
            ProductSpecParams specParams)
        {
            var spec = new ProductSpecification(specParams);

            var products = await _unitOfWork.ProductRepository
                .GetAllWithSpecAsync(spec);

            var countSpec = new ProductCountSpecification(specParams);

            var totalItems = await _unitOfWork.ProductRepository
                .CountAsync(countSpec);

            return new Pagination<ProductDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                Count = totalItems,
                Data = _mapper.Map<IReadOnlyList<ProductDto>>(products)
            };
        }

        public async Task<IEnumerable<DeletedProductDto>> GetDeletedProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository
                .GetDeletedProductsAsync();

            return products.Select(p => new DeletedProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                DeletedAt = p.DeletedAt,
                DeletedBy = p.DeletedBy
            });
        }

        public async Task<bool> RestoreProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository
                .GetDeletedProductByIdAsync(id);

            if (product == null)
                return false;

            product.IsDeleted = false;
            product.DeletedAt = null;
            product.DeletedBy = null;

            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _cache.RemoveByTagAsync(ProductsListTag);
                await _cache.RemoveByTagAsync($"{ProductTagPrefix}{id}");

                return true;
            }

            return false;
        }
    }
}