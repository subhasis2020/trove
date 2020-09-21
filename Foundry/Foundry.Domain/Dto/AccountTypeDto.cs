using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class AccountTypeDto
    {
        public int Id { get; set; }
        public string AccountType { get; set; }
        public string Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int ProgramId { get; set; }
    }
    public class OrganisationAccountTypeDto
    {
        public int Id { get; set; }
        public string AccountType { get; set; }
        public string Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int ProgramId { get; set; }
    }
}
