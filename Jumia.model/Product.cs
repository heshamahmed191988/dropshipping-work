using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.model;

namespace Jumia.Model
{
    public class Product : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SellerID { get; set; }

        public string BrandNameAr { get; set; } = "امازون";
        public string BrandNameEn { get; set; } = "Amazon";

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? clientPrice { get; set; }
        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        [Required]
        public int CategoryID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ?MaxPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public DateTime DateListed { get; set; }

        [ForeignKey("SellerID")]
        public virtual ApplicationUser Seller { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        //public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<OrderProduct> Orders { get; set; }
        public virtual ICollection<Item> items { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }

}
