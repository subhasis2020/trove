using System;

namespace Foundry.Domain.Dto
{
    public class BenefactorUserLinkingDto
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
    }
}
