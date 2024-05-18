using Jumia.Application.Services;
using Jumia.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Jumia.Dtos.ViewModel.Order;
using OfficeOpenXml;
using Spire.Barcode;
namespace Jumia.Mvc.Controllers
{
    [Authorize]

    public class AdminController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AdminController> _localizer;

        public AdminController(IOrderService orderService, IConfiguration configuration,IStringLocalizer<AdminController> localizer) 
        {
            this.orderService = orderService;
            _configuration = configuration;
            _localizer = localizer;
          
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult setLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
            );

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                // Fallback URL if the returnUrl is null, empty, or not local
                returnUrl = Url.Action("Index"); // Or any other default route
            }

            return LocalRedirect(returnUrl);
        }


        public async Task<IActionResult> DisplayOrders(string searchString, int pageNumber = 1, int pageSize =50)
        {
            try
            {
                // Get orders data list for the specified page with addresses
                var ordersDataList = await orderService.GetAllOrdersWithAddressAsync(pageNumber, pageSize);
                var ordersDto = ordersDataList.ToList(); // Convert to list

                // If a search string is provided, filter orders based on it
                if (!string.IsNullOrEmpty(searchString))
                {
                    // Assuming order.Status is the property you want to search in
                    ordersDto = ordersDto.Where(o => o.Status.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                         .ToList();
                }

                // Calculate total pages
                var totalOrders = await orderService.GetTotalOrdersCountAsync();
                var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                ViewBag.PageNumber = pageNumber; // Pass pageNumber to the view
                ViewBag.TotalPages = totalPages; // Pass totalPages to the view

                return View(ordersDto);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                TempData["ErrorMessage"] = "An error occurred while retrieving orders: " + ex.Message;
                return RedirectToAction(nameof(DisplayOrders));
            }
        }



        public async Task<IActionResult> ExportAllOrdersToExcel(string searchString)
        {
            try
            {
                const int pageSize = 5000000; // Set the page size
                int pageNumber = 1; // Initialize the page number
                var allOrders = new List<OrderWithAddressDTO>(); // List to store all orders

                // Loop through all pages to fetch orders
                while (true)
                {
                    // Get orders data list for the current page
                    var ordersDataList = await orderService.GetAllOrdersWithAddressAsync(pageNumber, pageSize);
                    var ordersDto = ordersDataList.ToList(); // Convert to list

                    // If a search string is provided, filter orders based on it
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        // Assuming order.Status is the property you want to search in
                        ordersDto = ordersDto.Where(o => o.Status.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                             .ToList();
                    }

                    // Add fetched orders to the list
                    allOrders.AddRange(ordersDto);

                    // Break the loop if the current page is the last page
                    if (ordersDto.Count < pageSize)
                        break;

                    pageNumber++; // Move to the next page
                }

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a new worksheet
                    var worksheet = package.Workbook.Worksheets.Add("All Orders");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Order ID";
                    worksheet.Cells[1, 2].Value = "Username";
                    worksheet.Cells[1, 3].Value = "Date Listed";
                    worksheet.Cells[1, 4].Value = "Total Price";
                    worksheet.Cells[1, 5].Value = "City";
                    worksheet.Cells[1, 6].Value = "Street";
                    worksheet.Cells[1, 7].Value = "Status";
                    worksheet.Cells[1,10].Value = "Barcode Image"; // Changed to indicate barcode image
                    worksheet.Cells[1, 8].Value = "Product Name"; // Added product name column
                    worksheet.Cells[1, 9].Value = "Quantity"; // Added quantity column
                    worksheet.Cells[1, 10].Value = "unit price";
                    int rowIndex = 2; // Start from the second row for data

                    // Fill data rows
                    // Fill data rows
                    foreach (var order in allOrders)
                    {
                        foreach (var product in order.Products)
                        {
                            // Write order details
                            worksheet.Cells[rowIndex, 1].Value = order.OrderId;
                            worksheet.Cells[rowIndex, 2].Value = order.UserName;
                            worksheet.Cells[rowIndex, 3].Value = order.DatePlaced.ToString("dd/MM/yyyy");
                            worksheet.Cells[rowIndex, 4].Value = order.TotalPrice;
                            worksheet.Cells[rowIndex, 5].Value = order.City;
                            worksheet.Cells[rowIndex, 6].Value = order.Street;
                            worksheet.Cells[rowIndex, 7].Value = order.Status;

                            // Write product details
                            worksheet.Cells[rowIndex, 8].Value = product.NameEn;
                            worksheet.Cells[rowIndex, 9].Value = product.StockQuantity;
                            worksheet.Cells[rowIndex, 10].Value = product.SelectedPrice;

                            rowIndex++;
                        }

                        // Convert Base64 string to byte array
                        byte[] imageBytes = Convert.FromBase64String(order.BarcodeImageUrl);

                        // Add the barcode image to the worksheet
                        var barcodeImage = worksheet.Drawings.AddPicture("Barcode" + rowIndex, new MemoryStream(imageBytes));
                        barcodeImage.SetPosition(rowIndex - 2, 0, 10, 0); // This will place the barcode in column 9, at the same row height as the other details

                        // Set the size of the barcode image to fit inside the cell
                        barcodeImage.SetSize(100, 20); // Width and height in pixels, adjust as needed

                        // Optionally, set the properties to ensure the image fits well inside the cell
                        barcodeImage.EditAs = OfficeOpenXml.Drawing.eEditAs.OneCell; // This makes the image move and size with the cell
                        barcodeImage.From.ColumnOff = 0;
                        barcodeImage.From.RowOff = 0;

                        // Increment the row index for the next order
                        rowIndex++;
                    }



                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Convert the package to a byte array
                    var excelBytes = package.GetAsByteArray();

                    // Return the Excel file
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllOrders.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                TempData["ErrorMessage"] = "An error occurred while exporting all orders to Excel: " + ex.Message;
                return RedirectToAction(nameof(DisplayOrders));
            }
        }

        public static int Pixel2MTU(int pixels)
{
    // 1 pixel = 9525 EMU
    return pixels * 9525;
}

        public async Task<IActionResult> ExportCurrentPageToExcel(string searchString, int pageNumber = 1, int pageSize = 70)
        {
            try
            {
               // const int pageSize = 5000000; // Set the page size
                var ordersDataList = await orderService.GetAllOrdersWithAddressAsync(pageNumber, pageSize);
                var ordersDto = ordersDataList.ToList(); // Convert to list

                // If a search string is provided, filter orders based on it
                if (!string.IsNullOrEmpty(searchString))
                {
                    // Assuming order.Status is the property you want to search in
                    ordersDto = ordersDto.Where(o => o.Status.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                         .ToList();
                }

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a new worksheet
                    var worksheet = package.Workbook.Worksheets.Add("All Orders");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Order ID";
                    worksheet.Cells[1, 2].Value = "Username";
                    worksheet.Cells[1, 3].Value = "Date Listed";
                    worksheet.Cells[1, 4].Value = "Total Price";
                    worksheet.Cells[1, 5].Value = "City";
                    worksheet.Cells[1, 6].Value = "Street";
                    worksheet.Cells[1, 7].Value = "Status";
                    worksheet.Cells[1, 8].Value = "Product Name"; // Added product name column
                    worksheet.Cells[1, 9].Value = "Quantity";
                    worksheet.Cells[1, 10].Value = "unit price";// Added quantity column
                    worksheet.Cells[1, 11].Value = "Barcode Image"; // Changed to indicate barcode image

                    int rowIndex = 2; // Start from the second row for data

                    // Fill data rows
                    foreach (var order in ordersDto)
                    {
                        foreach (var product in order.Products)
                        {
                            // Write order details
                            worksheet.Cells[rowIndex, 1].Value = order.OrderId;
                            worksheet.Cells[rowIndex, 2].Value = order.UserName;
                            worksheet.Cells[rowIndex, 3].Value = order.DatePlaced.ToString("dd/MM/yyyy");
                            worksheet.Cells[rowIndex, 4].Value = order.TotalPrice;
                            worksheet.Cells[rowIndex, 5].Value = order.City;
                            worksheet.Cells[rowIndex, 6].Value = order.Street;
                            worksheet.Cells[rowIndex, 7].Value = order.Status;

                            // Write product details
                            worksheet.Cells[rowIndex, 8].Value = product.NameEn;
                            worksheet.Cells[rowIndex, 9].Value = product.StockQuantity;
                            worksheet.Cells[rowIndex, 10].Value = product.SelectedPrice;
                            rowIndex++;
                        }

                        // Convert Base64 string to byte array
                        byte[] imageBytes = Convert.FromBase64String(order.BarcodeImageUrl);

                        // Add the barcode image to the worksheet
                        var barcodeImage = worksheet.Drawings.AddPicture("Barcode" + rowIndex, new MemoryStream(imageBytes));
                        barcodeImage.SetPosition(rowIndex - 2, 0, 11, 0); // This will place the barcode in column 10, at the same row height as the other details

                        // Set the size of the barcode image to fit inside the cell
                        barcodeImage.SetSize(100, 20); // Width and height in pixels, adjust as needed

                        // Optionally, set the properties to ensure the image fits well inside the cell
                        barcodeImage.EditAs = OfficeOpenXml.Drawing.eEditAs.OneCell; // This makes the image move and size with the cell
                        barcodeImage.From.ColumnOff = 0;
                        barcodeImage.From.RowOff = 0;

                        // Increment the row index for the next order
                        rowIndex++;
                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Convert the package to a byte array
                    var excelBytes = package.GetAsByteArray();

                    // Return the Excel file
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllOrders.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                TempData["ErrorMessage"] = "An error occurred while exporting all orders to Excel: " + ex.Message;
                return RedirectToAction(nameof(DisplayOrders));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            await orderService.UpdateOrderStatusAsync(orderId, status);
            return RedirectToAction("DisplayOrders","Admin");
        }
        [HttpPost]
        public JsonResult DashBoardcount()
        {
            try
            {
                List<string[]> DashBoardcount = new List<string[]>();
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT BrandNameEn, COUNT(*) AS NumberOfProducts FROM dbo.Products GROUP BY BrandNameEn", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string[] data = new string[2];
                        data[0] = reader["BrandNameEn"].ToString();
                        data[1] = reader["NumberOfProducts"].ToString();
                        DashBoardcount.Add(data);
                    }
                    reader.Close();
                }
                return Json(DashBoardcount);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }
        [HttpPost]

        public JsonResult DashBoardreview()
        {
            try
            {
                List<string[]> DashBoardreview = new List<string[]>();
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT  p.NameEn, AVG(r.Rating) AS AverageRating FROM  dbo.Products p JOIN reviews r ON p.Id = r.ProductID GROUP BY p.Id, p.NameEn ORDER BY AVG(r.Rating) DESC", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string[] data = new string[2];
                        data[0] = reader["NameEn"].ToString();
                        data[1] = reader["AverageRating"].ToString();
                        DashBoardreview.Add(data);
                    }

                    reader.Close();
                }
                return Json(DashBoardreview);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ProductCountDashbord()
        {
            try
            {
                int productCount = 0;
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS ProductCount FROM dbo.Products;", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        productCount = Convert.ToInt32(reader["ProductCount"]);
                    }
                    reader.Close();
                }
                return Json(new { ProductCount = productCount });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult OrderDashbord()
        {
            try
            {
                int totalOrders = 0;
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS TotalOrders FROM [dbo].[Orders]", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        totalOrders = Convert.ToInt32(reader["TotalOrders"]);
                    }
                    reader.Close();
                }
                return Json(new { TotalOrders = totalOrders });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PaymentDashbord()
        {
            try
            {
                decimal totalpayment = 0;
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT SUM(TotalPrice) AS TotalPaymentForAllProducts FROM [dbo].[orderProducts]", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        totalpayment = Convert.ToDecimal(reader["TotalPaymentForAllProducts"]);
                    }
                    reader.Close();
                }
                return Json(new { TotalPayment = totalpayment });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult UserDashbord()
        {
            try
            {
                int totalusers = 0;
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS NumberOfUsers FROM [dbo].[AspNetUsers]", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        totalusers = Convert.ToInt32(reader["NumberOfUsers"]);
                    }
                    reader.Close();
                }
                return Json(new { TotalUsers = totalusers });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult SellesbyYear()
        {
            var orderData = new List<object>();

            try
            {
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT DATEPART(YEAR, DatePlaced) AS Year, COUNT(*) AS NumberOfOrders FROM [dbo].[Orders] GROUP BY DATEPART(YEAR, DatePlaced) ORDER BY Year", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var year = Convert.ToInt32(reader["Year"]);
                        var numberOfOrders = Convert.ToInt32(reader["NumberOfOrders"]);
                        orderData.Add(new object[] { year.ToString(), numberOfOrders });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }

            return Json(orderData);
        }

        [HttpPost]
        public JsonResult GetTopSellers()
        {
            var topSellers = new List<object>();

            try
            {
                string connectionString = _configuration.GetConnectionString("Db");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP (3) p.NameEn, SUM(op.Quantity) AS TotalQuantitySold FROM [dbo].[products] p JOIN [dbo].[orderProducts] op ON p.Id = op.ProductId GROUP BY p.Id, p.NameEn ORDER BY TotalQuantitySold DESC", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var name = reader["NameEn"].ToString();
                        var totalQuantitySold = Convert.ToInt32(reader["TotalQuantitySold"]);
                        topSellers.Add(new { Name = name, TotalQuantitySold = totalQuantitySold });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }

            return Json(topSellers);
        }



    }

}


