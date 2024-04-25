using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Product
{
    public class ProductFilterModel
    {
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Brand { get; set; }
        public int? MinRating { get; set; }
    }
}
