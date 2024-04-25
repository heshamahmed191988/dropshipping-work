using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Product
{
    public class CreateOrUpdateProuductDTO
    {

        public int Id { get; set; }

        public int SellerID { get; set; }

        public string BrandNameAr { get; set; }
        public string BrandNameEn { get; set; }

        public int CategoryID { get; set; }

        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public DateTime DateListed { get; set; }

        //[ForeignKey("Id")]
        //public virtual ApplicationUserDto Seller { get; set; }



        ////public virtual ICollection<Review> Reviews { get; set; }
        //public virtual ICollection<CartItemDto> OrderDetails { get; set; }
        //public virtual ICollection<ItemDto> items { get; set; }
        //public virtual ICollection<ProductImageDto> ProductImages { get; set; }
    }
}