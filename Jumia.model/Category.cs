using Jumia.model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Model
{
    public class Category : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }
        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

}
