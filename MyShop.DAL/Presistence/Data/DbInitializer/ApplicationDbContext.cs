using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShop.DAL.Entities.IdentityEntity;

namespace MyShop.DAL.Presistence.Data.DbInitializer
{
    
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted);

            builder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<Review>()
                .HasQueryFilter(r => !r.IsDeleted && !r.Product.IsDeleted);

            builder.Entity<FavoriteProduct>()
                .HasQueryFilter(favorite => !favorite.IsDeleted && !favorite.Product.IsDeleted);

            builder.Entity<ApplicationUser>()
                        .HasIndex(u => u.NormalizedEmail)
                        .IsUnique();

            base.OnModelCreating(builder);
        }
        private string? GetCurrentUserId()
        {
            return _httpContextAccessor?
                .HttpContext?
                .User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
                .Value;
        }
        public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker
        .Entries<BaseEntity>()
        .ToList();

    var currentUserId = GetCurrentUserId() ?? "System";

    foreach (var entry in entries)
    {
        switch (entry.State)
        {
            case EntityState.Added:

                entry.Entity.CreatedOn = DateTime.UtcNow;
                entry.Entity.LastModifiedOn = DateTime.UtcNow;

                entry.Entity.CreatedBy = currentUserId;
                entry.Entity.LastModifiedBy = currentUserId;

                break;

            case EntityState.Modified:

                entry.Entity.LastModifiedOn = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = currentUserId;

                // Prevent changing Created information
                entry.Property(x => x.CreatedOn).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;

                break;

            case EntityState.Deleted:

                // Convert Hard Delete → Soft Delete
                entry.State = EntityState.Modified;

                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = currentUserId;

                entry.Entity.LastModifiedOn = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = currentUserId;

                // Prevent changing Created information
                entry.Property(x => x.CreatedOn).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;

                break;
        }
    }

    return await base.SaveChangesAsync(cancellationToken);
}

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public DbSet<PasswordResetTokens> PasswordResetTokens { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
    }
}

