using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class UserProgramModel
    {
        public int UserId { get; set; }

        public int ProgramId { get; set; }
    }
}
