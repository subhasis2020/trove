using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface II2CCardBankAccountService : IFoundryRepositoryBase<i2cCardBankAccount>
    {
        Task<List<I2CCardBankAccountModel>> GetBankAccountListing(int byUserId, int toUserId);
    }
}
