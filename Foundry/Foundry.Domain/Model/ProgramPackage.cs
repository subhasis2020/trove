using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ProgramPackage
    {
        public ProgramPackage()
        {
            BenefactorProgram = new HashSet<BenefactorProgram>();
            UserProgram = new HashSet<UserProgram>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int ProgramId { get; set; }
        public int? NoOfMealPasses { get; set; }
        public int? NoOfFlexPoints { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Program Program { get; set; }
        public virtual ICollection<BenefactorProgram> BenefactorProgram { get; set; }
        public virtual ICollection<UserProgram> UserProgram { get; set; }
    }
}
