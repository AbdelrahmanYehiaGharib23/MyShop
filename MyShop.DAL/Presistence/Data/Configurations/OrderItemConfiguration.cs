using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class OrderItemConfiguration:IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.ProductName)
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(oi => oi.ProductImageUrl)
                .HasMaxLength(500);

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(oi => !oi.IsDeleted);
        }
    }
}
