using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class PlanProgramAccountModel
    {
        public int id { get; set; }
        public int planId { get; set; }
        public int programAccountId { get; set; }
    }
}
