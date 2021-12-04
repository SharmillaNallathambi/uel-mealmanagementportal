using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareUiLayer.ViewModel;
using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MealManagementSoftwareUiLayer.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly MealManagementDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthenticationController(MealManagementDbContext context,

            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        [Route("login")]
        public IActionResult Authentication(string returnUrl = null,string message= null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new AuthenticationViewModel();
            model.Message = message;
            return View(model);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Authentication(AuthenticationViewModel model, string returnUrl = null)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userProfile = _context.UserProfiles.Where(x => x.EmailAddress == model.Username)
                    .FirstOrDefault();
                if (userProfile == null)
                {
                    ModelState.AddModelError("", "Something wrong with system. please try again later.");
                    return View();
                }
                
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.UserData, userProfile.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userProfile.Name)); 
                    var userRoles = await userManager.GetRolesAsync(user);

                    var role = userRoles.FirstOrDefault();
                    if (role != null)
                    {
                        identity.AddClaim(new Claim(JwtClaimTypes.Role, role));
                    }
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                        new ClaimsPrincipal(identity));
                    return RedirectToLocal(returnUrl); 
                 
            }
            ModelState.AddModelError("", "Invalid UserName or Password");
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        [Route("register")]
        public IActionResult RegisterUser()
        {
            return View(new RegisterUserViewModel());
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userExists = await userManager.FindByNameAsync(model.Email);
            var studentId = 0;
            if (!model.IsParent)
            {
                var existingRoleNumber = _context.UserProfiles.Where(x => x.RollNumber == model.Rollnumber).FirstOrDefault();
              
                if (userExists != null   || existingRoleNumber != null)
                {
                    ModelState.AddModelError("error", "User already exists!");

                    return View("RegisterUser", model);
                }
            }
            else
            {
                var studentEmailAddress = _context.UserProfiles.Where(x => x.EmailAddress == model.StudentEmail).FirstOrDefault();
                if (studentEmailAddress == null)
                {
                    ModelState.AddModelError("error", "The student email address doesnt exist");

                    return View("RegisterUser", model);
                }
                studentId = studentEmailAddress.Id;
            }
            var applicationUser = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await userManager.CreateAsync(applicationUser, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("error", "User creation failed! Please check user details and try again.");

                return View("RegisterUser", model);
            }

            var newlyCreatedUser = await userManager.FindByNameAsync(model.Email);
            if (newlyCreatedUser != null)
            {

                var user = new CustomerProfile()
                {
                    Name = model.Firstname + " " + model.Lastname,
                    EmailAddress = model.Email,
                    Department =null,
                    RollNumber = model.Rollnumber,
                    ApplicationUserId = newlyCreatedUser.Id
                };
                Random generator = new Random();
                int r = generator.Next(100000, 1000000);

                _context.Add(user);
                _context.SaveChanges();

                if (!model.IsParent)
                {
                    var card = new PaymentCard()
                    {
                        CardNumber = $"CN{r}SM",
                        IsActive = true,
                        AvailableBalance = 0,
                        UserProfileId = user.Id
                    };
                    _context.Add(card);
                }
                else
                {

                    var parentMapping = new GardianLink()
                    {
                        StudentId = studentId,
                        ParentId = user.Id
                    };
                    _context.Add(parentMapping);

                }
                _context.SaveChanges();
            }
            if (!model.IsParent)
            {
                if (!await roleManager.RoleExistsAsync(AvailableUserRoles.StudentRole))
                    await roleManager.CreateAsync(new IdentityRole(AvailableUserRoles.StudentRole));

                if (await roleManager.RoleExistsAsync(AvailableUserRoles.StudentRole))
                {
                    await userManager.AddToRoleAsync(newlyCreatedUser, AvailableUserRoles.StudentRole);
                }
            }
            else
            {
                if (!await roleManager.RoleExistsAsync(AvailableUserRoles.ParentsRole))
                    await roleManager.CreateAsync(new IdentityRole(AvailableUserRoles.ParentsRole));

                if (await roleManager.RoleExistsAsync(AvailableUserRoles.ParentsRole))
                {
                    await userManager.AddToRoleAsync(newlyCreatedUser, AvailableUserRoles.ParentsRole);
                }
            }
            return RedirectToAction(nameof(Index),"Home", new { message="User has been created successfully"});  
        }

        [Route("register/admin")]
        public IActionResult RegisterAdminUser()
        {
            return View("RegisterUser", new RegisterUserViewModel(true));
        }


        [HttpPost]
        [Route("register/admin")]
        public async Task<IActionResult> RegisterAdminUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsAdmin = true;
                return View(model);
            }
            var userExists = await userManager.FindByNameAsync(model.Email);
             
            if (userExists != null)
            {
                ModelState.AddModelError("error", "User already exists!");
                model.IsAdmin = true;
                return View("RegisterUser", model);
            }
            var applicationUser = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await userManager.CreateAsync(applicationUser, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("error", "User creation failed! Please check user details and try again.");
                model.IsAdmin = true;
                return View("RegisterUser", model);
            }

            var newlyCreatedUser = await userManager.FindByNameAsync(model.Email);
            if (newlyCreatedUser != null)
            {

                var user = new CustomerProfile()
                {
                    Name = model.Firstname + " " + model.Lastname,
                    EmailAddress = model.Email,
                    Department = null,
                    RollNumber = model.Rollnumber,
                    IsVerified = !model.IsParent,
                    ApplicationUserId = newlyCreatedUser.Id
                };
                _context.Add(user);
                _context.SaveChanges();
            }

            if (!await roleManager.RoleExistsAsync(AvailableUserRoles.AdminRole))
                await roleManager.CreateAsync(new IdentityRole(AvailableUserRoles.AdminRole));

            if (await roleManager.RoleExistsAsync(AvailableUserRoles.AdminRole))
            {
                await userManager.AddToRoleAsync(newlyCreatedUser, AvailableUserRoles.AdminRole);
            }


            return RedirectToAction(nameof(Index),"Home", new
            {
                message = "Admin User has been created and please login using your username and password."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

