using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "EmailAddress")]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string Location { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }

    }
}
