using Jumia.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.model
{
    public class OrderAddress
    {
        [Key]
        public int Id { get; set; }

        // Foreign keys
        public int OrderId { get; set; }
        [ForeignKey("Address")]

        public int AddressId { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual Address Address { get; set; }
    }
}
