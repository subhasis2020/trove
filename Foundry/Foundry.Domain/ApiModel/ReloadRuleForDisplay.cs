using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class ReloadRuleForDisplay
    {

        public int id { get; set; }
        public string CreatedbyName { get; set; }
        public decimal? userDroppedAmount { get; set; }
        public decimal reloadAmount { get; set; }
    }
}
