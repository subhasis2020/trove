using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IInvitationService : IFoundryRepositoryBase<Invitation>
    {
        Task<List<Invitation>> GetAllInvitation();
        Task<Invitation> GetExistingInvitation(int userId, string emailAddress);
        Task<int> AddUpdateInvitationByUser(BenefactorRegisterModel model);
        Task<Invitation> GetExistingInvitationWithEmail(string emailAddress, int userId);
        Task<Invitation> GetExistingInvitationWithEmailUserProgram(string emailAddress, int userId, int programId);
        Task<int> DeleteUserInvitation(int userId, int benefactorId);
        Task<int> AcceptUserInvitation(int userId, int benefactorId, int programId);
        Task<InvitationDto> GetUserInfoById(int inviteid);
    }
}
