using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class AccountHolderDto
    {
        public int RowNumber { get; set; }
        public int TotalCount { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? DateAdded { get; set; }
        public string AccountHolderID { get; set; }
        public string UserImagePath { get; set; }
        public int Status { get; set; }
        public int InvitationStatus { get; set; }        
        public string PlanName { get; set; }
        public string UserEncId { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondaryEmail { get; set; }
        public int GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string UserCustomJsonValue { get; set; }
        public List<PlanIdDto> planIds { get; set; }
        public string ImageFileName { get; set; }
        public string Jpos_AccountHolderId { get; set; }
        public string Jpos_AccountEncId { get; set; }
        public string PartnerUserId { get; set; }
        public string i2cReferenceId { get; set; }

        public string ProgramId { get; set; }
    }
}
