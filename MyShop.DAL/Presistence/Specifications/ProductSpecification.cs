using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        // List (Search + Sorting + Pagination)
        public ProductSpecification(ProductSpecParams specParams)
            : base(p =>
        string.IsNullOrWhiteSpace(specParams.Search)
        || p.Name.ToLower().Contains(specParams.Search.ToLower())
        || (p.Description != null && p.Description.ToLower().Contains(specParams.Search.ToLower())))
        { 
         // Include Category
            AddInclude(p => p.Category);

            // Sorting
            switch (specParams.Sort?.ToLower())
            {
                case "priceasc":
                    AddOrderBy(p => p.Price);
                    break;

                case "pricedesc":
                    AddOrderByDescending(p => p.Price);
                    break;

                case "nameasc":
                    AddOrderBy(p => p.Name);
                    break;

                case "namedesc":
                    AddOrderByDescending(p => p.Name);
                    break;

                default:
                    AddOrderBy(p => p.Name);
                    break;
            }

            // Pagination
            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize,specParams.PageSize);
        }
        
        // Details
        public ProductSpecification(int id)
            : base(p => p.Id == id)
        {
            AddInclude(p => p.Category);
        }

        public ProductSpecification()
        {
            AddInclude(p => p.Category);
        }
    }
}
