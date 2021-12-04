using System;
namespace MealManagementSoftwareUiLayer.ViewModel
{
    public class ProductOrderConfirmViewModel
    {
        public ProductOrderConfirmViewModel(string message)
        {
            NotificationMessage = message;
        }

        public string NotificationMessage { get; set; }
        public string Status { get; set; }

    }
}