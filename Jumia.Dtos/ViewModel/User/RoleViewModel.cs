﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.User
{
    public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
