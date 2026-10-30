using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyShop.IntegrationTests.Infrastructure;

public sealed class MyShopWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string DefaultConnectionStringKey =
        "ConnectionStrings__DefaultConnection";
    private const string RedisConnectionStringKey = "ConnectionStrings__Redis";

    private readonly string? _originalEnvironment =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    private readonly string? _originalDefaultConnection =
        Environment.GetEnvironmentVariable(DefaultConnectionStringKey);
    private readonly string? _originalRedisConnection =
        Environment.GetEnvironmentVariable(RedisConnectionStringKey);

    public MyShopWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable(
            DefaultConnectionStringKey,
            "Server=localhost;Database=MyShop.IntegrationTests;User Id=test;Password=unused;TrustServerCertificate=True");
        Environment.SetEnvironmentVariable(
            RedisConnectionStringKey,
            "rediss://default:integration-test@localhost:6379");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        Environment.SetEnvironmentVariable(
            "ASPNETCORE_ENVIRONMENT",
            _originalEnvironment);
        Environment.SetEnvironmentVariable(
            DefaultConnectionStringKey,
            _originalDefaultConnection);
        Environment.SetEnvironmentVariable(
            RedisConnectionStringKey,
            _originalRedisConnection);
    }
}
