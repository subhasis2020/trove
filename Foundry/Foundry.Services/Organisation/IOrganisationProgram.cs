using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using org = Foundry.Domain.DbModel;

namespace Foundry.Services
{
    public interface IOrganisationProgram : IFoundryRepositoryBase<org.OrganisationProgram>
    {
        Task<IEnumerable<OrganisationProgramDto>> GetOrganisationPrograms(int organisationId, string roleName, int userId);
        Task<int> DeleteOrganisationProgram(int organisationId, int programId);
        Task<string> AddUpdateOrganisationProgram(List<org.OrganisationProgram> model);
        Task<int> AddSpecificOrganisationProgram(int programId, int organisationId);
    }
}
