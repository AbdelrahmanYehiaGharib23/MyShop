using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Specifications
{
        public class ProductCountSpecification : BaseSpecification<Product>
        {
            public ProductCountSpecification(ProductSpecParams specParams)
                : base(p =>
                    string.IsNullOrWhiteSpace(specParams.Search)
                    || p.Name.ToLower().Contains(specParams.Search.ToLower())
                    || (p.Description != null && p.Description.ToLower().Contains(specParams.Search.ToLower())))
            {
            }
    }
}
