using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.ApiModel.PartnerApiModel
{
  public  class RequestBalanceReloadModel
    {
       
        [Required]
        public int BenefactorId { get; set; }
        public decimal Amount { get; set; }

        public string Message { get; set; }
    }
}
