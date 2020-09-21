using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class ProgramBrandingModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please select account name.")]
        public int programAccountID { get; set; }
        public string accountName { get; set; }
        public string programId { get; set; }
        public string brandingColor { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public int? accountTypeId { get; set; }
        public List<ProgramAccountDto> programAccount { get; set; }
        public string cardNumber { get; set; }
        public string accountType { get; set; }
        public string PostedFileUpload { get; set; }
        [Required(ErrorMessage = "Please chose a logo.")]
        public string ImagePath { get; set; }
        public string ImageFileName { get; set; }
    }
}
