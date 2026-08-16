using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Data.Configurations
{
    public class ReviewConfiguration : BaseEntityConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
