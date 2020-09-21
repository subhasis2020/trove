using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class OrganisationAdminDetailModel
    {
        public AdminLevelModel adminLevelModel { get; set; }
        [Required(ErrorMessage = "Please select atleast one program type.")]
        public List<ProgramIdDto> ProgramsAccessibility { get; set; }
       // [Required(ErrorMessage = "Please select merchant(s).")]
       // public List<MercahntIdDto> MerchantAccessibility { get; set; }
        public string OrganisationId { get; set; }
        public string Custom1 { get; set; }
    }

    public class MerchantAdminDetailModel
    {
        public AdminLevelModel adminLevelModel { get; set; }
       // [Required(ErrorMessage = "Please select atleast one program type.")]
       // public List<ProgramIdDto> ProgramsAccessibility { get; set; }
        [Required(ErrorMessage = "Please select merchant(s).")]
        public List<MercahntIdDto> MerchantAccessibility { get; set; }
        public string OrganisationId { get; set; }
        public string Custom1 { get; set; }
    }
}
