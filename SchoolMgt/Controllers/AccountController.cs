using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolMgt.Models.Helpers;
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

        private readonly AppSettings appSettings;

        public AccountController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, IOptions<AppSettings> options)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            appSettings = options.Value;
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


        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // find the user from database
            var user = await userManager.FindByNameAsync(model.UserName);

            if (user != null) // if user exists
            {
                // check if user's password is correct
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    // todo: check email is confirmed or not

                    // todo: generate token and send the token back to the front end 
                    var userRoles = await userManager.GetRolesAsync(user);
                    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Secret));
                    var tokenHandler = new JwtSecurityTokenHandler();

                    // token description
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Role, userRoles.FirstOrDefault()),
                            new Claim("LoggedOn", DateTime.Now.ToString())

                        }),
                        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                        Issuer = appSettings.Site,
                        Audience = appSettings.Audience,
                        Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(appSettings.ExpTime))
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var finalToken = tokenHandler.WriteToken(token);
                    return Ok(new
                    {
                        jwtToken = finalToken,
                        expiration = token.ValidTo,
                        userName = user.UserName,
                        email = user.Email,
                        role = userRoles.FirstOrDefault()
                    });
                }
                else // user exists but password is wrong
                {
                    return Unauthorized(new { error = $"Password does not match for username = ${user.UserName}! Please provide correct password" });
                }

            }
            return null;
        }
    }
}