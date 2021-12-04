using MealManagementSoftwareDataLayer;
using System.Collections.Generic;

namespace MealManagementSoftwareDataLayer
{
    public class CustomerProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int RollNumber { get; set; }
        public string Department { get; set; }
        public bool IsVerified { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ICollection<DishOrder> Orders { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<GardianLink> StudentUserProfiles { get; set; }
        public virtual ICollection<GardianLink> ParentUserProfiles { get; set; }
        public virtual ICollection<PaymentCard> Cards { get; set; }
        public virtual ICollection<Basket> Carts { get; set; }

    }


    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int RollNumber { get; set; }
        public string Department { get; set; }
        public bool IsVerified { get; set; }
        public string ApplicationUserId { get; set; }
        public string Role { get; set; }
    }
}
