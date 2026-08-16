using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Presistence.Data.DbInitializer;

namespace MyShop.DAL.Presistence.Repositories
{
    public class OrderRepository:GenericRepository<Order>,IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
