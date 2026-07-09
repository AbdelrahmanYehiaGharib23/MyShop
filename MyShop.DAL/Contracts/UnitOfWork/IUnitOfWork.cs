using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;

namespace MyShop.DAL.Contracts.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        Task<int> CompleteAsync();
        //void Dispose();
    }
}
