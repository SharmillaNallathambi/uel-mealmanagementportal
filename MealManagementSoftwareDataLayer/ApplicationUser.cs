using MealManagementSoftwareDataLayer;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealManagementSoftwareDataLayer
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<CustomerProfile> UserProfiles { get; set; }
    }

}
