using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMgt.Models.ViewModels
{
    public class EmployeeCreateViewModel    
    {
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50),EmailAddress]
        public string Email { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public bool ActiveStatus { get; set; }
        public int DepartmentId { get; set; }
        public IFormFile Photo { get; set; }
        public string PhotoName { get; set; }   
    }
}
