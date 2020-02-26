using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolMgt.DAL;
using SchoolMgt.Domain.Models;
using SchoolMgt.Models.ViewModels;

namespace SchoolMgt.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment environment;

        public EmployeeController(AppDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }


        // Get All Employees 
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(context.Employees.ToList());
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] EmployeeCreateViewModel model)
        {
            // todo: take out the image from model 
            // todo: what if there is no image provided
            var img = model.Photo;
            string uploadFolder = Path.Combine(environment.WebRootPath, "images");            
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + ((img == null) ? "" : img.FileName);
            var filePath = Path.Combine(uploadFolder, uniqueFileName);
            var employee = new Employee
            {
                Name = model.Name,
                DateOfBirth = DateTime.ParseExact(model.DateOfBirth,"dd/MM/yyyy",CultureInfo.InvariantCulture), // todo: ckthe format of dateofbirth that is passed from front end
                ActiveStatus = model.ActiveStatus,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                DepartmentId = model.DepartmentId
            };

            if (model.Photo != null)
            {
                employee.PhotoName = uniqueFileName;
            }
            // use try catch to do emp add and img upload
            try
            {
                context.Employees.Add(employee);
                var x = await context.SaveChangesAsync();
                if (x > 0 && model.Photo != null) // if db operation is successful and we received a photo only then upload img 
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

           
            return Ok(employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
