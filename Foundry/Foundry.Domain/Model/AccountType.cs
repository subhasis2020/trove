using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class AccountType
    {
        public AccountType()
        {
            ProgramAccountLinking = new HashSet<ProgramAccountLinking>();
            UserTransactionInfo = new HashSet<UserTransactionInfo>();
            UserWallet = new HashSet<UserWallet>();
        }

        public int Id { get; set; }
        public string AccountType1 { get; set; }
        public string Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual ICollection<ProgramAccountLinking> ProgramAccountLinking { get; set; }
        public virtual ICollection<UserTransactionInfo> UserTransactionInfo { get; set; }
        public virtual ICollection<UserWallet> UserWallet { get; set; }
    }
}
