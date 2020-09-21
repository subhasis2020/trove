using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
   public class ReloadRulesDto
    {
        public int ReloadUserId { get; set; }
        public decimal? ReloadAmount { get; set; }
        public bool IsAutoReload { get; set; }
        public int AccountTypeId { get; set; }
        public int BenefactorUserId { get; set; }
        public decimal? AutoReloadAmount { get; set; }
        public decimal CheckDroppedAmount { get; set; }
        public  string i2cBankAccountId { get; set; }     
        public  string CardId { get; set; }
        public string schemetransactionID { get; set; }
        public int programId { get; set; }

    }
}
