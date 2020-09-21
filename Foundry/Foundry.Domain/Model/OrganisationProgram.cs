using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OrganisationProgram
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public int? ProgramId { get; set; }

        public virtual Program Program { get; set; }
    }
}
