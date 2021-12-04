using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareUiLayer.ViewModel;
using System.Security.Claims;
using IdentityModel;

namespace MealManagementSoftwareUiLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MealManagementDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public HomeController(ILogger<HomeController> logger, MealManagementDbContext context,
                 UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var signedUserId=User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
            var homeModel = new HomeModel();
            if (signedUserId != null)
            {
                if (User.Claims.Any(c => c.Type == JwtClaimTypes.Role && c.Value == "RoleOfParents"))
                {
                    var parentDetail =  _context.ParentMapping
                          .Include(c => c.StudentUserProfile)
                          .FirstOrDefault(m => m.ParentId.ToString() == signedUserId.Value);

                    if (parentDetail == null)
                    {
                        return NotFound();
                    }

                    var card =  _context.Cards
                        .Include(c => c.UserProfile)
                        .FirstOrDefault(m => m.UserProfileId == parentDetail.StudentId);
                    if (card != null)
                    {
                        homeModel.AvailableBalance = card.AvailableBalance;
                    }
                }
                else if(User.Claims.Any(c => c.Type == JwtClaimTypes.Role && c.Value == "RoleOfStudent"))
                {
                    var userCardDetail = _context.Cards.Where(x => x.UserProfileId.ToString() == signedUserId.Value).FirstOrDefault();
                    if (userCardDetail != null)
                    {
                        homeModel.AvailableBalance = userCardDetail.AvailableBalance;
                    }
                }
                
            }

            
            return View(homeModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Route("ShowUserProfile/{id}")]
        public async Task<IActionResult> ShowUserProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
            {
                return NotFound();
            }
            var userRole = await userManager.GetRolesAsync(userProfile.ApplicationUser);
            return View(ConvertUserToUserProfile(userProfile, userRole?.First()));
        }


        [Route("EditUserProfile/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditUserProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.ApplicationUserId);
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("EditUserProfile/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserProfile(int id, [Bind("Id,Name,EmailAddress,RollNumber,Department,IsVerified,ApplicationUserId")] CustomerProfile userProfile)
        {
            if (id != userProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProfile);
                    await _context.SaveChangesAsync();

                    var applicationUser = await userManager.FindByIdAsync(userProfile.ApplicationUserId);
                    if (applicationUser != null)
                    {
                        applicationUser.UserName = userProfile.EmailAddress;
                        await userManager.UpdateAsync(applicationUser);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(userProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.ApplicationUserId);
            return View(userProfile);
        }


        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.Id == id);
        }
        private UserModel ConvertUserToUserProfile(CustomerProfile user, string role)
        {


            return new UserModel
            {
                ApplicationUserId = user.ApplicationUserId,
                Department = user.Department,
                EmailAddress = user.EmailAddress,
                IsVerified = user.IsVerified,
                Name = user.Name,
                RollNumber = user.RollNumber,
                Role = role,
                Id = user.Id
            };
        }
    }

}
