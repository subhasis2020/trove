using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.ApiModel.PartnerApiModel
{
  public  class PartnerBenefactorRegisterModel
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Email length must be between 100 characters long.")]
        public string EmailAddress { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "PhoneNumber length must be between 20 characters long.")]
        public string MobileNumber { get; set; }
        [Required]
        public int RelationshipId { get; set; }
    }
}
