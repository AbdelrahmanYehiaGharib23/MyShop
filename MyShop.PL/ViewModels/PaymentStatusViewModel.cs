using MyShop.DAL.Entities.Enum;

namespace MyShop.PL.ViewModels
{
    public class PaymentStatusViewModel
    {
        public int OrderId { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public int ItemCount { get; set; }

        public OrderStatus Status { get; set; }
    }
}
