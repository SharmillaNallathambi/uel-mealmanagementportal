using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class Dish
    {
        public Dish()
        {
            Carts = new HashSet<Basket>();
            MealMenuAvailabilities = new HashSet<DishAvailability>();
            OrderItems = new HashSet<DishOrderItem>();
        }

        public int Id { get; set; }
        [DisplayName("Dish name")]
        public string MealName { get; set; } 
        public int MealTypeId { get; set; }
        [DisplayName("Dish cost")]
        public double Price { get; set; }
      
        public int? DiscountId { get; set; }
        [DisplayName("Dish Image name")]
        public string ImageName { get; set; }
        [DisplayName("Dish offer")]
        public virtual Offer Discount { get; set; }
        [DisplayName("Dish type")]
        public virtual DishType MealType { get; set; }
        public virtual ICollection<Basket> Carts { get; set; }
        public virtual ICollection<DishAvailability> MealMenuAvailabilities { get; set; }
        public virtual ICollection<DishOrderItem> OrderItems { get; set; }
    }
}
