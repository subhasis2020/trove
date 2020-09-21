using System.ComponentModel.DataAnnotations;

namespace Foundry.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "*Email address is required.")]
        [EmailAddress(ErrorMessage = "*Please enter a valid email address.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "*Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int? Id { get; set; }
        public bool? IsLink { get; set; }
        public string ReturnURL { get; set; }
    }
}
