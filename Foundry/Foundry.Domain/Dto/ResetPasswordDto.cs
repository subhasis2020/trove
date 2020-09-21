using System;

namespace Foundry.Domain.Dto
{
    public class ResetPasswordDto
    {
        public int Id { get; set; }
        public int? userId { get; set; }
        public int? inviteeId { get; set; }
        public string resetToken { get; set; }
        public DateTime? validTill { get; set; }
        public bool? isPasswordReset { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? updatedDate { get; set; }
    }
}
