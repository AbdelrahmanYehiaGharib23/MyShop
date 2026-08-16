using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Entities;
using MyShop.DAL.Presistence.Data.DbInitializer;

namespace MyShop.DAL.Presistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetDeletedProductsAsync()
        {
            return await _dbContext.Products
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetDeletedProductByIdAsync(int id)
        {
            return await _dbContext.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);
        }
    }
}
