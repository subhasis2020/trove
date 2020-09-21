using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class VerifyBankModel
    {
        public string BankName { get; set; }
        [Required(ErrorMessage = "Please enter first amount for verification.")]
        public decimal AmountOne { get; set; }
        [Required(ErrorMessage = "Please enter second amount for verification.")]
        public decimal AmountTwo { get; set; }
    }
}
