using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class Createorder
    {
        public string UserID { get; set; }
        public List<OrderQuantity> orderQuantities {  get; set; }
        public int AddressId { get; set; }

        public decimal? DeliveryPrice { get; set; } = 5;
        public decimal  Earning { get; set; }

    }
}
