using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class UserProgram
    {
        public UserProgram()
        {
            UserWallet = new HashSet<UserWallet>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProgramId { get; set; }
        public int? ProgramPackageId { get; set; }
        public string UserEmailAddress { get; set; }
        public bool? IsLinkedProgram { get; set; }
        public string LinkAccountVerificationCode { get; set; }
        public DateTime? VerificationCodeValidTill { get; set; }
        public bool? IsVerificationCodeDone { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Program Program { get; set; }
        public virtual ProgramPackage ProgramPackage { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserWallet> UserWallet { get; set; }
    }
}
