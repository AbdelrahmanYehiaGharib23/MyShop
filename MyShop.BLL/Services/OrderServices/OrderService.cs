using Microsoft.Extensions.Logging;
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
            var cartItems = _cartService.GetCart();

            if (cartItems is null || !cartItems.Any())
                throw new InvalidOperationException(
                    "Cart is empty.");

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

            var order = new Order
            {
                UserId = userId,
                CustomerEmail = userEmail,
                Address = dto.Address,
                Phone = dto.Phone,
                TotalPrice = _cartService.GetTotalPrice(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImageUrl = item.ImageUrl,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            _unitOfWork.OrderRepository.Add(order);

            await _unitOfWork.CompleteAsync();

            return order.Id;
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
    }
}
