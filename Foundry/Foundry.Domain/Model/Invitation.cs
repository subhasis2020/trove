using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Invitation
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int? InvitationType { get; set; }
        public int? RelationshipId { get; set; }
        public int? ProgramId { get; set; }
        public bool? IsRequestAccepted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
