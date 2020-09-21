using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class SessionCheckModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string SessionId { get; set; }

        public int ProgramId { get; set; }
    }
}
