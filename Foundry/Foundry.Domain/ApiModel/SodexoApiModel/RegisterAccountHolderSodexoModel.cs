using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class RegisterAccountHolderSodexoModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }   
        [Required]
        [EmailAddress]
        [Display(Name = "PrimaryEmail")]
        [StringLength(80, ErrorMessage = "PrimaryEmail address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid PrimaryEmail address.")]
        public string PrimaryEmail { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        //public List<int> PlanIds { get; set; }
        [Required]
        public string ProgramIssuerId { get; set; }
        public string DeviceId{ get; set; }
        public string DeviceType { get; set; }
        [Required]
        public string PartnerUserId { get; set; }
        [Required]
        public int PartnerId { get; set; }
    }
}
