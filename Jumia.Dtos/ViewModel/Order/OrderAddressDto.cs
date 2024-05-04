using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderAddressDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int AddressId { get; set; }
    }
}
