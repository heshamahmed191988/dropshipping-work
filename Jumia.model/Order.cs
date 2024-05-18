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
    public class Order : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public string? UserID { get; set; }
        [Required]
        public DateTime DatePlaced { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }
        public string? BarcodeImageUrl { get; set; } = "fdgdggd";
        [Column(TypeName = "decimal(18,2)")]

        public decimal? DeliveryPrice { get; set; } = 5;
        [Column(TypeName = "decimal(18,2)")]

        public decimal? Totalearning { get; set; }

        // Foreign key for Address
        //[ForeignKey("Address")]

        public int AddressId { get; set; }

        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<OrderProduct>? Products { get; set; }
        public virtual Payment? Payment { get; set; }

        public virtual Address Address { get; set; }


        // Navigation property to Address
        public virtual ICollection<OrderAddress>? OrderAddresses { get; set; }
    }
}