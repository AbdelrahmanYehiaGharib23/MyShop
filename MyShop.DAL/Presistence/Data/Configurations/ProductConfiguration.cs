using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>, IEntityTypeConfiguration<Product>
    {

        public new void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(E => E.Name).HasColumnType("varchar(50)");
            builder.Property(E => E.Price).HasColumnType("decimal(18,2)");
            base.Configure(builder);
        }
    }
}
