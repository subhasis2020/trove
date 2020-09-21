using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Role
    {
        public Role()
        {
            RoleClaim = new HashSet<RoleClaim>();
            UserRole = new HashSet<UserRole>();
        }

        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual ICollection<RoleClaim> RoleClaim { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
