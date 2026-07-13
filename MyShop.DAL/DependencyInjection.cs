using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyShop.DAL.Contracts;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Presistence.Data.DbInitializer;
using MyShop.DAL.Presistence.Repositories;
using MyShop.DAL.Presistence.UnitOfWork;

namespace MyShop.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();

            });
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));  //Because DI doesn't know what type of T it will use.
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbInitiaizer, DbInitializer>();

            return services;
        }
    }
}
