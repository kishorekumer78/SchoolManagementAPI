using SchoolMgt.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolMgt.Domain.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int id);
    }
}
