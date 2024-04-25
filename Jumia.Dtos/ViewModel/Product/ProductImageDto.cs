using System.ComponentModel.DataAnnotations.Schema;

namespace Jumia.Dtos.ViewModel.Product
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int ProductID { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}