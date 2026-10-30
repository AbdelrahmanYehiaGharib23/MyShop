using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.CurrentUserServices;
using MyShop.BLL.Services.IdentityServices;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;
using MyShop.DAL.Entities.Enum;
using MyShop.DAL.Presistence.Specifications;

namespace MyShop.BLL.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OrderService> _logger;
        private readonly IEmailTemplateService _emailTemplateService;

        public OrderService(
            IUnitOfWork unitOfWork,
            ICartService cartService,
            ICurrentUserService currentUserService,
            IEmailSender emailSender,
            ILogger<OrderService> logger,
            IEmailTemplateService emailTemplateService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _currentUserService = currentUserService;
            _emailSender = emailSender;
            _logger = logger;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<int> CreatePendingOrderAsync(
            CreateOrderDto dto)
        {
            var userId =
                _currentUserService.GetCurrentUserId();

            var userEmail =
                _currentUserService.GetCurrentUserEmail();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            if (string.IsNullOrWhiteSpace(userEmail))
                throw new InvalidOperationException(
                    "User email is not available.");

            var checkoutRequestId = dto.CheckoutRequestId?.Trim();
            if (checkoutRequestId is null
                || !Guid.TryParseExact(checkoutRequestId, "N", out _))
                throw new InvalidOperationException("The checkout session expired. Return to checkout and try again.");

            var existingOrder = (await _unitOfWork.OrderRepository.FindAsync(
                    order => order.UserId == userId
                        && order.CheckoutRequestId == checkoutRequestId))
                .FirstOrDefault();

            if (existingOrder is not null)
                return existingOrder.Id;

            var cartItems = _cartService.GetCart();
            if (cartItems.Count == 0)
                throw new InvalidOperationException("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                CustomerEmail = userEmail,
                Address = dto.Address,
                Phone = dto.Phone,
                TotalPrice = 0,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                CheckoutRequestId = checkoutRequestId
            };

            foreach (var item in cartItems)
            {
                if (item.ProductId <= 0 || item.Quantity <= 0)
                    throw new InvalidOperationException("Your cart has an invalid item. Review it and try again.");

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product is null)
                    throw new InvalidOperationException("A product in your cart is no longer available. Review your cart.");

                order.TotalPrice += product.Price * item.Quantity;
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            _unitOfWork.OrderRepository.Add(order);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException)
            {
                // If two retries reach the server together, the unique key makes
                // the later request return the first order instead of duplicating it.
                var concurrentOrder = (await _unitOfWork.OrderRepository.FindAsync(
                        existing => existing.UserId == userId
                            && existing.CheckoutRequestId == checkoutRequestId))
                    .FirstOrDefault();

                if (concurrentOrder is not null)
                    return concurrentOrder.Id;

                throw;
            }

            return order.Id;
        }

        public async Task<Order?> GetOrderForCustomerAsync(int orderId)
        {
            var userId = _currentUserService.GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var order = await _unitOfWork.OrderRepository
                .GetEntityWithSpecAsync(new OrderByIdSpecification(orderId));

            return order?.UserId == userId ? order : null;
        }

        public async Task<Order?> GetOrderForPaymentAsync(
            int orderId)
        {
            var userId =
                _currentUserService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var order =
                await _unitOfWork.OrderRepository
                    .GetEntityWithSpecAsync(
                        new OrderByIdSpecification(orderId));

            if (order is null)
                return null;

            if (order.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You cannot access this order.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException(
                    "This order is not pending.");

            return order;
        }
        public async Task<Order?> GetOrderForPaymobCallbackAsync(int orderId)
        {
            return await _unitOfWork.OrderRepository
                .GetEntityWithSpecAsync(new OrderByIdSpecification(orderId));
        }

        public async Task<Order?>
            GetOrderByPaymentIntentIdAsync(
                string paymentIntentId)
        {
            var orders =
                await _unitOfWork.OrderRepository.FindAsync(
                    order =>
                        order.PaymentIntentId ==
                        paymentIntentId);

            return orders.FirstOrDefault();
        }

        public async Task MarkAsPaidAsync(int orderId)
        {
            var order =
                await _unitOfWork.OrderRepository
                    .GetByIdAsync(orderId);

            if (order is null)
                return;

            if (order.Status == OrderStatus.Paid)
                return;

            order.Status = OrderStatus.Paid;

            await _unitOfWork.CompleteAsync();

            try
            {
                var emailBody =
                    _emailTemplateService.OrderConfirmation(
                        order.Id,
                        order.TotalPrice,
                        order.Address,
                        order.Phone);

                await _emailSender.SendEmailAsync(
                    order.CustomerEmail,
                    "Order Confirmation - MyShop",
                    emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send confirmation email for Order {OrderId} to {Email}",
                    order.Id,
                    order.CustomerEmail);
            }
        }

        public async Task MarkAsFailedAsync(int orderId)
        {
            var order =
                await _unitOfWork.OrderRepository
                    .GetByIdAsync(orderId);

            if (order is null)
                return;

            if (order.Status == OrderStatus.Paid)
                return;

            order.Status = OrderStatus.Failed;

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<AdminOrderDto>> GetOrdersForAdminAsync()
        {
            var orders = await _unitOfWork.OrderRepository
                .GetAllWithSpecAsync(new AdminOrderSpecification());

            return orders.Select(MapAdminOrder);
        }

        public async Task<AdminOrderDto?> GetOrderForAdminAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository
                .GetEntityWithSpecAsync(new AdminOrderSpecification(orderId));

            return order is null ? null : MapAdminOrder(order);
        }

        public async Task UpdateOrderFulfillmentAsync(
            int orderId,
            OrderFulfillmentStatus fulfillmentStatus,
            string? courierName,
            string? trackingNumber,
            DateOnly? estimatedDeliveryDate)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

            if (order is null)
                throw new KeyNotFoundException($"Order {orderId} was not found.");

            if (order.Status != OrderStatus.Paid)
                throw new InvalidOperationException("Only paid orders can enter fulfillment.");

            if (!Enum.IsDefined(fulfillmentStatus))
                throw new InvalidOperationException("The selected fulfillment stage is invalid.");

            var currentStage = (int)order.FulfillmentStatus;
            var requestedStage = (int)fulfillmentStatus;

            if (requestedStage < currentStage || requestedStage > currentStage + 1)
                throw new InvalidOperationException("Move the order one fulfillment stage at a time.");

            if ((fulfillmentStatus is OrderFulfillmentStatus.Shipped or OrderFulfillmentStatus.Delivered)
                && (string.IsNullOrWhiteSpace(courierName)
                    || string.IsNullOrWhiteSpace(trackingNumber)))
            {
                throw new InvalidOperationException(
                    "Enter the courier and tracking number before shipping the order.");
            }

            if (courierName?.Length > 120 || trackingNumber?.Length > 120)
                throw new InvalidOperationException("Courier and tracking details must be 120 characters or fewer.");

            order.FulfillmentStatus = fulfillmentStatus;
            order.CourierName = NormalizeOptionalText(courierName);
            order.TrackingNumber = NormalizeOptionalText(trackingNumber);
            order.EstimatedDeliveryDate = estimatedDeliveryDate;

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<OrderDto>>
            GetMyOrdersAsync()
        {
            var userId =
                _currentUserService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var spec =
                new OrderSpecification(userId);

            var orders =
                await _unitOfWork.OrderRepository
                    .GetAllWithSpecAsync(spec);

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                Address = order.Address,
                Phone = order.Phone,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status,
                FulfillmentStatus = order.FulfillmentStatus,
                CourierName = order.CourierName,
                TrackingNumber = order.TrackingNumber,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                PaymentIntentId = order.PaymentIntentId,

                Items = order.OrderItems.Select(item =>
                    new OrderItemDto
                    {
                        ProductId = item.ProductId,

                        ProductName =
                            !string.IsNullOrWhiteSpace(
                                item.ProductName)
                                ? item.ProductName
                                : item.Product?.Name
                                  ?? $"Product #{item.ProductId}",

                        ProductImageUrl =
                            item.ProductImageUrl
                            ?? item.Product?.ImageUrl,

                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList()
            });
        }

        private static AdminOrderDto MapAdminOrder(Order order)
        {
            return new AdminOrderDto
            {
                Id = order.Id,
                CustomerEmail = order.CustomerEmail,
                Address = order.Address,
                Phone = order.Phone,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status,
                FulfillmentStatus = order.FulfillmentStatus,
                CourierName = order.CourierName,
                TrackingNumber = order.TrackingNumber,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImageUrl = item.ProductImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

        private static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
