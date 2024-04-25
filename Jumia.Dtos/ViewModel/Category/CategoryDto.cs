using System.ComponentModel.DataAnnotations;

namespace Jumia.Dtos.ViewModel.category
{
    public class CategoryDto
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }
        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }

    }
}