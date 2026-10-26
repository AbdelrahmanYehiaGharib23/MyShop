using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Models.Dto.PaymentDto;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities.Enum;

namespace MyShop.BLL.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IPaymentGateway paymentGateway,
            IUnitOfWork unitOfWork)
        {
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderPaymentResultDto>
            CreateOrUpdatePaymentIntentAsync(
                int orderId,
                decimal amount,
                string currency = "egp")
        {
            var order =
                await _unitOfWork.OrderRepository
                    .GetByIdAsync(orderId);

            if (order is null)
                throw new KeyNotFoundException(
                    "Order not found.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException(
                    "Payment is not available for this order.");

            if (amount <= 0)
                throw new InvalidOperationException(
                    "Invalid payment amount.");

            if (string.IsNullOrWhiteSpace(
                order.PaymentIntentId))
            {
                var result =
                    await _paymentGateway.CreatePaymentIntentAsync(
                        order.Id,
                        amount,
                        currency);

                order.PaymentIntentId =
                    result.PaymentIntentId;

                await _unitOfWork.CompleteAsync();

                return result;
            }

            var updatedResult = await _paymentGateway
                .UpdatePaymentIntentAsync(
                    order.PaymentIntentId,
                    amount,
                    currency);

            if (order.PaymentIntentId != updatedResult.PaymentIntentId)
            {
                order.PaymentIntentId = updatedResult.PaymentIntentId;
                await _unitOfWork.CompleteAsync();
            }

            return updatedResult;
        }

        public async Task<bool> ConfirmPaymentSucceededAsync(
            int orderId,
            string paymentIntentId,
            decimal amount,
            string currency = "egp")
        {
            if (orderId <= 0 ||
                string.IsNullOrWhiteSpace(paymentIntentId) ||
                amount <= 0)
                return false;

            var order =
                await _unitOfWork.OrderRepository
                    .GetByIdAsync(orderId);

            if (order is null ||
                order.Status != OrderStatus.Pending ||
                order.PaymentIntentId != paymentIntentId)
                return false;

            return await _paymentGateway
                .IsPaymentIntentSucceededAsync(
                    paymentIntentId,
                    order.Id,
                    amount,
                    currency);
        }
    }
}
