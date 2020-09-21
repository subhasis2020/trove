using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class LinkAccountValidateModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProgramId { get; set; }

        [Required]
        public string VerificationCode { get; set; }
    }
}
