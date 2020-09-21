using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class UserWallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? UserProgramId { get; set; }
        public int AccountTypeId { get; set; }
        public decimal? Value { get; set; }
        public DateTime ExpirationDate { get; set; }

        public virtual AccountType AccountType { get; set; }
        public virtual User User { get; set; }
        public virtual UserProgram UserProgram { get; set; }
    }
}
