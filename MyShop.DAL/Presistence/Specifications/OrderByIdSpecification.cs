namespace MyShop.DAL.Presistence.Specifications
{
    public sealed class OrderByIdSpecification : BaseSpecification<Order>
    {
        public OrderByIdSpecification(int orderId)
            : base(order => order.Id == orderId)
        {
            AddIncludeChain(query => query.Include(order => order.OrderItems));
        }
    }
}
