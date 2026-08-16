using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Specifications
{
    public class ProductReviewsSpecification : BaseSpecification<Review>
    {
        public ProductReviewsSpecification(int productId): base(r => r.ProductId == productId && !r.IsDeleted)
        {
            AddOrderByDescending(r => r.CreatedOn ?? DateTime.MinValue);
        }
    }
}
