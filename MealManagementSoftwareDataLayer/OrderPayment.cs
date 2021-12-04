using System;
using System.Collections.Generic;

#nullable disable

namespace MealManagementSoftwareDataLayer
{
    public partial class OrderPayment
    {
        public long Id { get; set; }
        public string PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public long OrderId { get; set; }

        public virtual DishOrder Order { get; set; }
    }
}
