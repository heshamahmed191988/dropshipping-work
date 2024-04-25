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
    public class Payment : BaseEntity
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int orderID { get; set; }

        [Required]
        public DateTime DatePaid { get; set; }

        //public enum PaymentMethod
        //{
        //    PayPal,
        //    CashOnDelievary,

        //}

        [Required]
        [MaxLength(50)]
        public string paymentMethod { get; set; } = "PayPal";
        public virtual Order Order { get; set; }
    }

}
