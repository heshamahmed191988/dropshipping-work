using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Jumia.Dtos.ViewModel.User;

namespace Jumia.Dtos
{
    public class ReviewDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        //[ForeignKey("ProductID")]
        //public virtual Product Product { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUserDto User { get; set; }
    }
}