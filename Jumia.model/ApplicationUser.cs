using Jumia.model;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;


namespace Jumia.Model
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser, IBaseEntity
    {
        private BaseEntity _baseEntity = new BaseEntity();

        public bool IsDeleted { get; set; } = false;
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        //[Required]
        //public string Password { get; set; } 

        //[Required]
        //[MaxLength(256)]
        ////public string ?FullName { get; set; }

        public int? AddressId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Earning { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public ApplicationUser()
        {
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
            Reviews = new HashSet<Review>();
            Addresses = new HashSet<Address>(); // Initialize the addresses collection
        }
    }
}
