using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Product
{
    public class ProductFilterCriteria
    {
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public PriceSortOrder PriceSortOrder { get; set; } = PriceSortOrder.None;
    }

    public enum PriceSortOrder
    {
        None,
        asc,
        desc
    }

}
