using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.Dtos.ViewModel.Product;
namespace Jumia.Dtos.ViewModel.category
{
    public class CateogaryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }
        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }

        public virtual ICollection<ProuductViewModel> Products { get; set; }
    }
}