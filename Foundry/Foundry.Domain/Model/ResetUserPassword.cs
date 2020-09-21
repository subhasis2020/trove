using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ResetUserPassword
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? InviteeId { get; set; }
        public string ResetToken { get; set; }
        public DateTime? ValidTill { get; set; }
        public bool? IsPasswordReset { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual User User { get; set; }
    }
}
