using System.ComponentModel.DataAnnotations;

namespace Foundry.Web.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "*Email address is required.")]
        [EmailAddress(ErrorMessage = "*Please enter a valid email address")]
        [StringLength(80, ErrorMessage = "*Email address must not be greater than 80 characters long.")]
        public string EmailAddress { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "*Password is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "*Confirm password is required.")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "*Password and confirm password does not match.")]
        public string ConfirmPassword { get; set; }
        public bool invite { get; set; }
        public string LabelForPage { get; set; }
        public int UserId { get; set; }
        public bool IsLink { get; set; }
    }
}
