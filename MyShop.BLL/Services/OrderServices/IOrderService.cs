using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.OrderDto;

namespace MyShop.BLL.Services.OrderServices
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync();
    }
}
