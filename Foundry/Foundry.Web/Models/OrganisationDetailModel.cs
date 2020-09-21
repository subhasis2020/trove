using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class OrganisationDetailModel
    {
        public string OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        [Required(ErrorMessage = "Please enter organization name.")]
        public string OrganisationSubTitle { get; set; }
        [Required(ErrorMessage = "Please enter address.")]
        public string Address { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid website.")]
        public string Website { get; set; }
        [Required(ErrorMessage = "Please enter contact name.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Contact name contains only letters")]
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        [Required(ErrorMessage = "Please enter contact number.")]
        // [RegularExpression("^(\\+\\d{1}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage = "Please enter a valid phone number in +1 (XXX) XXX-XXXX.")]
        [RegularExpression("^[- +()0-9]+$", ErrorMessage = "Please enter a valid phone number.")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "Please enter email address.")]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public List<ProgramTypeIdDto> OrganisationProgramType { get; set; }
        [Required(ErrorMessage = "Please select atleast one program type.")]
        public List<int> SelectedOrganisationProgramType { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid facebook link.")]
        public string FacebookURL { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid twitter link.")]
        public string TwitterURL { get; set; }
        public string SkypeHandle { get; set; }
        public string JPOS_MerchantId { get; set; }
    }
}
