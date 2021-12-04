using System.Collections.Generic;

namespace MealManagementSoftwareDataLayer
{
    public class GardianLink
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public virtual CustomerProfile StudentUserProfile { get; set; }
        public virtual CustomerProfile ParentUserProfile { get; set; }
    }
}
