using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Authorize(Policy = "LoggedInPrevilage")]
        public IActionResult Get()
        {
            return Ok(context.Employees.Include(x => x.Department).ToList());
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var emp = context.Employees.FirstOrDefault(x => x.Id == id);
            if (emp != null)
            {
                return Ok(emp);
            }
            return NotFound(new { error = $"Employee with ID = {id} does not exist" });
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] EmployeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // take out the image from model   and assign file name to it          
            GetPhotoPath(model.Photo, out IFormFile img, out string uniqueFileName, out string filePath);
            var employee = new Employee
            {
                Name = model.Name,
                // convert dateofbirth that is passed from front end as string to C# DateTime
                DateOfBirth = DateTime.ParseExact(model.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture),
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
                return Ok(employee);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = e.Message, message = "Employee Creation Unsuccessful" });
            }

        }


        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromForm] EmployeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // bring the old details of employee from db
            Employee emp = context.Employees.FirstOrDefault(x => x.Id == id);
            // if employee with passed id does not exist return error message
            if (emp == null)
            {
                return NotFound(new { error = $"employee with id {id} does not exist" });
            }

            // get photo details from model.Photo 
            GetPhotoPath(model.Photo, out IFormFile img, out string uniqueFileName, out string filePath);

            // assign the new properties to found employee object from  model 
            emp.Name = model.Name;
            emp.PhoneNumber = model.PhoneNumber;
            emp.Email = model.Email;
            emp.DateOfBirth = DateTime.ParseExact(model.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            emp.DepartmentId = model.DepartmentId;
            emp.ActiveStatus = model.ActiveStatus;
            // photo handling
            if (model.Photo != null)
            {
                if (emp.PhotoName != null)
                {
                    // delete the old photo
                    DeletePhoto(emp.PhotoName);
                }
                // assign new photoname
                emp.PhotoName = uniqueFileName;
            }

            // save new properties of emp obj to db with try catch block
            try
            {
                context.Entry(emp).State = EntityState.Modified;
                var x = await context.SaveChangesAsync();

                if (x > 0 && model.Photo != null)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }
                }

                // send the updated employee back to front end 
                return Ok(emp);

            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = e.Message });
            }           

        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emp = context.Employees.FirstOrDefault(x => x.Id == id);
            if (emp == null)
            {
                return NotFound(new { error = $"Employee with ID  {id} does not exist" });
            }
            context.Employees.Remove(emp);
            await context.SaveChangesAsync();
            return Ok(new { message = $"Employee with ID = {emp.Id} has been deleted successfully" });
        }


        // This method receives a imageFile and gives the file , filePath and a unique filepath to save in the server
        private void GetPhotoPath(IFormFile image, out IFormFile img, out string uniqueFileName, out string filePath)
        {
            img = image;
            string uploadFolder = Path.Combine(environment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + ((img == null) ? "" : img.FileName);
            filePath = Path.Combine(uploadFolder, uniqueFileName);
        }
        // This method receives a file name and deletes the file from server image folder
        private void DeletePhoto(string photoName)
        {
            string filePath = Path.Combine(environment.WebRootPath, "images", photoName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

    }
}