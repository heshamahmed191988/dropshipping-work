using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Jumia.Dtos.ViewModel.Order;

namespace Jumia.Dtos
{
    public class PaymentDto
    {
        public int Id { get; set; }
        [Required]
        public DateTime DatePaid { get; set; }
        [Required]
        [MaxLength(50)]
        public string paymentMethod { get; set; } = "PayPal";
        public  int OrderId { get; set; }
    }
}
