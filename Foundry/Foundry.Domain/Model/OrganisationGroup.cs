using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OrganisationGroup
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public int? GroupId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
