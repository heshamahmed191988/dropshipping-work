using AutoMapper;
using Jumia.Context;
using System;
using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.Application.Contract;
using Jumia.Dtos.ViewModel.Order;
using Jumia.Dtos.ResultView;
using Jumia.Model;
using Jumia.Dtos;
using System.Data.Entity;

using Jumia.Dtos.ViewModel.category;
using Jumia.Dtos.ViewModel.Product;

namespace Jumia.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderReposatory _orderRepository;
        private readonly IProductService _productService;
        private readonly IOrderProuduct _orderProuduct;
        private readonly IProductReposatory _productReposatory;
        private readonly IAddressRepository addressRepository;
        private readonly IMapper _mapper;



        public OrderService(IOrderReposatory orderRepository, 
            IMapper mapper, IProductService productService, IOrderProuduct orderProuduct,
            IProductReposatory productReposatory,IAddressRepository addressRepository)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _orderProuduct = orderProuduct;
            _productReposatory = productReposatory;
            this.addressRepository = addressRepository;
            _mapper = mapper;

        }

        //public async Task<ResultView<OrderDto>> CreateOrderAsync(List<OrderQuantity> ProductIDs, string UserID, int AddressId)
        //{
        //    try
        //    {
        //        decimal totalPrice = 0;
        //        foreach (var orderProductDto in ProductIDs)
        //        {
        //            // Assuming unitAmount is now provided by the orderProductDto
        //            decimal productPrice = orderProductDto.unitAmount;
        //            if (productPrice >= 0)
        //            {
        //                totalPrice += productPrice * orderProductDto.quantity;

        //                // Optional: Fetch product to update stock quantity if necessary
        //                var product = await _productReposatory.GetByIdAsync(orderProductDto.productID);
        //                if (product != null)
        //                {
        //                    product.StockQuantity -= orderProductDto.quantity; // Ensure stock doesn't go below 0 in real scenarios
        //                    await _productReposatory.UpdateAsync(product);
        //                }
        //            }
        //        }
        //        await _productReposatory.SaveChangesAsync();

        //        // Verify the AddressId is valid
        //        var address = await addressRepository.GetByIdAsync(AddressId);
        //        if (address == null)
        //        {
        //            return new ResultView<OrderDto>
        //            {
        //                IsSuccess = false,
        //                Message = "Invalid address ID provided."
        //            };
        //        }

        //        var order = new Order
        //        {
        //            DatePlaced = DateTime.Now,
        //            TotalPrice = totalPrice,
        //            Status = "Pending",
        //            UserID = UserID,
        //            AddressId = AddressId ,

        //        };

        //        var createdOrder = await _orderRepository.CreateAsync(order);
        //        await _orderRepository.SaveChangesAsync();

        //        foreach (var id in ProductIDs)
        //        {
        //            await _orderProuduct.CreateAsync(new OrderProduct
        //            {
        //                ProductId = id.productID,
        //                OrderId = createdOrder.Id,
        //                TotalPrice = id.quantity * id.unitAmount, // Utilize the provided unitAmount
        //                Quantity = id.quantity
        //            });
        //        }
        //        await _orderProuduct.SaveChangesAsync();

        //        var createdOrderDto = _mapper.Map<OrderDto>(createdOrder);
        //        // Optionally add address information to the createdOrderDto here if your OrderDto includes address details

        //        return new ResultView<OrderDto>
        //        {
        //            IsSuccess = true,
        //            Message = "Order created successfully",
        //            Entity = createdOrderDto
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultView<OrderDto>
        //        {
        //            IsSuccess = false,
        //            Message = "Failed to create order: " + ex.Message
        //        };
        //    }
        //}

        public async Task<ResultView<OrderDto>> CreateOrderAsync(List<OrderQuantity> ProductIDs, string UserID, int AddressId)
        {
            try
            {
                decimal totalPrice = 0;
                foreach (var orderProductDto in ProductIDs)
                {
                    // Assuming unitAmount is now provided by the orderProductDto
                    decimal productPrice = orderProductDto.unitAmount;
                    if (productPrice >= 0)
                    {
                        totalPrice += productPrice * orderProductDto.quantity;

                        // Optional: Fetch product to update stock quantity if necessary
                        var product = await _productReposatory.GetByIdAsync(orderProductDto.productID);
                        if (product != null)
                        {
                            product.StockQuantity -= orderProductDto.quantity; // Ensure stock doesn't go below 0 in real scenarios
                            await _productReposatory.UpdateAsync(product);
                        }
                    }
                }
                await _productReposatory.SaveChangesAsync();

                // Verify the AddressId is valid
                var address = await _orderRepository.GetAddressByIdAsync(AddressId);
                if (address == null)
                {
                    return new ResultView<OrderDto>
                    {
                        IsSuccess = false,
                        Message = "Invalid address ID provided."
                    };
                }

                var order = new Order
                {
                    DatePlaced = DateTime.Now,
                    TotalPrice = totalPrice,
                    Status = "Pending",
                    UserID = UserID,
                    AddressId = AddressId,
                };

                // Generate unique barcode for the order
                var barcode = BarcodeGenerator.GenerateUniqueBarcode();
                order.BarcodeImageUrl = barcode;

                var createdOrder = await _orderRepository.CreateAsync(order);
                await _orderRepository.SaveChangesAsync();

                foreach (var id in ProductIDs)
                {
                    await _orderProuduct.CreateAsync(new OrderProduct
                    {
                        ProductId = id.productID,
                        OrderId = createdOrder.Id,
                        TotalPrice = id.quantity * id.unitAmount, // Utilize the provided unitAmount
                        Quantity = id.quantity
                    });
                }
                await _orderProuduct.SaveChangesAsync();

                var createdOrderDto = _mapper.Map<OrderDto>(createdOrder);
                // Optionally add address information to the createdOrderDto here if your OrderDto includes address details

                return new ResultView<OrderDto>
                {
                    IsSuccess = true,
                    Message = "Order created successfully",
                    Entity = createdOrderDto
                };
            }
            catch (Exception ex)
            {
                return new ResultView<OrderDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create order: " + ex.Message
                };
            }
        }

        public static class BarcodeGenerator
        {
            // Generate a unique barcode using a combination of current timestamp and a random number
            public static string GenerateUniqueBarcode()
            {
                // Get the current timestamp in ticks and convert it to a string
                string timestamp = DateTime.UtcNow.Ticks.ToString();

                // Generate a random number between 1000 and 9999
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                // Combine timestamp and random number to create a unique barcode
                string barcode = timestamp + randomNumber.ToString();

                return barcode;
            }}
            public async Task<ResultView<OrderProducutDTo>> UpdateOrderProductAsync(UpdateOrderProductDto updateOrderProduct)
        {
            try
            {
                var orderProduct = await _orderProuduct.GetByIdAsync(updateOrderProduct.OrderItemId);
                if (orderProduct == null)
                {
                    return new ResultView<OrderProducutDTo>
                    {
                        IsSuccess = false,
                        Message = $"Order product with ID {updateOrderProduct.OrderItemId} not found."
                    };
                }

                var product = await _productReposatory.GetByIdAsync(updateOrderProduct.ProductId);
                if (product == null)
                {
                    return new ResultView<OrderProducutDTo>
                    {
                        IsSuccess = false,
                        Message = $"Product with ID {updateOrderProduct.ProductId} not found."
                    };
                }

                // Adjust stock quantity based on the difference in ordered quantity
                if (orderProduct.Quantity > updateOrderProduct.Quantity)
                {
                    product.StockQuantity += (orderProduct.Quantity - updateOrderProduct.Quantity);
                }
                else
                {
                    var diff = (updateOrderProduct.Quantity - orderProduct.Quantity);
                    product.StockQuantity -= diff;
                }

                // Calculate and update the total price for the order product
                orderProduct.Quantity = updateOrderProduct.Quantity;
                orderProduct.TotalPrice = updateOrderProduct.Quantity * product.Price;

                // Proceed to update the order's total price
                var order = await _orderRepository.GetByIdAsync(orderProduct.OrderId);
                if (order != null)
                {
                    // Recalculate the total price for the entire order
                    var orderProducts = await _orderRepository.GetByOrderIdAsync(orderProduct.OrderId);
                    order.TotalPrice = orderProducts.Sum(op => op.TotalPrice);

                    // Save the updated order
                    await _orderRepository.UpdateAsync(order);
                    // Assuming your repository pattern includes a method for saving changes; if not, use your context directly
                    await _orderRepository.SaveChangesAsync();
                }

                // Save changes for the order product and product stock quantity update
                await _orderProuduct.UpdateAsync(orderProduct);
                await _orderProuduct.SaveChangesAsync();
                await _productReposatory.UpdateAsync(product);
                await _productReposatory.SaveChangesAsync();

                // Map the updated order product to DTO
                var updatedOrderProductDto = _mapper.Map<OrderProducutDTo>(orderProduct);

                return new ResultView<OrderProducutDTo>
                {
                    IsSuccess = true,
                    Message = "Order product updated successfully.",
                    Entity = updatedOrderProductDto
                };
            }
            catch (Exception ex)
            {
                return new ResultView<OrderProducutDTo>
                {
                    IsSuccess = false,
                    Message = "Failed to update order product: " + ex.Message
                };
            }
        }










        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            //return _mapper.Map<IEnumerable<OrderDto>>(orders);

            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserID = o.User?.Id ?? string.Empty, 
                UserName = o.User?.UserName ?? "Unknown User", 
                DatePlaced = o.DatePlaced,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                AddressId = o.AddressId,
                Cities = o.User?.Addresses.Select(a => a.City).ToList() ?? new List<string> { "Unknown City" },
                Streets = o.User?.Addresses.Select(a => a.Street).ToList() ?? new List<string> { "Unknown Street" }
            }).ToList();

            
            return ordersDto;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var orderitem = await _orderRepository.GetByIdAsync(orderId);
            orderitem.IsDeleted = true;
            await _orderRepository.SaveChangesAsync();
        }



        public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(string userId)
        {
            if (userId == "")
            {
                return null;
            }
            else
            {
                var orders = await _orderRepository.GetOrdersByUserId(userId);
                var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
                return orderDtos;
            }
        }
        public async Task<IQueryable<OrderDetailsDTO>> GetOrderDetailsByorderId(int orderid)
        {
            var order = await _orderRepository.GetOrderDetailsByordrId(orderid);
            return order;
        }

    }
}