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
    public class Review : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        //[ForeignKey("ProductID")]
        //public virtual Product Product { get; set; }

        [ForeignKey("UserID")] 
        public virtual ApplicationUser User { get; set; }
    }
}

