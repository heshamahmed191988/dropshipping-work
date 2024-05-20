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
using Jumia.model;
using Jumia.Dtos;
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
                                         .Include(o => o.Address)
                                      .ToListAsync();

            return orders;
        }

        public async Task<List<Order>> GetAllOrdersWithAddressesAsync()
        {
            return await context.orders
                .Include(o => o.Address)
                .Include(o => o.User)
                .Include(o => o.Products) // Include the products related to the order
           .ThenInclude(op => op.Product) // Include the product details
       .ToListAsync();
        }


        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await context.orders
                .Include(o => o.Address)
                .Include(o => o.User)
                .Include(o => o.Products)
                    .ThenInclude(op => op.Product)
                .Where(o => o.DatePlaced >= startDate && o.DatePlaced <= endDate)
                .ToListAsync();
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
                            join orderdetails in context.orderProducts.Where(p => p.IsDeleted == false) on order.Id equals orderdetails.OrderId
                            join product in context.products on orderdetails.ProductId equals product.Id
                            join orderAddress in context.orderAddresses on order.Id equals orderAddress.OrderId
                            join address in context.addresses on orderAddress.AddressId equals address.Id
                            where order.Id == orderid
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
                                ProductPrice = product.Price,
                                SelectedPrice = orderdetails.SelectedPrice,
                                Street = address.Street,
                                City = address.City,
                                State = address.State,
                                ZipCode = address.ZipCode
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
                .Select(o => new
                {
                    o.Id,
                    o.UserID,
                    o.DatePlaced,
                    o.TotalPrice,
                    o.Status,
                    //o.BarcodeImageUrl,
                    Address = context.orderAddresses
                        .Where(oa => oa.OrderId == o.Id)
                        .Select(oa => new
                        {
                            oa.AddressId,
                            oa.Address.Street,
                            oa.Address.City,
                            oa.Address.State,
                            oa.Address.ZipCode
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return ordersDto.Select(o => new OrderDto
            {
                Id = o.Id,
                UserID = o.UserID,
                DatePlaced = o.DatePlaced, // ISO format for string representation
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                //BarcodeImageUrl = o.BarcodeImageUrl,
                AddressId = o.Address?.AddressId,
                Street = o.Address?.Street,
                City = o.Address?.City,
                State = o.Address?.State,
                ZipCode = o.Address?.ZipCode
            }).ToList();
        }





        public async Task<Address> GetAddressByIdAsync(int addressId)
        {
            // Retrieve the address by its ID asynchronously
            var address = await context.addresses.FirstOrDefaultAsync(a => a.Id == addressId);
            return address;
        }


        public async Task<int> GetTotalOrdersCountAsync()
        {
            try
            {
                // Query the Orders DbSet and count the total number of orders
                var totalOrdersCount = await context.orders.CountAsync();
                return totalOrdersCount;
            }
            catch (Exception ex)
            {
                // Handle any potential exceptions here
                // Log or throw as needed
                throw new Exception("Failed to retrieve total orders count: " + ex.Message, ex);
            }
        }


        public async Task<List<Order>> SearchOrdersByIdAsync(int orderId)
        {
            return await context.orders
                .Where(o => o.Id == orderId)
                .ToListAsync();
        }


        public async Task<bool> UpdateOrderStatusAsync2(int orderId, string newStatus)
        {
            var order = await context.orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }



        public async Task<Order> CreateOrder(Order order)
        {
            context.orders.Add(order);
            await context.SaveChangesAsync();
            return order;
        }
        public async Task<Address> CreateAddress(Address address)
        {
            context.addresses.Add(address);
            await context.SaveChangesAsync();
            return address;
        }
        public async Task<OrderAddress> CreateOrderAddress(OrderAddress orderAddress)
        {
            context.orderAddresses.Add(orderAddress);
            await context.SaveChangesAsync();
            return orderAddress;
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await context.orders.FindAsync(orderId);
            if (order == null || order.Status != "Pending")
            {
                return false;
            }

            var user = await context.Users.FindAsync(order.UserID);
            if (user == null || user.Earning < order.Totalearning)
            {
                return false; // Prevent deletion if user earnings are less than order total earnings
            }

            // Delete related order addresses
            var orderAddresses = context.orderAddresses.Where(oa => oa.OrderId == orderId);
            context.orderAddresses.RemoveRange(orderAddresses);

            // Delete related payments
            var payments = context.payments.Where(p => p.orderID == orderId);
            context.payments.RemoveRange(payments);

            // Delete the order
            context.orders.Remove(order);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DecreaseUserEarningsAsync(string userId, decimal? totalEarning)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Earning -= totalEarning;
            return await context.SaveChangesAsync() > 0;
        }
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



