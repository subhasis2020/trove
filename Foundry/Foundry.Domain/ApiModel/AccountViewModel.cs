using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2}, at max {1} characters long and unique.", MinimumLength = 2)]
        [Display(Name = "Username")]
        public string UserName { get; set; }
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
        [Display(Name = "Email")]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\\S+$).{8,15}$", ErrorMessage = "Password is not valid. Password must be  8 to 15 characters long and should contain atleast one capital and one special character.")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public int OrganisationId { get; set; }
        public int ProgramId { get; set; }
        public string ProfileImagePath { get; set; }
        [Required]
        [StringLength(700,ErrorMessage ="The address must not be greater than 700 characters.")]
        public string Address { get; set; }
        public int GroupId { get; set; }
        public int RoleId { get; set; }
    }
}
