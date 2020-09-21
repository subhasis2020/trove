using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class ProgramTypeService : FoundryRepositoryBase<ProgramType>, IProgramTypeService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public ProgramTypeService(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get All the Programs present in the DB.
        /// </summary>
        /// <returns>Programs List</returns>
        public async Task<IEnumerable<ProgramType>> GetProgramTypes()
        {
            return await GetDataAsync(new { IsActive = true, IsDeleted = false });
        }


        /// <summary>
        /// Checking the program expiration based on user and program detail.
        /// </summary>
        /// <param name="programId">ProgramId</param>
        /// <param name="userId">UserId</param>
        /// <returns>int (ProgramId)</returns>
        public async Task<IEnumerable<ProgramType>> CheckOrganisationProgramType(int organisationId)
        {
            return await GetDataAsync(SQLQueryConstants.GetOrgSelectedPrgTypeQuery, new { OrganisationId = organisationId });
        }

        public async Task<IEnumerable<ProgramTypesDto>> GetProgramTypesDetailByIds(List<int> programTypeId)
        {
            try
            {
                var programTypes = await GetDataAsync(SQLQueryConstants.GetProgramTypeDetailByIdsQuery, new { programTypeId });
                if (programTypes != null)
                {
                    return programTypes.Select(x => new ProgramTypesDto { Id = x.Id, ProgramTypeName = x.ProgramTypeName});
                }
                else { return null; }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

