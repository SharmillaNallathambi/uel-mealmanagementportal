using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class DishAvailability
    {
        public int Id { get; set; }
        public int MealMenuId { get; set; }
        public DateTime AvailabilityDate { get; set; }
        public int Quantity { get; set; }
        public virtual Dish MealMenu { get; set; }
    }
}
