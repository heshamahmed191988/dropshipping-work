using Jumia.Model;
using Spire.Barcode;
using System.Drawing;
using System.IO;

namespace Jumia.Application.Services
{
    public static class BarcodeGenerator
    {
        // Generate a unique barcode containing order details
        public static byte[] GenerateUniqueBarcode(Order order)
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

            // Convert the barcode image to byte array
            using (MemoryStream memoryStream = new MemoryStream())
            {
                barcodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }
}
