using System;

namespace Foundry.Domain.Dto
{
    public class UserProgramDto
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int programId { get; set; }
        public int? programPackageId { get; set; }
        public string userEmailAddress { get; set; }
        public bool? isLinkedProgram { get; set; }
        public string linkAccountVerificationCode { get; set; }
        public DateTime? verificationCodeValidTill { get; set; }
        public bool? isVerificationCodeDone { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
    }
}
