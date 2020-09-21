using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramLinkingNCardholderAgreement
    {
        public ProgramLinking ProgramLinking{ get; set; }
        public CardholderAgreementDto CardholderAgreement { get; set; }
        public bool IsUserVirtualCardExists { get; set; }
    }

    public class ProgramLinking {
        public bool IsProgramLinked { get; set; }
    }

   
}
