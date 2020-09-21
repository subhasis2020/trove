using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramAccountDetailDto
    {
        public int ProgramId { get; set; }
        public int PlanId { get; set; }
        public int ProgramAccountId { get; set; }
        public string AccountName { get; set; }
        public decimal InitialBalance { get; set; }
        public int AccountTypeId { get; set; }
    }
}
