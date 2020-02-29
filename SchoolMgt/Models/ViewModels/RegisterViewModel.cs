using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMgt.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required, MaxLength(50)]
        public string UserName { get; set; }
        [Required, MaxLength(50),EmailAddress]
        public string Email { get; set; }
        [Required, MaxLength(50)]
        public string Password { get; set; }
    }
}
