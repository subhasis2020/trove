using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserTransactionInfoes : IFoundryRepositoryBase<UserTransactionInfo>
    {
        Task<IEnumerable<UserAvailableBalanceDto>> GetUserAvailableBalance(int userId, int programId);
        Task<UserAvailableBalanceDto> GetUserAvailableBalanceForVPL(int userId, int programId);
        Task<List<LinkedUsersTransactionsDto>> GetRespectiveUsersTransactions(int userId, DateTime? dateMonth, int? programId);
        Task<List<UsersTransactionsAccountDto>> GetProgramAccountsDetailsByAccountIds(List<int> accountIds);
    }
}
