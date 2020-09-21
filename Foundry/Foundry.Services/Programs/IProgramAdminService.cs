using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IProgramAdminService : IFoundryRepositoryBase<ProgramAdmins>
    {
        Task<int> AddUpdateProgramFromPrgAdmins(List<ProgramAdmins> model);
        Task<int> DeleteAdminProgram(int UserId);
    }
}
