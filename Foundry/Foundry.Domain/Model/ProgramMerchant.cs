using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ProgramMerchant
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual Program Program { get; set; }
    }
}
