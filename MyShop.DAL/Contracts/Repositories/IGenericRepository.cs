using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Entities;

namespace MyShop.DAL.Contracts.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity:BaseEntity
    {
        void Add(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync(bool WithTracking=false);
        Task<TEntity?> GetByIdAsync(int id);
        void Remove(TEntity entity);
        void Update(TEntity entity);

    }
}
