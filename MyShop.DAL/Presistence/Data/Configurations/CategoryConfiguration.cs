using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class CategoryConfiguration : BaseEntityConfiguration<Category>, IEntityTypeConfiguration<Category>
    {
        public new void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(E => E.Name).HasColumnType("varchar(50)");
            base.Configure(builder);
        }
    }
}
