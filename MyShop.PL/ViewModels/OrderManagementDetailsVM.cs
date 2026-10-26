using System.ComponentModel.DataAnnotations;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.DAL.Entities.Enum;

namespace MyShop.PL.ViewModels
{
    public sealed class OrderManagementDetailsVM
    {
        public AdminOrderDto Order { get; set; } = new();

        [Display(Name = "Fulfillment stage")]
        public OrderFulfillmentStatus FulfillmentStatus { get; set; }

        [StringLength(120)]
        [Display(Name = "Courier / carrier")]
        public string? CourierName { get; set; }

        [StringLength(120)]
        [Display(Name = "Tracking number")]
        public string? TrackingNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Estimated delivery date")]
        public DateOnly? EstimatedDeliveryDate { get; set; }
    }
}
