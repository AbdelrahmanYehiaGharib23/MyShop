
using System.Linq.Expressions;
using MyShop.DAL.Contracts.Specifications;

namespace MyShop.DAL.Contracts.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity:BaseEntity
    {
        void Add(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync(bool WithTracking=false);
        Task<TEntity?> GetByIdAsync(int id);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);



        // Specification
        Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity> spec);

        Task<TEntity?> GetEntityWithSpecAsync(ISpecification<TEntity> spec);
        Task<int> CountAsync(ISpecification<TEntity> spec);

    }
}
