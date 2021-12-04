using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class DishType
    {
        public DishType()
        {
            MealMenus = new HashSet<Dish>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Dish> MealMenus { get; set; }
    }
}
