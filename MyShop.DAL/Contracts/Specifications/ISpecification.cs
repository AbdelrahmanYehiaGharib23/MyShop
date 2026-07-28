using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MyShop.DAL.Contracts.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; }

        List<Expression<Func<T, object>>> Includes { get; }
      
        // Sorting
        Expression<Func<T, object>>? OrderBy { get; }

        Expression<Func<T, object>>? OrderByDescending { get; }

       
        // Pagination
        int Skip { get; }

        int Take { get; }

        bool IsPagingEnabled { get; }
    }
}
