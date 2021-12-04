using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class Offer
    {
        public Offer()
        {
            MealMenus = new HashSet<Dish>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }

        public virtual ICollection<Dish> MealMenus { get; set; }
    }
}
