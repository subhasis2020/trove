using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class BenefactorUsersLinking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BenefactorId { get; set; }
        public int? RelationshipId { get; set; }
        public bool? IsRequestAccepted { get; set; }
        public bool? IsInvitationSent { get; set; }
        public DateTime? LinkedDateTime { get; set; }
        public bool? CanViewTransaction { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User Benefactor { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual UserRelations Relationship { get; set; }
        public virtual User User { get; set; }
    }
}
