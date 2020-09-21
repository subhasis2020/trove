using System;

namespace Foundry.Domain.Dto
{
    public class BenefactorDto
    {
        public int Id { get; set; }
        public string BenefactorImage { get; set; }
        public int BenefactorUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RelationshipName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool IsInvitee { get; set; }
        public bool IsReloadRequest { get; set; }
        public DateTime? CreationDate{ get; set; }
        public bool CanViewTransaction { get; set; }
    }
}
