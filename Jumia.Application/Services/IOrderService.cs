using Jumia.Dtos.ResultView;
using Jumia.Dtos.ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task DeleteOrderAsync(int orderId);


        Task<ResultView<OrderProducutDTo>> UpdateOrderProductAsync(UpdateOrderProductDto updateOrderProduct);
        Task<IEnumerable<OrderDto>> GetOrdersByUserId(string userId);
       // Task<ResultView<OrderDto>> CreateOrderAsync(List<OrderQuantity> ProdactID, String UserID);
        Task<IQueryable<OrderDetailsDTO>> GetOrderDetailsByorderId(int orderid);

        Task<ResultView<OrderDto>> CreateOrderAsync(List<OrderQuantity> ProductIDs, string UserID, int AddressId);


    }
}