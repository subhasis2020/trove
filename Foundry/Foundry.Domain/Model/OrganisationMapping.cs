using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OrganisationMapping
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public int? ParentOrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual Organisation ParentOrganisation { get; set; }
    }
}
