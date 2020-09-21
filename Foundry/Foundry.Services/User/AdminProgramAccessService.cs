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
    public class AdminProgramAccessService : FoundryRepositoryBase<AdminProgramAccess>, IAdminProgramAccessService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public AdminProgramAccessService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }


        public async Task<int> AddUpdateAdminProgramType(List<AdminProgramAccess> model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {

                    await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteAdminProgramTypeQuery, new { UserId = model.FirstOrDefault().UserId });
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

        public async Task<int> DeleteAdminProgramType(int UserId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteAdminProgramTypeQuery, new { UserId });
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
