using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class BenefactorProgram
    {
        public int Id { get; set; }
        public int BenefactorId { get; set; }
        public int ProgramId { get; set; }
        public int? ProgramPackageId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User Benefactor { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Program Program { get; set; }
        public virtual ProgramPackage ProgramPackage { get; set; }
    }
}
