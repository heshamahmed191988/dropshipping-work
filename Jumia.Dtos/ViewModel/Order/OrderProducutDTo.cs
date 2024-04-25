using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderProducutDTo
    {
        public int Id { get; set; }


        [Required]
        public string UserID { get; set; }

        public int OrderId { get; set; }
        [Required]
        public DateTime DatePlaced { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProducutId { get; set; }
    }
}