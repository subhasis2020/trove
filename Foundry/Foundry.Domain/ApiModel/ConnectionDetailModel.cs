using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class ConnectionDetailModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BenefactorId { get; set; }
        public int ProgramId { get; set; }
        [Required]
        public string SessionId { get; set; } = string.Empty;
        public int Type { get; set; }
        public decimal Amount { get; set; }
    }
}
