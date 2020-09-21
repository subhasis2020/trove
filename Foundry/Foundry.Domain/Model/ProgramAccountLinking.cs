using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ProgramAccountLinking
    {
        public ProgramAccountLinking()
        {
            ProgramMerchantAccountType = new HashSet<ProgramMerchantAccountType>();
        }

        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int AccountTypeId { get; set; }

        public virtual AccountType AccountType { get; set; }
        public virtual Program Program { get; set; }
        public virtual ICollection<ProgramMerchantAccountType> ProgramMerchantAccountType { get; set; }
    }
}
