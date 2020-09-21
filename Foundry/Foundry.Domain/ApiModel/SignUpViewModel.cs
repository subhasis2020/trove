using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class SignUpViewModel
    {

        [Required]
        [StringLength(50, ErrorMessage = "FirstName length must be between 50 characters long.")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "PhoneNumber length must be between 20 characters long.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }

        [Required]
        [RegularExpression("^(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\\S+$).{8,15}$", ErrorMessage = "Password is not valid. Password must be  8 to 15 characters long and should contain atleast one capital and one special character.")]
        public string Password { get; set; }

        public string PhotoPath { get; set; }

        [Required]
        public int ProgramCodeId { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }

        public string location { get; set; }


    }
}
