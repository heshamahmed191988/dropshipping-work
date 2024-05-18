using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Jumia.Dtos.ViewModel.Order;
using Jumia.model;


namespace Jumia.Application.Contract
{
    public interface IOrderReposatory : IRepository<Order, int>
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        //Task DeleteOrderAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserId(string userId);
        Task<IQueryable<OrderDetailsDTO>> GetOrderDetailsByordrId(int orderid);
        Task<List<OrderProduct>> GetByOrderIdAsync(int orderId);
        Task<Address> GetAddressByIdAsync(int addressId);
        Task<int> GetTotalOrdersCountAsync();

        Task<List<Order>> SearchOrdersByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync2(int orderId, string newStatus);

        Task<Order> CreateOrder(Order order);

        Task<Address> CreateAddress(Address address);

        Task<OrderAddress> CreateOrderAddress(OrderAddress orderAddress);
        Task<List<Order>> GetAllOrdersWithAddressesAsync();
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> DecreaseUserEarningsAsync(string userId, decimal? totalEarning);



    }
}