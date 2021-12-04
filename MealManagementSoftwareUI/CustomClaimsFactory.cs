using MealManagementSoftwareDataLayer;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MealManagementSoftwareUiLayer
{
    public class CustomClaimsFactory
         : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {

        private readonly UserManager<ApplicationUser> _userManager;
        public CustomClaimsFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { _userManager = userManager; }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();
            var userRoles = await _userManager.GetRolesAsync(user);

            var role = userRoles.FirstOrDefault();
            if (role != null)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
            claims.Add(new Claim("UserName", user.UserName));
            identity.AddClaims(claims);
            return principal;
        }


    }

}
