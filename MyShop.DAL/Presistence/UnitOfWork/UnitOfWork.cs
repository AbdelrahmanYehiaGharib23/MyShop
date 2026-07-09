using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Presistence.Data.DbInitializer;
using MyShop.DAL.Presistence.Repositories;

namespace MyShop.DAL.Presistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly Lazy<IProductRepository> _ProductRepository;
        private readonly Lazy<ICategoryRepository> _CategoryRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _ProductRepository = new Lazy<IProductRepository>(() => new ProductRepository(dbContext));
            _CategoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(dbContext));
        }
        public IProductRepository ProductRepository => _ProductRepository.Value;
        public ICategoryRepository CategoryRepository => _CategoryRepository.Value;
        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        // No need to call Dispose manually here.
        // DbContext is managed by Dependency Injection and registered via AddDbContext.
        // It will be disposed automatically at the end of the request lifecycle.

        //public void Dispose()
        //{
        //    _dbContext.Dispose();
        //}
    }
}
