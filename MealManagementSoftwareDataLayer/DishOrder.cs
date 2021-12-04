using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class DishOrder
    {
        public DishOrder()
        {
            OrderItems = new HashSet<DishOrderItem>();
            Payments = new HashSet<OrderPayment>();
        }

        public long Id { get; set; }
        public string OrderReference { get; set; }
        public int UserProfileId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual CustomerProfile UserProfile { get; set; }
        public virtual ICollection<DishOrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderPayment> Payments { get; set; }
    }
}
