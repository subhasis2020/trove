using Foundry.Domain.ApiModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class MerchantDto
    {
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string Location { get; set; }
        public string LastTransaction { get; set; }
        public DateTime DateAdded { get; set; }
        public string AccountType { get; set; }
        public int Activity { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid number.")]
        public int MaxCapacity { get; set; }
        public int DwellTime { get; set; }
        public bool IsClosed { get; set; }
        public string Jpos_MerchantId { get; set; }
        public string Jpos_MerchantEncId { get; set; }
    }

    public class PrimaryMerchantDetail
    {
        public int PrimaryOrganisationId { get; set; }
        public string PrimaryOrganisationIdEnc { get; set; }
        public string PrimaryOrgName { get; set; }
        public string PrimaryOrgSubtitle { get; set; }
        public int PrimaryProgramId { get; set; }
        public string PrimaryProgramIdEnc { get; set; }
        public string PrimaryProgramName { get; set; }
        public int TotalMerchantInAdmin { get; set; }
        public int TotalProgramInAdmin { get; set; }
        public int MerchantId { get; set; }
    }
    public class MerchantBusinessDto
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

        [RegularExpression("^[0-9]*$", ErrorMessage = "Max capacity contains numbers only.")]
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
        public bool isClosed { get; set; }
        public int? businessTypeId { get; set; }
        public string OrganisationSubTitle { get; set; }
        public List<OrganisationProgramTypeModel> OrganisationProgramTypes { get; set; }
        public List<OrganisationProgramModel> OrganisationProgram { get; set; }
        public List<OrganisationAccTypeModel> OrganisationAccountType { get; set; }
        public int programId { get; set; }
        public string ImagePath { get; set; }
        public int? dwellTime { get; set; }
        public bool? isTrafficChartVisible { get; set; } = true;
    }
    public class DwellTimeDto
    {
        public int Id { get; set; }
        public string Time { get; set; }
    }
    public class TerminalTypeDto
    {
        public int Id { get; set; }
        public string TerminalType { get; set; }
    }
}
