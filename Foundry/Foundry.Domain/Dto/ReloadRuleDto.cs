using System;

namespace Foundry.Domain.Dto
{
    public class ReloadRuleDto
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int? benefactorUserId { get; set; }
        public bool? isAutoReloadAmount { get; set; }
        public decimal? userDroppedAmount { get; set; }
        public decimal? reloadAmount { get; set; }
        public int? programId { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public string i2cBankAccountId { get; set; }
        public string CardId { get; set; }
    }
}
