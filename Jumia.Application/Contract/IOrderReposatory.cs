using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Jumia.Dtos.ViewModel.Order;


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
    }
}