using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.CreatedOn)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.LastModifiedOn)
                   .HasDefaultValueSql("GETDATE()")
                   .ValueGeneratedOnUpdate();
        }
    }
}
