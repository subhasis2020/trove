using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class UserProgramViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int ProgramId { get; set; }
        public int ProgramPackageId { get; set; }
        public string UserEmailAddress { get; set; }
        public bool? IsLinkedProgram { get; set; }
        public string LinkAccountVerificationCode { get; set; }
        public DateTime? VerificationCodeValidTill { get; set; }
        public bool? IsVerificationCodeDone { get; set; }

    }
}
