using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class AdminLevelModel
    {
        public int? UserId { get; set; } = null;
        public string UserImagePath { get; set; }
        public IFormFile PostedFileUpload { get; set; }
        [Required(ErrorMessage = "Please enter first name.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Name contains only letters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter last name.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Last name contains only letters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter email.")]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Please enter phone number.")]
        //  [RegularExpression("^(\\+\\d{1}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage = "Please enter a valid phone number in +1 (XXX) XXX-XXXX.")]
        [RegularExpression("^[- +()0-9]+$", ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please select user access role.")]
        public int RoleId { get; set; }
    }
}
