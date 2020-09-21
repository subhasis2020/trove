using System;

namespace Foundry.Domain.ApiModel
{
    public class ResetUserPasswordModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ResetToken { get; set; }
        public DateTime? ValidTill { get; set; }
        public bool? IsPasswordReset { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
