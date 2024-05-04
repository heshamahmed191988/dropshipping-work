using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderCreateDto
    {
        public string UserId { get; set; }

        [Required]
        public DateTime DatePlaced { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
        public string? BarcodeImageUrl { get; set; } = "fdgdggd";

        [MaxLength(50)]
        public string Status { get; set; }
    }
}
