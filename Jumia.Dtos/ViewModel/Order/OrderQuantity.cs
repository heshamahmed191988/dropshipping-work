using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderQuantity
    {
        public int quantity { get; set; }

        public int productID { get; set; }
        public int unitAmount { get; set; }
    }
}