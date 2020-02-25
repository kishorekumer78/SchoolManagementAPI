using SchoolMgt.Domain.Models;
using SchoolMgt.Domain.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolMgt.Domain.Repository.Repos
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public List<Employee> EmpList { get; set; }

        public EmployeeRepository()
        {
            EmpList = new List<Employee>
            {
                new Employee { Id = 101, Name = "John", ActiveStatus = true, DateOfBirth = new DateTime(1980, 3, 25), DepartmentId = 2, Email = "john@test.com", PhoneNumber = "78956422", PhotoName = "john.jfif" },
                new Employee { Id = 102, Name = "Mary", ActiveStatus = true, DateOfBirth = new DateTime(1990, 7, 14), DepartmentId = 1, Email = "mary@test.com", PhoneNumber = "78956422", PhotoName = "mary.jfif" },
                new Employee { Id = 103, Name = "Richard", ActiveStatus = true, DateOfBirth = new DateTime(1989, 9, 17), DepartmentId = 4, Email = "richard@test.com", PhoneNumber = "78956422", PhotoName = "richard.jfif" }
            };
        }

        public Employee GetEmployee(int id)
        {
            return EmpList.FirstOrDefault(x => x.Id == id);
        }
    }
}
