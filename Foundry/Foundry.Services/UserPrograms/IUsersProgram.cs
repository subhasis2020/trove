using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;

namespace Foundry.Services
{
    public interface IUsersProgram : IFoundryRepositoryBase<UserProgram>
    {
        Task<UserProgram> GetUserProgram(int userId);
        Task<UserProgram> GetUserProgramLinkingByEmailNID(int programId, string userId, string emailAddress);
        Task<UserProgram> GetUserProgramLinkingByEmail(int programId, string emailAddress);
        Task<UserProgram> UpdateUserProgramNUserReturn(string userId, int programId, string emailAddress);
        Task<UserProgram> ValidateLinkAccountCode(int userId, int programId, string verificationCode);
        Task<UserDto> UpdateValidationCodeNUserDetailReturn(int userId, int programId, bool isForAuthorizedUser);
        Task<int> AddUserInProgram(int userId, int programId);
        Task<UserProgram> CheckUserLinkingWithProgram(int userId, int programId);
    }
}
