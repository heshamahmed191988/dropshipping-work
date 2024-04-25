using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.User
{
    public class UpdatUserInfo
    {
        public string? Id { get; set; }
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }



        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        public bool RememberMe { get; set; }
    }
}