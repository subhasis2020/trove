using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class ProgramLevelAdminViewDetail
    {
        public int UserId { get; set; }
        public string UserImagePath { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public List<ProgramLevelAdminTypeModel> ProgramsAccessibility { get; set; }
        public int ProgramId { get; set; }
        public string Custom1 { get; set; }
        public List<ProgramIdModel> ProgramAdminAccessibility { get; set; }
    }
}
