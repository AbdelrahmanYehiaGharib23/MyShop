using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Contracts.Repositories.Identity;

namespace MyShop.DAL.Contracts.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        IPasswordResetRepository PasswordResetRepository { get; }
        IOrderRepository OrderRepository { get; }
        IReviewRepository ReviewRepository { get; }
        IGenericRepository<MyShop.DAL.Entities.FavoriteProduct> FavoriteRepository { get; }

        Task<int> CompleteAsync();
        //void Dispose();
    }
}
