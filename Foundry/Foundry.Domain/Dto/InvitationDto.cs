using System;

namespace Foundry.Domain.Dto
{
    public class InvitationDto
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int? InvitationType { get; set; }
        public int? relationshipId { get; set; }
        public int? programId { get; set; }
        public bool? IsRequestAccepted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
