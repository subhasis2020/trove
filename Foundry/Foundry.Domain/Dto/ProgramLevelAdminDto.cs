using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{

    public class ProgramLevelAdminDto
    {
        public int UserId { get; set; }
        public string UserImagePath { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateAdded { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public bool Status { get; set; }
        public string ProgramsAccessibility { get; set; }
        public string Title { get; set; }
        public bool IsAdmin { get; set; }
        public int InvitationStatus { get; set; }
        public bool EmailConfirmed { get; set; }
    }
    public class ProgramLevelAdminDtoStatusDto
    {
        public int UserId { get; set; }
        public bool isActive { get; set; }
    }

}
