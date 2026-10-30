using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Contracts.Repositories.Identity;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Presistence.Data.DbInitializer;
using MyShop.DAL.Presistence.Repositories;
using MyShop.DAL.Presistence.Repositories.Identity;

namespace MyShop.DAL.Presistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly Lazy<IProductRepository> _ProductRepository;
        private readonly Lazy<ICategoryRepository> _CategoryRepository;
        private readonly Lazy<IPasswordResetRepository> _PasswordResetRepository;
        private readonly Lazy<IOrderRepository> _OrderRepository;
        private readonly Lazy<IReviewRepository> _ReviewRepository;
        private readonly Lazy<IGenericRepository<MyShop.DAL.Entities.FavoriteProduct>> _FavoriteRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _ProductRepository = new Lazy<IProductRepository>(() => new ProductRepository(dbContext));
            _CategoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(dbContext));
            _PasswordResetRepository = new Lazy<IPasswordResetRepository>(() => new PasswordResetRepository(dbContext));
            _OrderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(dbContext));
            _ReviewRepository = new Lazy<IReviewRepository>(() => new ReviewRepository(dbContext));
            _FavoriteRepository = new Lazy<IGenericRepository<MyShop.DAL.Entities.FavoriteProduct>>(() => new GenericRepository<MyShop.DAL.Entities.FavoriteProduct>(dbContext));
        }
        public IProductRepository ProductRepository => _ProductRepository.Value;
        public ICategoryRepository CategoryRepository => _CategoryRepository.Value;
        public IPasswordResetRepository PasswordResetRepository => _PasswordResetRepository.Value;
        public IOrderRepository OrderRepository => _OrderRepository.Value;
        public IReviewRepository ReviewRepository => _ReviewRepository.Value;
        public IGenericRepository<MyShop.DAL.Entities.FavoriteProduct> FavoriteRepository => _FavoriteRepository.Value;
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
