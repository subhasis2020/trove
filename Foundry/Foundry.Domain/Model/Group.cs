using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Group
    {
        public Group()
        {
            OfferGroup = new HashSet<OfferGroup>();
            OrganisationGroup = new HashSet<OrganisationGroup>();
            ProgramGroup = new HashSet<ProgramGroup>();
            UserGroup = new HashSet<UserGroup>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GroupType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual ICollection<OfferGroup> OfferGroup { get; set; }
        public virtual ICollection<OrganisationGroup> OrganisationGroup { get; set; }
        public virtual ICollection<ProgramGroup> ProgramGroup { get; set; }
        public virtual ICollection<UserGroup> UserGroup { get; set; }
    }
}
