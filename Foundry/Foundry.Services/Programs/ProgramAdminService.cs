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
    public class ProgramAdminService : FoundryRepositoryBase<ProgramAdmins>, IProgramAdminService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public ProgramAdminService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<int> AddUpdateProgramFromPrgAdmins(List<ProgramAdmins> model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteProgramLevelAdminQuery, new { UserId = model.FirstOrDefault().adminUserId });
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

        public async Task<int> DeleteAdminProgram(int UserId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteProgramLevelAdminQuery, new { UserId });
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
    }
}
