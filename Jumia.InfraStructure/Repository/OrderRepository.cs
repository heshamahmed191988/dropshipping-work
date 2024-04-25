using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.Dtos.ViewModel.Order;
//using System.Data.Entity;

//using System.Data.Entity;

namespace Jumia.InfraStructure.Repository
{
    public class OrderRepository : Repository<Order, int>, IOrderReposatory
    {
        private readonly JumiaContext context;

        public OrderRepository(JumiaContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            // Fetch orders with related users and addresses eagerly loaded
            var orders = await context.orders
                                      .Include(o => o.User)
                                          .ThenInclude(u => u.Addresses) // Include addresses related to the user
                                      .ToListAsync();

            return orders;
        }

        //public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        //{
        //    var order = await context.orders.FindAsync(orderId);
        //    if (order != null)
        //    {
        //        order.Status = newStatus;
        //        await context.SaveChangesAsync();
        //    }
        //}

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            // First, fetch the order to update its status
            var order = await context.orders.FindAsync(orderId);
            if (order == null) return; // Order not found, exit early

            order.Status = newStatus;

            // Optionally, here you could access order.AddressId or order.Address if you need to check or log the address details
            // For example, logging the address ID or verifying the address exists if business logic requires such validation

            // Then, fetch the related order items (OrderProducts) and their products to calculate the total price
            var orderItems = await context.orderProducts
                .Where(op => op.OrderId == orderId && !op.IsDeleted) // Assuming there's an IsDeleted flag
                .Include(op => op.Product) // Include product to access price
                .ToListAsync();

            decimal newTotalPrice = 0;
            foreach (var item in orderItems)
            {
                // Calculate total price for each item and sum them up
                newTotalPrice += item.Quantity * item.Product.Price;
            }

            // Update the total price of the order
            order.TotalPrice = newTotalPrice;

            // Save changes
            await context.SaveChangesAsync();
        }


        public async Task<List<OrderProduct>> GetByOrderIdAsync(int orderId)
        {
            // Include the Order to access its AddressId or Address navigation property
            return await context.orderProducts
                .Where(op => op.OrderId == orderId && !op.IsDeleted)
                .Include(op => op.Product)
                .Include(op => op.Order) // Include Order to access Address
                .ThenInclude(order => order.OrderAddresses) // Include Address from Order
                .ToListAsync();
        }













        public Task<IQueryable<OrderDetailsDTO>> GetOrderDetailsByordrId(int orderid)
        {
            var ordersDto = from order in context.orders
                            join orderdetails in context.orderProducts.Where(p => p.IsDeleted == false) on order.Id equals orderid
                            join product in context.products on orderdetails.ProductId equals product.Id
                            where order.Id == orderdetails.OrderId
                            select new OrderDetailsDTO
                            {
                                Quantity = orderdetails.Quantity,
                                UserID = order.UserID,
                                productname = product.NameEn,
                                TotalPrice = orderdetails.TotalPrice,
                                DatePlaced = order.DatePlaced,
                                Status = order.Status,
                                orderitemid = orderdetails.Id,
                                orderid = order.Id,
                                productid = product.Id,
                                ProductDescription = product.DescriptionEn,
                                ProductImage = product.ProductImages.Select(p => p.Path).FirstOrDefault() ?? "null",
                                ProductPrice = product.Price
                            };

            return Task.FromResult(ordersDto);
        }























        //public async Task DeleteOrderAsync(int Orderitemid)
        //{
        //    var order = await context.orders.FindAsync(Orderitemid);
        //    if (order != null)
        //    {
        //        context.orders.Remove(order);
        //        await context.SaveChangesAsync();
        //    }
        //}
        public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(string userId)
        {
            var ordersDto = await context.orders
           .Where(o => o.UserID == userId && o.IsDeleted == false)
           .Select(o => new OrderDto
           {
               Id = o.Id,
               UserID = o.UserID,
               DatePlaced = o.DatePlaced,
               TotalPrice = o.TotalPrice,
               Status = o.Status,
               BarcodeImageUrl = o.BarcodeImageUrl,
           })
           .ToListAsync();

            return ordersDto;
        }


        public async Task<Address> GetAddressByIdAsync(int addressId)
        {
            // Retrieve the address by its ID asynchronously
            var address = await context.addresses.FirstOrDefaultAsync(a => a.Id == addressId);
            return address;
        }
        //public Task<IQueryable<OrderDetailsDTO>> GetOrderDetailsByordrId(int orderid)
        //{
        //    var ordersDto = from order in context.orders
        //                    join orderdetails in context.orderProducts.Where(p => p.IsDeleted == false) on order.Id equals orderid
        //                    join product in context.products on orderdetails.ProductId equals product.Id
        //                    where order.Id == orderdetails.OrderId
        //                    select new OrderDetailsDTO
        //                    {
        //                        Quantity = orderdetails.Quantity,
        //                        UserID = order.UserID,
        //                        productname = product.NameEn,
        //                        TotalPrice = orderdetails.TotalPrice,
        //                        DatePlaced = order.DatePlaced,
        //                        Status = order.Status,
        //                        orderitemid = orderdetails.Id,
        //                        orderid = order.Id,
        //                        productid = product.Id
        //                    };

        //    return Task.FromResult(ordersDto);
        //}

    }

}

