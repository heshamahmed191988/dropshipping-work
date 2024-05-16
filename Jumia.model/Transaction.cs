using Jumia.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Jumia.Model
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ?User { get; set; }
        public String? userName { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        [Required]
        [MaxLength(20)]
        public string ?WithdrawalMethod { get; set; }
        
        [Required]
        public DateTime DatePlaced { get; set; }
    }
}
