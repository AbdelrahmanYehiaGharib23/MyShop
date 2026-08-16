using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Specifications
{
    public class OrderSpecification:BaseSpecification<Order>
    {
        public OrderSpecification(string userId) : base(o => o.UserId == userId)
        {
            AddOrderByDescending(o => o.OrderDate);

            AddIncludeChain(query =>
                query
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product));
        }
    }
}
