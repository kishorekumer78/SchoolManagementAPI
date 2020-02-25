using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMgt.Models.ViewModels
{
    public class EmployeeCreateViewModel    
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool ActiveStatus { get; set; }
        public int DepartmentId { get; set; }
        public IFormFile Photo { get; set; }
        public string PhotoName { get; set; }   
    }
}
