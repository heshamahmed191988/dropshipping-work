using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Jumia.Dtos.ViewModel.User;
using Jumia.model;

namespace Jumia.Dtos.ViewModel.Order
{
    public class OrderDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserID { get; set; }
        [Required] public string UserName { get; set; }
        public int AddressId { get; set; }
        public List<string> ?Cities { get; set; } = new List<string>(); // Collection of cities
        public List<string> ?Streets { get; set; } = new List<string>();
        [Required]
        public DateTime DatePlaced { get; set; }
        public string? BarcodeImageUrl { get; set; }
        public decimal? DeliveryPrice { get; set; } = 5;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [ForeignKey("Id")]
        public virtual ApplicationUserDto User { get; set; }
        public virtual ICollection<OrderAddress>? OrderAddresses { get; set; }

        public virtual ICollection<CartItemDto> OrderDetails { get; set; }
        public virtual PaymentDto Payment { get; set; }

    }
}