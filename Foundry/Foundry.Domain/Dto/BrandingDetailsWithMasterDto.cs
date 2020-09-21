using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class BrandingDetailsWithMasterDto
    {
        public int id { get; set; }
        public int programAccountID { get; set; }
        public string accountName { get; set; }
        public int? programId { get; set; }
        public string brandingColor { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public int accountTypeId { get; set; }
        public string accountType { get; set; }
        public List<ProgramAccountDto> programAccount { get; set; }
        public string cardNumber { get; set; }
        public string ImagePath { get; set; }
        public string ImageFileName { get; set; }
    }

    public class BrandingDetailsWithAccountType
    {
        public int id { get; set; }
        public int programAccountID { get; set; }
        public string accountName { get; set; }
        public int? programId { get; set; }
        public string brandingColor { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public int accountTypeId { get; set; }
        public string accountType { get; set; }
        public string cardNumber { get; set; }
        public string ImagePath { get; set; }
        public string ImageFileName { get; set; }
    }
}
