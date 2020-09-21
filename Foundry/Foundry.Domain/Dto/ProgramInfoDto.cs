using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramInfoDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int organisationId { get; set; }
        public int? logoPath { get; set; }
        public string colorCode { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public string timeZone { get; set; }
        public string ProgramCodeId { get; set; }
        public int? ProgramTypeId { get; set; }
        public string website { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public string customName { get; set; }
        public string customInputMask { get; set; }
        public string customErrorMessaging { get; set; }
        public string customInstructions { get; set; }
        public string programCustomFields { get; set; }
        public string AccountHolderGroups { get; set; }
        public string AccountHolderUniqueId { get; set; }
        public bool IsAllNotificationShow { get; set; } = true;
        public bool IsRewardsShowInApp { get; set; } = true;
        public string JPOS_IssuerId { get; set; }
        public string OrganisationJPOSId { get; set; }
    }
}
