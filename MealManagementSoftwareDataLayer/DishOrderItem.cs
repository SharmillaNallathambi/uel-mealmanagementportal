using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class DishOrderItem
    {
        public long Id { get; set; }
        public int MealMenuId { get; set; }
        public DateTime MealMenuOrderDate { get; set; }
        public long OrderId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public virtual Dish MealMenu { get; set; }
        public virtual DishOrder Order { get; set; }
    }
}
