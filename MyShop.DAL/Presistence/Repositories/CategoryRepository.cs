using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Entites;
using MyShop.DAL.Presistence.Data.DbInitializer;

namespace MyShop.DAL.Presistence.Repositories
{
    public class CategoryRepository(ApplicationDbContext dbContext):GenericRepository<Category>(dbContext),ICategoryRepository
    {
    }
}
