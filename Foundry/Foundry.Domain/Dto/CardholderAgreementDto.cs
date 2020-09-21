using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class CardholderAgreementDto
    {
        public int SrNo { get; set; }
        public int CardHolderAgreementId { get; set; }
        public string CardHolderAgreementIdEnc { get; set; }
        public int ProgramId { get; set; }
        public string ProgramIdEnc { get; set; }
        public string cardHoldrAgreementContent { get; set; }
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

    public class UserAgreementHistoryDto
    {
        public int RowNum { get; set; }
        public int UserId { get; set; }
        public string CardHolderAgreementIdEnc { get; set; }
        public string CardHolderName { get; set; }
        public string ProgramIdEnc { get; set; }
        public DateTime DateAccepted { get; set; }
        public string DateAcceptedString { get; set; }
        public string VersionNo { get; set; }
    }
}
