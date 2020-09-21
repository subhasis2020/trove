using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Foundry.Web.Models
{
    public class ProgramInfoModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please enter program name.")]
        public string name { get; set; }
        public string AccountHolderGroups { get; set; }
        [Required(ErrorMessage = "Please select program id.")]
        public string ProgramCodeId { get; set; }
        [Required(ErrorMessage = "Please select time zone.")]
        public string timeZone { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid website.")]
        public string website { get; set; }
        [Required(ErrorMessage = "Please enter address.")]
        public string address { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        [Required(ErrorMessage = "Please enter account holder unique id.")]
        public string AccountHolderUniqueId { get; set; }
        [Required(ErrorMessage = "Please enter custom name.")]
        public string customName { get; set; }
        [Required(ErrorMessage = "Please enter custom input mask.")]
        [RegularExpression(@"^[a-zA-Z0-9_\- #]+$", ErrorMessage = "Input mask contains letters, numbers and some special characters like '_, -, #'")]
        public string customInputMask { get; set; }
        [Required(ErrorMessage = "Please enter custom error messaging.")]
        public string customErrorMessaging { get; set; }
        public string customInstructions { get; set; }
        public string programCustomFields { get; set; }
        [Required(ErrorMessage = "Please select program type.")]
        public int? ProgramTypeId { get; set; }
        public int organisationId { get; set; }
        public string OrganizationEncId { get; set; }
        public string ProgramEncId { get; set; }
        public bool IsAllNotificationShow { get; set; }
        public bool IsRewardsShowInApp { get; set; }
        public string JPOS_IssuerId { get; set; }
        public List<IssuerProp> IssuerProps { get; set; }
    }
    public class IssuerProp
    {
        public IssuerProp(string _key, string _value)
        {
            Key = _key;
            Value = _value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
