using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class PrivacyDto
    {
        public bool IsOnlyMe { get; set; }
        public List<BenefactorDto> Benefator { get; set; } 
    }
}
