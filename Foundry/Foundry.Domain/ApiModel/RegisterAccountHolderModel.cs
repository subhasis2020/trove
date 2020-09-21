using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class RegisterAccountHolderModel
    {
        public int Id { get; set; }
        public string userEncId { get; set; }
        public string UserImagePath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountHolderUniqueId { get; set; }
        public string PhoneNumber { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public int GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CustomFields { get; set; }
        public List<int> PlanIds { get; set; }
        public int ProgramId { get; set; }
        public int OrganizationId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public bool? IsMobileRegistered { get; set; }
        public int? InvitationStatus { get; set; }
        public string Jpos_AccountHolderId { get; set; }
    }
}
