using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.PaymentDto;

namespace MyShop.BLL.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<PaymentIntentResultDto> CreatePaymentIntentAsync(
            decimal amount,
            string currency = "usd");

        Task<bool> IsPaymentIntentSucceededAsync(
            string paymentIntentId,
            decimal expectedAmount,
            string currency = "usd");
    }
}
