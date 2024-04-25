using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.Review
{
    public class ReviewAdminDTO
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public string UserID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; }
    }

}
