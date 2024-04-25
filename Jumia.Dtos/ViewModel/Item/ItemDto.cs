using System.ComponentModel.DataAnnotations.Schema;
using Jumia.Dtos.ViewModel.Product;

namespace Jumia.Dtos.ViewModel.Item
{
    public class ItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        [ForeignKey("Id")]
        public ProductDTO Product { get; set; }
    }
}