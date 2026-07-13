using MyShop.DAL.Contracts;

namespace MyShop.PL.Extensions
{
    public static class InitializerExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using var Scope = app.ApplicationServices.CreateScope();
            var Services = Scope.ServiceProvider;
            var dbInitializer = Services.GetRequiredService<IDbInitiaizer>(); //Ask Explicitly 
            dbInitializer.Initialize();
        }
    }
}