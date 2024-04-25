using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.User
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? Role { get; set; }

        public string? AddressId { get; set; }

        public bool IsDeleted { get; set; } = false;


        // Constructor
        public UserViewModel()
        {
        }
    }
}
