using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {

        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Name)
                   .HasColumnType("varchar(50)");

            builder.Property(e => e.Price)
                   .HasPrecision(18, 2);
        }
    }
}
