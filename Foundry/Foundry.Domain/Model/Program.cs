using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Program
    {
        public Program()
        {
            BenefactorProgram = new HashSet<BenefactorProgram>();
            Offer = new HashSet<Offer>();
            OrganisationProgram = new HashSet<OrganisationProgram>();
            ProgramAccountLinking = new HashSet<ProgramAccountLinking>();
            ProgramGroup = new HashSet<ProgramGroup>();
            ProgramMerchant = new HashSet<ProgramMerchant>();
            ProgramPackage = new HashSet<ProgramPackage>();
            UserProgram = new HashSet<UserProgram>();
            UserTransactionInfo = new HashSet<UserTransactionInfo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OrganisationId { get; set; }
        public int? LogoPath { get; set; }
        public string ColorCode { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ProgramExpiryTypeId { get; set; }
        public int? ProgramExpiryDuration { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<BenefactorProgram> BenefactorProgram { get; set; }
        public virtual ICollection<Offer> Offer { get; set; }
        public virtual ICollection<OrganisationProgram> OrganisationProgram { get; set; }
        public virtual ICollection<ProgramAccountLinking> ProgramAccountLinking { get; set; }
        public virtual ICollection<ProgramGroup> ProgramGroup { get; set; }
        public virtual ICollection<ProgramMerchant> ProgramMerchant { get; set; }
        public virtual ICollection<ProgramPackage> ProgramPackage { get; set; }
        public virtual ICollection<UserProgram> UserProgram { get; set; }
        public virtual ICollection<UserTransactionInfo> UserTransactionInfo { get; set; }
    }
}
