using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class ProgramLevelAdminDetailModel
    {
        public AdminLevelModel adminLevelModel { get; set; }
        [Required(ErrorMessage = "Please select atleast one program type.")]
        public List<ProgramTypeIdDto> ProgramsAccessibility { get; set; }

        public List<AdminProgramIdDto> AdminProgramAccessibility { get; set; }
        public string ProgramId { get; set; }
        public string Custom1 { get; set; }
    }
}
