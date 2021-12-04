using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MealManagementSoftwareUiLayer.ViewModel
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

  
        [RollnumberRequiredField("IsParent", "IsAdmin", ErrorMessage = "Rollnumber is required and should be greater than 0")]
        public int Rollnumber { get; set; } 
        [ParentRequiredField("IsParent", "IsAdmin", ErrorMessage = "StudentEmail is required")]
        public string StudentEmail { get; set; }
        public bool IsParent { get; set; }
        public bool IsAdmin { get; set; }

        public RegisterUserViewModel()
        {

        }
        public RegisterUserViewModel(bool isAdmin =false)
        {
            IsAdmin = isAdmin;
        }


    }

    public class RollnumberRequiredField : ValidationAttribute
    {
        public RollnumberRequiredField(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames; 
        }

        public string[] PropertyNames { get; private set; } 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = this.PropertyNames.Select(validationContext.ObjectType.GetProperty);
            var isParentProperty = properties.Where(x => x.Name == "IsParent").FirstOrDefault();
            var isParent = (bool)isParentProperty.GetValue(validationContext.ObjectInstance, null);
            var isAdminProperty = properties.Where(x => x.Name == "IsAdmin").FirstOrDefault();
            var isAdmin = (bool)isAdminProperty.GetValue(validationContext.ObjectInstance, null);
            var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null)).OfType<string>();

            if ((value == null || value == string.Empty || (int)value == 0)  &&  (!isAdmin && !isParent))
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            
            return null;
        }
    }

    public class StudentRequiredField : ValidationAttribute
    {
        public StudentRequiredField(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        public string[] PropertyNames { get; private set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = this.PropertyNames.Select(validationContext.ObjectType.GetProperty);
            var isParentProperty = properties.Where(x => x.Name == "IsParent").FirstOrDefault();
            var isParent = (bool)isParentProperty.GetValue(validationContext.ObjectInstance, null);
            var isAdminProperty = properties.Where(x => x.Name == "IsAdmin").FirstOrDefault();
            var isAdmin = (bool)isAdminProperty.GetValue(validationContext.ObjectInstance, null);
            var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null)).OfType<string>();

            if ((value == null || value == string.Empty) && (!isAdmin && !isParent))
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }
    }


    public class ParentRequiredField : ValidationAttribute
    {
        public ParentRequiredField(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        public string[] PropertyNames { get; private set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = this.PropertyNames.Select(validationContext.ObjectType.GetProperty);
            var isParentProperty = properties.Where(x => x.Name == "IsParent").FirstOrDefault();
            var isParent = (bool)isParentProperty.GetValue(validationContext.ObjectInstance, null);
            var isAdminProperty = properties.Where(x => x.Name == "IsAdmin").FirstOrDefault();
            var isAdmin = (bool)isAdminProperty.GetValue(validationContext.ObjectInstance, null);
            var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null)).OfType<string>();

            if ((value == null || value == string.Empty) && (!isAdmin && isParent))
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }
    }
}
