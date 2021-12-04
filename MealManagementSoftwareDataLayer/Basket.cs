using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class Basket
    {
        public int Id { get; set; }
        public int MealMenuId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime MealAvailableDate { get; set; }

        public int UserProfileId { get; set; }
        public virtual CustomerProfile UserProfile { get; set; }
        public virtual Dish MealMenu { get; set; }
    }
}
