using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyShop.DAL.Entities;

namespace MyShop.DAL.Presistence.Data.Configurations;

public sealed class FavoriteProductConfiguration : BaseEntityConfiguration<FavoriteProduct>
{
    public override void Configure(EntityTypeBuilder<FavoriteProduct> builder)
    {
        base.Configure(builder);

        builder.Property(favorite => favorite.UserId).HasMaxLength(450).IsRequired();
        builder.HasIndex(favorite => new { favorite.UserId, favorite.ProductId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(favorite => favorite.User)
            .WithMany()
            .HasForeignKey(favorite => favorite.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(favorite => favorite.Product)
            .WithMany()
            .HasForeignKey(favorite => favorite.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
