using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolMgt.Domain.Models
{
    public class Employee
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool ActiveStatus { get; set; }
        public int DepartmentId { get; set; }
        public string PhotoName { get; set; }

        public Department Department { get; set; }
    }

    
}
