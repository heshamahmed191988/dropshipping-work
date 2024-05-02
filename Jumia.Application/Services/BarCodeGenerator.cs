using Jumia.Model;
using Spire.Barcode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
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
}
