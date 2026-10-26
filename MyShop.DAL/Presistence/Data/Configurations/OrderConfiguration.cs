using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class OrderConfiguration:IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(o => o.Address)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(o => o.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.CustomerEmail)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(o => o.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
