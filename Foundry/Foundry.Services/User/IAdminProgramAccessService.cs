using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IAdminProgramAccessService : IFoundryRepositoryBase<AdminProgramAccess>
    {
        Task<int> AddUpdateAdminProgramType(List<AdminProgramAccess> model);
        Task<int> DeleteAdminProgramType(int UserId);
    }
}
