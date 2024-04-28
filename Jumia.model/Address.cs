using Jumia.model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Model
{
    public class Address : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Street { get; set; }
        [Required]
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(100)]
        public string State { get; set; }
        [MaxLength(20)]
        public string ZipCode { get; set; }
        public string UserID { get; set; }

        public int OrderId { get; set; }

        // Navigation property
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<OrderAddress> OrderAddresses { get; set; }

        // Collection of Orders for the one-to-many relationship
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
