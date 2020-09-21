using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class OrganisationProgramTypeService : FoundryRepositoryBase<OrganisationProgramType>, IOrganisationProgramTypeService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public OrganisationProgramTypeService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<int> AddUpdateOrganisationProgramType(List<OrganisationProgramType> model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteOrganisationProgramTypeQuery, new { OrganisationId = model.FirstOrDefault().OrganisationId });
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

            return await AddAsync(model);

        }
    }
}
