using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Entites;
using MyShop.DAL.Presistence.Data.DbInitializer;

namespace MyShop.DAL.Presistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(TEntity entity) => _dbContext.Set<TEntity>().Add(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool WithTracking = false)
        {
            if (WithTracking)
            {
                return await _dbContext.Set<TEntity>().Where(T => T.IsDeleted != true).ToListAsync();
            }
            else
            {
                return await _dbContext.Set<TEntity>().Where(T => T.IsDeleted != true).AsNoTracking().ToListAsync();
            }
        }

        public async Task<TEntity?> GetByIdAsync(int id) =>await _dbContext.Set<TEntity>().FindAsync(id);
        public void Remove(TEntity entity)
        {
            var isDeletedProp = entity.GetType().GetProperty("IsDeleted");
            var deletedAtProp = entity.GetType().GetProperty("DeletedAt");
            if(isDeletedProp != null)
            {
                // Soft delete via property
                isDeletedProp.SetValue(entity, true);
                if (deletedAtProp != null)
                    deletedAtProp.SetValue(entity, DateTime.UtcNow);

                _dbContext.Set<TEntity>().Update(entity);

            }
            else
            {
                // Hard delete
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                    _dbContext.Set<TEntity>().Attach(entity);

                _dbContext.Set<TEntity>().Remove(entity);
            }
        }

        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);
    }
}
