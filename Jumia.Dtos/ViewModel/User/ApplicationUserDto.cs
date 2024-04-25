using System.ComponentModel.DataAnnotations;
using Jumia.Dtos.ViewModel.Order;
using Jumia.Dtos.ViewModel.Product;

namespace Jumia.Dtos.ViewModel.User
{
    public class ApplicationUserDto
    {

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(256)]
        public string FullName { get; set; }
        public string? Role { get; set; }
        public string AddressId { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public bool? IsDeleted { get; set; }
        public virtual ICollection<OrderDto> Orders { get; set; }
        public virtual ICollection<ProductDTO> Products { get; set; }
        public virtual ICollection<ReviewDto> Reviews { get; set; }
        public virtual ICollection<AddressDto> Addresses { get; set; }
        public ApplicationUserDto()
        {
            Orders = new HashSet<OrderDto>();
            Products = new HashSet<ProductDTO>();
            Reviews = new HashSet<ReviewDto>();
            Addresses = new HashSet<AddressDto>(); // Initialize the addresses collection
        }
    }
}