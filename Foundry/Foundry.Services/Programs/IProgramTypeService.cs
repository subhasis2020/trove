using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IProgramTypeService : IFoundryRepositoryBase<ProgramType>
    {
        Task<IEnumerable<ProgramType>> GetProgramTypes();
        Task<IEnumerable<ProgramType>> CheckOrganisationProgramType(int organisationId);
        Task<IEnumerable<ProgramTypesDto>> GetProgramTypesDetailByIds(List<int> programTypeId);
    }
}
