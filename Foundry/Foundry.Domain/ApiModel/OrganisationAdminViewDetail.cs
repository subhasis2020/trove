using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class OrganisationAdminViewDetail
    {
        public int UserId { get; set; }
        public string UserImagePath { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public List<OrganisationProgramIdModel> ProgramsAccessibility { get; set; }
        public List<MerchantIdModel> MerchantAccessibility { get; set; }
        public int OrganisationId { get; set; }
        public string Custom1 { get; set; }
    }
}
