using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ReloadRules
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BenefactorUserId { get; set; }
        public bool? IsAutoReloadAmount { get; set; }
        public decimal? UserDroppedAmount { get; set; }
        public decimal? ReloadAmount { get; set; }
        public int? ProgramId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User BenefactorUser { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
