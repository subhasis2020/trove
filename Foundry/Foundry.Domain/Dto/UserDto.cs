using Newtonsoft.Json;
using System;

namespace Foundry.Domain.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int? OrganisationId { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string UserCode { get; set; }
        public int ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string ProgramImagePath { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom3 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom4 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom5 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom6 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom7 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom8 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom9 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom10 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom11 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Custom12 { get; set; }
        public string UserDeviceId { get; set; }
        public string UserDeviceType { get; set; }
        public string SessionId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string ImagePath { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ImageFileName { get; set; }
        public bool? IsLinkedProgram { get; set; }
        public string PartnerUserId { get; set; }
        public int PartnerId { get; set; }
    }
}
