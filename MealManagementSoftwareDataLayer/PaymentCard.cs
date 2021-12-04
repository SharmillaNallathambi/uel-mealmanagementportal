using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealManagementSoftwareDataLayer
{
    public class PaymentCard
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public decimal AvailableBalance { get; set; }
        public bool IsActive { get; set; }
        public int UserProfileId { get; set; }
        public virtual CustomerProfile UserProfile { get; set; }
    }
}
