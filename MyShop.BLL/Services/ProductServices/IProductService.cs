using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.DAL.Entities;

namespace MyShop.BLL.Services.ProductServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductAsync();
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);
        Task<int> CreateProductAsync(CreateProductDto product);
        Task<int> UpdateProductAsync(UpdateProductDto product);
        Task<bool> DeleteProductAsync(int? id);

    }
}
