using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolMgt.Models.ViewModels;

namespace SchoolMgt.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly SignInManager<IdentityUser> signInManager;
#pragma warning restore IDE0052 // Remove unread private members
        private readonly UserManager<IdentityUser> userManager;

        public AccountController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            // will hold all the errors related to registration
            List<string> errorList = new List<string>();

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) // If creating new user is successful
            {
                // Asigning User role the newly registered user.
                await userManager.AddToRoleAsync(user, "User");

                // TODO: sending confirmation email

                // we dont want to send the password of the user. user which is an object of IdentityUser class have password property 
                // so we are creating a new object and only sending non sensitive information
                return Ok(new { userName = user.UserName, email = user.Email, status = 1, message = "Registration Successful" });
            }
            else // if creating new user is not successful 
            {
                foreach (var error in result.Errors)
                {
                    errorList.Add(error.Description);
                }

                return BadRequest(new JsonResult(errorList));
            }

        }
    }
}