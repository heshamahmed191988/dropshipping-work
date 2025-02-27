﻿using AutoMapper;
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
using Spire.Barcode;

using Jumia.Dtos.ViewModel.category;
using Jumia.Dtos.ViewModel.Product;
using QRCoder;
using System.Drawing;
using Newtonsoft.Json;
using Spire.Barcode;

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

        public async Task<ResultView<OrderDto>> CreateOrderAsync(List<OrderQuantity> ProductIDs, string UserID, int AddressId, decimal? DeliveryPrice)
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
                    DeliveryPrice = DeliveryPrice
                    
                };

                var createdOrder = await _orderRepository.CreateAsync(order);
                await _orderRepository.SaveChangesAsync();
                var barcode = BarcodeGenerator.GenerateUniqueBarcode(order);
                order.BarcodeImageUrl = barcode;
                
           

                if (createdOrder.Id == 0) // Assuming 0 is the default value for Id
                {
                    throw new Exception("Order ID not set after saving changes.");
                }
                // Generate unique barcode for the order with order details
              

              

                foreach (var id in ProductIDs)
                {
                    await _orderProuduct.CreateAsync(new OrderProduct
                    {
                        ProductId = id.productID,
                       
                        OrderId = order.Id,
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
            // Generate a unique barcode containing order details
            public static string GenerateUniqueBarcode(Order order)
            {
                // Format order details into a structured format
                string orderDetails = $"Order ID: {order.Id}\n" +
                                      //$"Date Placed: {order.DatePlaced}\n" +
                                      //$"Total Price: {order.TotalPrice}\n" +
                                      $"Status: {order.Status}\n";
                                      //$"User ID: {order.UserID}\n" +
                                      //$"Address ID: {order.AddressId}\n";

                // Initialize a barcode generator instance
                BarcodeSettings settings = new BarcodeSettings
                {
                    Data = orderDetails,
                    Data2D = orderDetails,
                    Type = BarCodeType.Code128,
                    ShowText = true,
                    TextFont = new Font("Arial", 8f)
                };

                // Increase the size of the barcode image
                settings.BarHeight = 15;
            

                BarCodeGenerator generator = new BarCodeGenerator(settings);

                // Generate the barcode image
                Image barcodeImage = generator.GenerateImage();

                // Convert the barcode image to Base64 string
                string base64String;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    barcodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = memoryStream.ToArray();
                    base64String = Convert.ToBase64String(byteImage);
                }

                return base64String;
            }
        }

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

            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserID = o.User?.Id ?? string.Empty,
                UserName = o.User?.UserName ?? "Unknown User",
                DatePlaced = o.DatePlaced,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                AddressId = o.AddressId,
                BarcodeImageUrl = o.BarcodeImageUrl,
                Cities = o.User?.Addresses?.Select(a => a.City).ToList() ?? new List<string>(),
                Streets = o.User?.Addresses?.Select(a => a.Street).ToList() ?? new List<string>()
            }).ToList();

            // Alternatively, you can handle "Unknown City" and "Unknown Street" here if needed

            return ordersDto;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            // Calculate the number of items to skip based on the page number and page size
            var itemsToSkip = (pageNumber - 1) * pageSize;

            var ordersDto = orders.Skip(itemsToSkip)
                                  .Take(pageSize)
                                  .Select(o => new OrderDto
                                  {
                                      Id = o.Id,
                                      UserID = o.User?.Id ?? string.Empty,
                                      UserName = o.User?.UserName ?? "Unknown User",
                                      DatePlaced = o.DatePlaced,
                                      TotalPrice = o.TotalPrice,
                                      Status = o.Status,
                                      AddressId = o.AddressId,
                                      BarcodeImageUrl = o.BarcodeImageUrl,
                                      Cities = o.User?.Addresses?.Select(a => a.City).ToList() ?? new List<string>(),
                                      Streets = o.User?.Addresses?.Select(a => a.Street).ToList() ?? new List<string>()
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
        public async Task<int> GetTotalOrdersCountAsync()
        {
            try
            {
                return await _orderRepository.GetTotalOrdersCountAsync();
            }
            catch (Exception ex)
            {
                // Handle any potential exceptions here
                // Log or throw as needed
                throw new Exception("Failed to retrieve total orders count: " + ex.Message, ex);
            }
        }

        public async Task<List<OrderDto>> SearchOrdersByIdAsync(int orderId)
        {
            var orders = await _orderRepository.SearchOrdersByIdAsync(orderId);
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    UserID = order.UserID,
                    UserName = order.User.UserName ?? "Unknown User", // Assuming User is a navigation property
                    AddressId = order.AddressId,
                    Cities = order.OrderAddresses?
        .Where(a => a.Address != null && a.Address.City != null)
        .Select(a => a.Address.City)
        .ToList() ?? new List<string>(), // Handle null with an empty list
                    Streets = order.OrderAddresses?
        .Where(a => a.Address != null && a.Address.Street != null)
        .Select(a => a.Address.Street)
        .ToList() ?? new List<string>(),
                    DatePlaced = order.DatePlaced,
                    BarcodeImageUrl = order.BarcodeImageUrl,
                    DeliveryPrice = order.DeliveryPrice,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status
                  
                };

                orderDtos.Add(orderDto);
            }

            return orderDtos;
        }



        public async Task<bool> UpdateOrderStatusAsync2(int orderId, string newStatus)
        {
            return await _orderRepository.UpdateOrderStatusAsync2(orderId, newStatus);
        }


    }
}