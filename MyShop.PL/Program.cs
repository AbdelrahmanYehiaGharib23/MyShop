using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using MyShop.BLL.Mapping;
using MyShop.BLL.Models.Common;
using MyShop.BLL.Services.AttachmentServices;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.CategoryServices;
using MyShop.BLL.Services.CurrentUserServices;
using MyShop.BLL.Services.IdentityServices;
using MyShop.BLL.Services.OrderServices;
using MyShop.BLL.Services.PaymentServices;
using MyShop.BLL.Services.ProductServices;
using MyShop.BLL.Services.ReviewServices;
using MyShop.DAL;
using MyShop.DAL.Entities.IdentityEntity;
using MyShop.DAL.Presistence.Data.DbInitializer;
using MyShop.PL.Extensions;
using MyShop.PL.Mapping;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Loopback);
    options.KnownProxies.Add(IPAddress.IPv6Loopback);
});

builder.Services.AddPersistenceServices(builder.Configuration);

// Configure AutoMapper
builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<PresentationProfile>());

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4);
    options.User.RequireUniqueEmail = true;
})
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Register application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAttachmentServices, AttachmentService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPasswordResetManager, PasswordResetManager>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Stripe
builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddHttpContextAccessor();

//Paymob
builder.Services.Configure<PaymobSettings>(
    builder.Configuration.GetSection("Paymob"));

builder.Services.AddHttpClient<IPaymobPaymentGateway,
    PaymobPaymentGateway>();

// Redis - Upstash
var redisConnectionString =
    builder.Configuration.GetConnectionString("Redis");

if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    throw new InvalidOperationException(
        "Redis connection string is missing.");
}

var redisUri = new Uri(redisConnectionString);

var userInfo = redisUri.UserInfo.Split(':', 2);

var redisOptions = new ConfigurationOptions
{
    Ssl = true,
    SslHost = redisUri.Host,
    AbortOnConnectFail = false,
    User = userInfo[0],
    Password = Uri.UnescapeDataString(userInfo[1])
};

redisOptions.EndPoints.Add(redisUri.Host, 6379);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = redisOptions;
    options.InstanceName = "MyShop:";
});

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HybridCache
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});

var app = builder.Build();

app.InitializeDatabase();

// Configure the HTTP request pipeline
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");

app.Run();
