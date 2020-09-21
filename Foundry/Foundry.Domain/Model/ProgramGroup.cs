using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ProgramGroup
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int? GroupId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Program Program { get; set; }
    }
}
