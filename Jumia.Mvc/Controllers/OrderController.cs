using Jumia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }


        public async Task<IActionResult> Index()
        {
            // Retrieve orders from the service
            var orders = await orderService.GetAllOrdersAsync();

            // Pass the orders to the view
            return View(orders);
        }


        public async Task<IActionResult> deliverd()
        {
            // Retrieve orders from the service
            var orders = await orderService.GetAllOrdersAsync();

            // Pass the orders to the view
            return View(orders);
        }
        public async Task<IActionResult> Notdeliverd()
        {
            // Retrieve orders from the service
            var orders = await orderService.GetAllOrdersAsync();

            // Pass the orders to the view
            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> ScanBarcode(string barcode)
        {
            try
            {
                int orderId = ExtractOrderIdFromBarcode(barcode);

                // Update order status to "Processing"
                var success = await orderService.UpdateOrderStatusAsync2(orderId, "Processing");

                if (success)
                {
                    return RedirectToAction("Index", "Order"); // Redirect to order list page
                }
                else
                {
                    // Handle error
                    return RedirectToAction("Index", "Home"); // Redirect to home page or display an error message
                }
            }
            catch (ArgumentException ex)
            {
                // Handle invalid barcode format
                ModelState.AddModelError("Barcode", "Invalid barcode format");
                return View(); // Return to the same view with an error message
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateScannedOrdersStatus(List<int> orderIds)
        {
            foreach (var orderId in orderIds)
            {
                await orderService.UpdateOrderStatusAsync2(orderId, "Processing");
            }
            return RedirectToAction("Index"); 
        }
        [HttpPost]
        public async Task<IActionResult> UpdateScannedOrdersStatusfordelevierd(List<int> orderIds)
        {
            foreach (var orderId in orderIds)
            {
                await orderService.UpdateOrderStatusAsync2(orderId, "Delivered");
            }
            return RedirectToAction("deliverd"); 
        }


        [HttpPost]
        public async Task<IActionResult> UpdateScannedOrdersStatusforNotdelevierd(List<int> orderIds)
        {
            foreach (var orderId in orderIds)
            {
                await orderService.UpdateOrderStatusAsync2(orderId, "NotDelivered");
            }
            return RedirectToAction("Notdeliverd"); 
        }
        private int ExtractOrderIdFromBarcode(string barcode)
        {
            // Assuming the barcode contains the order ID as the first part before any delimiter
            // You may need to adjust this logic based on your actual barcode format
            string[] parts = barcode.Split('-'); // Assuming '-' is the delimiter separating the order ID
            if (parts.Length > 0 && int.TryParse(parts[0], out int orderId))
            {
                return orderId;
            }
            throw new ArgumentException("Invalid barcode format");
        }
    }
}
