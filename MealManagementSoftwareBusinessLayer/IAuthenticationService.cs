
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareUiLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealManagementSoftwareBusinessLayer
{
   

    public interface IAuthenticationService
    {
        List<CustomerProfile> GetUserDetails();
    }
}
