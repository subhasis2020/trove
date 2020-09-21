using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IMerchantAdminService : IFoundryRepositoryBase<MerchantAdmins>
    {
        Task<int> AddUpdateMerchantFromAdmins(List<MerchantAdmins> model);
        Task<int> DeleteAdminMerchant(int UserId);
    }
}
