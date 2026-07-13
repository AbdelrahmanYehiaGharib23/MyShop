using System;
using System.Collections.Generic;
using System.Text;
using MyShop.DAL.Contracts;

namespace MyShop.DAL.Presistence.Data.DbInitializer
{
    public class DbInitializer:IDbInitiaizer
    {
        private readonly ApplicationDbContext _dbContext;

        public DbInitializer(ApplicationDbContext dbContext)
        {
              _dbContext = dbContext;
        }
        public void Initialize()
        {
            _dbContext.Database.Migrate();   //UpdateDataBase
            if (_dbContext.Database.GetPendingMigrations().Any()) {
                _dbContext.Database.Migrate();
            }
            ;
        }

        
    }
}
