using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Specifications
{
    public class ReviewSpecification : BaseSpecification<Review>
    {
        public ReviewSpecification(string userId, int productId) : base(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted)
        {

        }
    }
}
