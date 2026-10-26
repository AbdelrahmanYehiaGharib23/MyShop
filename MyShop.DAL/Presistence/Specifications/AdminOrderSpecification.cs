using MyShop.DAL.Entities;

namespace MyShop.DAL.Presistence.Specifications
{
    public sealed class AdminOrderSpecification : BaseSpecification<Order>
    {
        public AdminOrderSpecification(int? orderId = null)
            : base(orderId.HasValue
                ? order => order.Id == orderId.Value
                : order => true)
        {
            AddOrderByDescending(order => order.OrderDate);
            AddIncludeChain(query => query.Include(order => order.OrderItems));
        }
    }
}
