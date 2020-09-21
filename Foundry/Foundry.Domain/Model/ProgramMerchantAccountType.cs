using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ProgramMerchantAccountType
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public int? ProgramAccountLinkingId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual ProgramAccountLinking ProgramAccountLinking { get; set; }
    }
}
