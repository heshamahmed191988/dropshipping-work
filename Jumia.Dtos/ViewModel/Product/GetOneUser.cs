using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Product
{
    public class GetOneUser
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public int StockQuantity { get; set; }

        public string BrandNameAr { get; set; }

        public string BrandNameEn { get; set; }

        public ICollection<string> Productimages { get; set; }
        public string descriptionAr { get; set; }
        public string descriptionEn { get; set; }


        public ICollection<string> colors { get; set; }
        public ICollection<string> itemimages { get; set; }
        public decimal price { get; set; }

        public string? CategoryNameAr { get; set; }
        public string? CategoryNameEn { get; set; }
    }
}