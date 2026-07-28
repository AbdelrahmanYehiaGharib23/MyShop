
namespace MyShop.DAL.Presistence.Specifications
{
    public class ProductWithCategorySpecification : BaseSpecification<Product>
    {
        public ProductWithCategorySpecification()
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Name);
        }
    }
}
