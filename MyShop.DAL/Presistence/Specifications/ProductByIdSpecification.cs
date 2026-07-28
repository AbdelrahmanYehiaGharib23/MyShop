

namespace MyShop.DAL.Presistence.Specifications
{
    public class ProductByIdSpecification:BaseSpecification<Product>
    {
        public ProductByIdSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Category);
        }
    }
}
