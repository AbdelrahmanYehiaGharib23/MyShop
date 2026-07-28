using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Specifications;

namespace MyShop.DAL.Presistence.Specifications
{
    public static class SpecificationEvaluator<TEntity> where TEntity :BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(
           IQueryable<TEntity> inputQuery,
           ISpecification<TEntity> specification)
        {
            var query = inputQuery.Where(entity => !entity.IsDeleted);

            if (specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            query = specification.Includes.Aggregate(query,
                (currentQuery, includeExpression)
                    => currentQuery.Include(includeExpression));

            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            return query;
        }
    }
}
