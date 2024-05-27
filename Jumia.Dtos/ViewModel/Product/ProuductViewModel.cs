using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.Dtos.ViewModel.User;
using System.ComponentModel.DataAnnotations.Schema;
namespace Jumia.Dtos.ViewModel.Product
{
    public class ProuductViewModel
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }
        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }
        public string BrandNameAr { get; set; }
        public string BrandNameEn { get; set; }
        public int? TotalCount { get; set; }
        public string DescriptionAR { get; set; }
        public string DescriptionEn { get; set; }

        [Required]
        //[Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinPrice { get; set; }

        public ICollection<string>? itemscolor { get; set; }
        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public DateTime DateListed { get; set; }
        public string? SellerID { get; set; }

        //public virtual CateogaryViewModel Category { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryNameAr { get; set; }
        public string? CategoryNameEn { get; set; }
        public virtual LoginViewModel? Seller { get; set; }
        public string? SellerName { get; set; }
        public bool IsDeleted { get; set; } = false;
        public IEnumerable<string>? Colors { get; set; }
        // public virtual ICollection<ProductImageViewModel> ProductImages { get; set; }
    }
}