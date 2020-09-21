using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto.Sodexo
{
    public class SodexoCardHolderAgreementDto
    {
        public int CardHolderAgreementId { get; set; }
        public int ProgramId { get; set; }
        public string CardHolderAgreementContent { get; set; }
        public string versionNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAgreementRead { get; set; }
        public string CardholderAgreementURL { get; set; }
        public int AccountTypeId { get; set; }
        public bool IsDiscretionaryAccountType { get; set; }

    }
}
