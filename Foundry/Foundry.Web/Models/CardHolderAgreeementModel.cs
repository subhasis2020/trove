using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class CardHolderAgreeementModel
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string ProgramIdEnc { get; set; }
        public string CardHolderAgreementId { get; set; }
        [Required(ErrorMessage = "Please enter cardholder agreement content.")]
        public string CardHolderAgreementContent { get; set; }
        public string VersionNo { get; set; }
    }
}
