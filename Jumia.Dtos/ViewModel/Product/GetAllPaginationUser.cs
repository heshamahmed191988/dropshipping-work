using Jumia.Dtos.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Product
{
    public class GetAllPaginationUser
    {
        public int id { get; set; }
        public string BrandNameAr { get; set; } = "Amazon";
        public string BrandNameEn { get; set; } = "امازون";
        [Required]
        public int StockQuantity { get; set; }
        public ICollection<string> itemscolor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? clientPrice { get; set; }
        public virtual ICollection<string> ProductImages { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public decimal Price { get; set; }

        public IEnumerable<string>? Colors { get; set; }

    }
}