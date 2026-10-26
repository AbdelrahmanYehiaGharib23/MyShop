using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Models.Dto.PaymentDto;

namespace MyShop.BLL.Services.PaymentServices
{
    public interface IPaymentGateway
    {
        Task<OrderPaymentResultDto> CreatePaymentIntentAsync(
           int orderId,
           decimal amount,
           string currency);

        Task<OrderPaymentResultDto> UpdatePaymentIntentAsync(
            string paymentIntentId,
            decimal amount,
            string currency);

        Task<bool> IsPaymentIntentSucceededAsync(
            string paymentIntentId,
            int orderId,
            decimal amount,
            string currency);
    }
}
