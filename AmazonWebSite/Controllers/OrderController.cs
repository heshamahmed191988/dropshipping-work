using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Jumia.Dtos.ViewModel.Order;
using Jumia.Application.Services;
using Jumia.Dtos.ResultView;
using Jumia.Dtos.ViewModel.Product;
using Jumia.Model;
using System.Configuration;
using System.Net.Http;
using Jumia.Dtos;
namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration configuration;
        private readonly IPaymentServices paymentServices;
        private readonly IUserService userService;

        public OrderController(IOrderService orderService, IConfiguration configuration,IPaymentServices paymentServices,IUserService userService)
        {
            _orderService = orderService;
            this.configuration = configuration;
            this.paymentServices = paymentServices;
            this.userService = userService;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateOrderAsync([FromBody] Createorder createorder)
        //{
        //    try
        //    {
        //        // Pass createorder properties to the service method
        //        var result = await _orderService.CreateOrderAsync(createorder.orderQuantities, createorder.UserID, createorder.AddressId);

        //        // Check the result and return appropriate response
        //        if (result.IsSuccess)
        //        {
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            return BadRequest(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}



        ///refactor order try to make it work 
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Createorder createorder)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(createorder.orderQuantities, createorder.UserID, createorder.AddressId, createorder.DeliveryPrice, createorder.Earning);

                if (result.IsSuccess)
                {
                    // Check if total earnings is greater than 0
                    if (createorder.Earning <= 0)
                    {
                        return BadRequest("Total earnings must be greater than 0.");
                    }

                    // Increase user earnings after order creation
                    //var increaseEarningResult = await _orderService.IncreaseUserEarnings(createorder.UserID, createorder.Earning);

                    //if (increaseEarningResult == null)
                    //{
                    //    return StatusCode(500, "Failed to increase user earnings: User not found");
                    //}

                    // Create payment for the order
                    var paymentDto = await CreatePaymentAsync(result.Entity.Id);

                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





        private async Task<PaymentDto> CreatePaymentAsync(int orderId)
        {
            var paymentDto = await paymentServices.CreatePaymentAsync(orderId);
            return paymentDto;
        }










































        //// GET: api/Order
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var order = await _orderService.GetAllOrdersAsync();
                return Ok(order);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        /// DELETE: api/Order/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        await _orderService.DeleteOrderAsync(id);
        //        return Ok("Order deleted successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateOrderProduct(UpdateOrderProductDto updateOrderProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.UpdateOrderProductAsync(updateOrderProductDto);

            if (result.IsSuccess)
            {
                return Ok(result.Entity);
            }

            return BadRequest(new { message = result.Message });
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserId(userId);
                if (orders == null)
                {
                    return NotFound();
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }
        }
        //[HttpGet("orderid")]
        //public async Task<IActionResult> GetOrderdetailsByUserId(int orderId)
        //{
        //    try
        //    {
        //        var orders = await _orderService.GetOrderDetailsByorderId(orderId);
        //        if (orders == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(orders);
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error");
        //    }
        //}



        [HttpGet("orderid")]
        public async Task<IActionResult> GetOrderdetailsByUserId(int orderId)
        {
            try
            {
                var orders = await _orderService.GetOrderDetailsByorderId(orderId);
                if (orders == null)
                {
                    return NotFound();
                }

                var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");
                var ordersDTO = new List<OrderDetailsDTO>(); // Assuming OrderDetailsDTO is your DTO class

                foreach (var order in orders)
                {
                    var fullPath = Path.Combine(basePath, order.ProductImage.Replace("/", "\\").TrimStart('\\'));
                    string base64String = null;

                    if (System.IO.File.Exists(fullPath))
                    {
                        var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                        base64String = Convert.ToBase64String(imageBytes);
                    }

                    // Manual mapping
                    ordersDTO.Add(new OrderDetailsDTO
                    {
                        // Assuming these are the properties of your OrderDetailsDTO
                        orderid = order.orderid,
                        UserID = order.UserID,
                        productname = order.productname,
                        TotalPrice = order.TotalPrice,
                        DatePlaced = order.DatePlaced,
                        Status = order.Status,
                        ProductDescription = order.ProductDescription,
                        ProductImage = base64String, // Set the base64 string here
                        ProductPrice = order.ProductPrice,
                        orderitemid = order.orderitemid,
                        Quantity = order.Quantity,
                        productid = order.productid,
                        SelectedPrice = order.SelectedPrice,
                        City=order.City,
                        Street=order.Street,
                        State = order.State,
                        ZipCode = order.ZipCode,

                    });
                }

                return Ok(ordersDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }







        [HttpPost("{userId}/earnings/increase")]
        public async Task<IActionResult> IncreaseUserEarnings(string userId, decimal amountToAdd)
        {
            var updatedUser = await _orderService.IncreaseUserEarnings(userId, amountToAdd);

            if (updatedUser == null)
                return NotFound(); // Handle case where user is not found

            return Ok(updatedUser);
        }



        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrderAsync(int orderId)
        {
            var result = await _orderService.DeleteOrderAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
