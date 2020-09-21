using Dapper;
using Foundry.Domain;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using org = Foundry.Domain.DbModel;

namespace Foundry.Services
{
    public class OrganisationProgram : FoundryRepositoryBase<org.OrganisationProgram>, IOrganisationProgram
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public OrganisationProgram(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        #region API


        public async Task<IEnumerable<OrganisationProgramDto>> GetOrganisationPrograms(int organisationId, string roleName, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        OrganisationId = organisationId,
                        RoleName = roleName,
                        UserId = userId
                    };
                    return await sqlConnection.QueryAsync<OrganisationProgramDto>(SQLQueryConstants.GetOrganisationProgramsListQuery, obj);

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<int> DeleteOrganisationProgram(int organisationId, int programId)
        {
            return await RemoveAsync(new { OrganisationId = organisationId, ProgramId = programId });
        }

        public async Task<string> AddUpdateOrganisationProgram(List<org.OrganisationProgram> model)
        {
            var resultDeleted = await DeleteEntityAsync(new { OrganisationId = model.FirstOrDefault().organisationId });
            await AddAsync(model);
            return Cryptography.EncryptPlainToCipher(model.FirstOrDefault().organisationId.ToString());
        }

        public async Task<int> AddSpecificOrganisationProgram(int programId, int organisationId)
        {
            int orgPrgId = 0;
            var orgPrg = await FindAsync(new { ProgramId = programId, OrganisationId = organisationId });
            if (orgPrg == null)
            {
                orgPrg = new org.OrganisationProgram()
                {
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    organisationId = organisationId,
                    programId = programId
                };

                orgPrgId = await AddAsync(orgPrg);
            }
            else
            {
                orgPrgId = orgPrg.id;
            }
            return orgPrgId;
        }
        #endregion
    }
}
