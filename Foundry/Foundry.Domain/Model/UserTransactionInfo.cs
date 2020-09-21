using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class UserTransactionInfo
    {
        public int Id { get; set; }
        public int DebitUserId { get; set; }
        public int? CreditUserId { get; set; }
        public int? AccountTypeId { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string PeriodRemark { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? ProgramId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual AccountType AccountType { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User CreditUser { get; set; }
        public virtual User DebitUser { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Program Program { get; set; }
    }
}
