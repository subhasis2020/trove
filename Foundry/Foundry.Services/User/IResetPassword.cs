using Foundry.Domain.DbModel;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IResetPassword : IFoundryRepositoryBase<ResetUserPassword>
    {
        Task<User> CheckUser(string email);
        Task<ResetUserPassword> GenerateForgotPasswordToken(string email, bool isFromWeb = false);
        Task<bool> ResetUserPassword(string email, string password);
        Task<ResetUserPassword> VerifyPasswordReset(string email, string token);
        Task<int> SaveResetPassword(int userId);
        Task<ResetUserPassword> VerifyPasswordResetWeb(string email);
    }
}
