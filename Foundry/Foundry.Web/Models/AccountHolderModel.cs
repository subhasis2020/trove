using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class AccountHolderModel
    {
        public int Id { get; set; }
        public IFormFile PostedFileUpload { get; set; }
        public string UserImagePath { get; set; }
        [Required(ErrorMessage = "Please enter first name.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "First name contains only letters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter last name.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Last name contains only letters")]
        public string LastName { get; set; }
        public string AccountHolderID { get; set; }
        [Required(ErrorMessage = "Please enter email.")]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter phone number.")]
        // [RegularExpression("^(\\+\\d{1}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage = "Please enter a valid phone number in +1 (XXX) XXX-XXXX.")]
        [RegularExpression("^[- +()0-9]+$", ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid secondary email.")]
        public string SecondaryEmail { get; set; }
        public int GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string UserCustomJsonValue { get; set; }
        public List<PlanIdDto> planIds { get; set; }
        [Required(ErrorMessage = "Please select atleast one plan.")]
        public List<int> SelectedPlanIds { get; set; }
        public string ProgramCustomJsonFields { get; set; }
        public string UserEncId { get; set; }
        public string ProgEncId { get; set; }
        public string OrgEncId { get; set; }
        public string ProgramUniqueColumnName { get; set; }
        public string ImageFileName { get; set; }
        public string Jpos_AccountHolderId { get; set; }
        public string Jpos_EncUserID { get; set; }
        public string PartnerUserId { get; set; }
        public string i2cReferenceId { get; set; }

    }
}
