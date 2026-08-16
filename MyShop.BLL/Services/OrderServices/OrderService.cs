using Microsoft.Extensions.Logging;
using MyShop.BLL.Models.Dto.OrderDto;
using MyShop.BLL.Services.CartServices;
using MyShop.BLL.Services.CurrentUserServices;
using MyShop.BLL.Services.IdentityServices;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;
using MyShop.DAL.Presistence.Specifications;

namespace MyShop.BLL.Services.OrderServices
{
    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OrderService> _logger;
        private readonly IEmailTemplateService _emailTemplateService;

        public OrderService(IUnitOfWork unitOfWork, ICartService cartService,ICurrentUserService currentUserService,IEmailSender emailSender,ILogger<OrderService> logger,IEmailTemplateService emailTemplateService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _currentUserService = currentUserService;
            _emailSender = emailSender;
            _logger = logger;
            _emailTemplateService = emailTemplateService;
        }

        public async Task CreateOrderAsync(CreateOrderDto dto)
        {
            var cartItems = _cartService.GetCart();

            if (cartItems == null || !cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var userId = _currentUserService.GetCurrentUserId();
            var userEmail = _currentUserService.GetCurrentUserEmail();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (string.IsNullOrWhiteSpace(userEmail))
                throw new InvalidOperationException("User email is not available.");

            var order = new Order
            {
                UserId = userId,
                Address = dto.Address,
                Phone = dto.Phone,
                TotalPrice = _cartService.GetTotalPrice(),
                OrderDate = DateTime.UtcNow
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

            var emailBody = _emailTemplateService.OrderConfirmation(
                order.Id,
                order.TotalPrice,
                order.Address,
                order.Phone);

            try
            {
                await _emailSender.SendEmailAsync(
                    userEmail,
                    "Order Confirmation - MyShop",
                    emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send order confirmation email for Order {OrderId} to {Email}",
                    order.Id,
                    userEmail);
            }
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync()
        {
            var userId = _currentUserService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var spec = new OrderSpecification(userId);

            var orders = await _unitOfWork.OrderRepository
                .GetAllWithSpecAsync(spec);

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                Address = order.Address,
                Phone = order.Phone,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,

                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,

                    ProductName = !string.IsNullOrWhiteSpace(item.ProductName)
                        ? item.ProductName
                        : item.Product?.Name ?? $"Product #{item.ProductId}",

                    ProductImageUrl = item.ProductImageUrl ?? item.Product?.ImageUrl,

                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            });
        }
    }
}
