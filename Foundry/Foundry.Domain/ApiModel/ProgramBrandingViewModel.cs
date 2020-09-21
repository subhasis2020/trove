using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class ProgramBrandingViewModel
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
        public string cardNumber { get; set; }
        public string ImagePath { get; set; }
    }
}
