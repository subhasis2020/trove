using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class MerchantDetailModel
    {
        public string OrganisationId { get; set; }
        [Required(ErrorMessage = "Please enter merchant name.")]
        public string OrganisationName { get; set; }
        [Required(ErrorMessage = "Please enter address.")]
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        //  [RegularExpression("^(\\+\\d{1}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage = "Please enter a valid phone number in +1 (XXX) XXX-XXXX.")]
        [RegularExpression("^[- +()0-9]+$", ErrorMessage = "Please enter a valid phone number.")]
        public string ContactNumber { get; set; }
        public int PrimaryProgramId { get; set; }
        public string PrimaryProgramName { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid website.")]
        public string Website { get; set; }
        public string Description { get; set; }
        public List<ProgramDto> Program { get; set; }
        public List<AccountTypeDto> AccType { get; set; }
        public List<OrganisationProgramDto> OrgProgram { get; set; }
        public List<AccountTypeDto> OrgAccType { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid facebook link.")]
        public string FacebookURL { get; set; }
        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessage = "Please enter a valid twitter link.")]
        public string TwitterURL { get; set; }
        public string SkypeHandle { get; set; }
        public string InstagramHandle { get; set; }
        public int BusinessTypeId { get; set; }
        public List<BusinessTypeDto> BusinessType { get; set; }
        public bool ShowMap { get; set; }
        public IEnumerable<int> SelectedOrgProgram { get; set; }
        public IEnumerable<int> SelectedOrgAccType { get; set; }
        public IFormFile PostedFileUpload { get; set; }
        public string ImagePath { get; set; }
        public string Location { get; set; }
        public string ImageFileName { get; set; }
        public string PrimaryProgramIdEnc { get; set; }
        public List<OrganisationProgramSelect> OrganisationProgramSelect { get; set; }
        public string Jpos_MerchantId { get; set; }
        public string Jpos_MerchantEncId { get; set; }
    }

    public class OrganisationProgramSelect
    {
        public int ProgramId { get; set; }
        public bool IsPrimaryAssociation { get; set; }
    }
}
