using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MealManagementSoftwareUiLayer.ViewModel
{
    public class AuthenticationViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string Message { get; set; }

        public AuthenticationViewModel()
        {

        }
    }
}
