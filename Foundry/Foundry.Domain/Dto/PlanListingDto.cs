using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class PlanListingDto
    {
        public int Id { get; set; }
        public string StrId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public string InternalId { get; set; }
        public string StartEnd { get; set; }
        public string Accounts { get; set; }
        public bool Status { get; set; }
        public string Jpos_PlanId { get; set; }
        public string Jpos_PlanEncId { get; set; }
    }
}
