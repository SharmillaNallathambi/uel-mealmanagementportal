using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MealManagementSoftwareUiLayer.ViewModel
{
    public class PaymentCardViewModel
    {
        public int UserProfileId { get; set; }
        public int CardId { get; set; }
        
        [Required]
      
        public int BankCardNumber { get; set; }
        [Required]
        [Range(01, 12,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
       
        public int ExpiryMonth { get; set; }
        [Required]
        [Range(2020, 2050,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        
        public int ExpiryYear { get; set; }
        [Required] 
        public decimal Amount { get; set; }
        [Required]
        
        public int CVV { get; set; }
    }
}
