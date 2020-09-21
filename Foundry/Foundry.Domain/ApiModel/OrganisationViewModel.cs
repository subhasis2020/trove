using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class OrganisationViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string location { get; set; }
        public int organisationType { get; set; }
        public string emailAddress { get; set; }
        public string contactNumber { get; set; }
        public bool? isMaster { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public int? maxCapacity { get; set; }
        public string description { get; set; }
        public string websiteURL { get; set; }
        public string getHelpEmail { get; set; }
        public string getHelpPhone1 { get; set; }
        public string getHelpPhone2 { get; set; }
        public string facebookURL { get; set; }
        public string twitterURL { get; set; }
        public string skypeHandle { get; set; }
        public string InstagramHandle { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string MerchantId { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public bool? isMapVisible { get; set; }
        public bool? isClosed { get; set; }
        public int? businessTypeId { get; set; }
        public string OrganisationSubTitle { get; set; }
        public List<OrganisationProgramTypeModel> OrganisationProgramTypes { get; set; }
        public List<OrganisationProgramModel> OrganisationProgram { get; set; }
        public List<OrganisationAccTypeModel> OrganisationAccountType { get; set; }
        public int programId { get; set; }
        public string ImagePath { get; set; }
        public int? dwellTime { get; set; }
        public bool? isTrafficChartVisible { get; set; }
        public string JPOS_OrgId { get; set; }   // This will also work for JPOS_OrgId
    }
}
