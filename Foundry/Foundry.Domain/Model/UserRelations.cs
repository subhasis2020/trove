using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class UserRelations
    {
        public UserRelations()
        {
            BenefactorUsersLinking = new HashSet<BenefactorUsersLinking>();
        }

        public int Id { get; set; }
        public string RelationName { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<BenefactorUsersLinking> BenefactorUsersLinking { get; set; }
    }
}
