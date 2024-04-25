using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class UpdateOrderProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]

        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public int OrderItemId { get; set; }


    }
}