using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderDetailsDTO
    {

        [Required]
        public string UserID { get; set; }
        public int orderitemid { get; set; }

        [Required]
        public DateTime DatePlaced { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        public int Quantity { get; set; }
        public string productname { get; set; }
        public int orderid { get; set; }
        public int productid { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ?SelectedPrice { get; set; }
        public string? Street {  get; set; }
        public string? City { get; set; }

        public string? State { get; set; }
        public string? ZipCode { get; set; }


    }
}