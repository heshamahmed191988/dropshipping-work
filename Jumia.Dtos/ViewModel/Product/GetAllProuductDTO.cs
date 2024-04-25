using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.Dtos.ViewModel.User;

namespace Jumia.Dtos.ViewModel.Product
{
    public class GetAllProuductDTO
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //public int SellerID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }
        [Required]
        [MaxLength(256)]

        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public DateTime DateListed { get; set; }
        public string CategoryName { get; set; }

        public virtual ApplicationUserDto Seller { get; set; }
        ////public virtual ICollection<Review> Reviews { get; set; }
        //public virtual ICollection<CartItemDto> OrderDetails { get; set; }
        //public virtual ICollection<ItemDto> items { get; set; }
        // public virtual ICollection<ProductImageDto> ProductImages { get; set; }
    }
}