using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserAgreementHistoryService : IFoundryRepositoryBase<UserAgreementHistory>
    {
        Task<List<UserAgreementHistoryDto>> GetCardHolderAgreementHistory(int programId);
        Task<List<UserAgreementHistoryDto>> GetCardHolderAgreementHistoryVersions(int programId, int userId);
    }
}
