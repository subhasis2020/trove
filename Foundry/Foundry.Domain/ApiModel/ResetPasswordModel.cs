using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class ResetPasswordModel
    {
        [Required]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\\S+$).{8,15}$", ErrorMessage = "Password is not valid. Password must be  8 to 15 characters long and should contain atleast one capital and one special character.")]
        public string Password { get; set; }

        public int UserId { get; set; }

    }

    public class VerifyPasswordModel
    {
        [Required]
        [StringLength(5, ErrorMessage = "Token must be between 5 characters long.", MinimumLength = 4)]
        public string Token { get; set; }
        [Required]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
    }
}
