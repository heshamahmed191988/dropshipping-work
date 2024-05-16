using Jumia.Dtos.ViewModel.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderWithAddressDTO
    {
        public int OrderId { get; set; }
        public string? UserName { get; set; } = "unknown";
        public DateTime DatePlaced { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string BarcodeImageUrl { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public List<ProductDTO> Products { get; set; }
    }
}
