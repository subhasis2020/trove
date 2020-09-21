using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramBrandingDto
    {
        public string PlanName { get; set; }
        public int PlanId { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public string BrandingImagePath { get; set; }
        public int AccountTypeId { get; set; }
        public string CardNumber { get; set; }
        public string BrandingColor { get; set; }
        public string AccountTypeName { get; set; }
    }
}
