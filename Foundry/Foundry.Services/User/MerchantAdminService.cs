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
    public class MerchantAdminService: FoundryRepositoryBase<MerchantAdmins>, IMerchantAdminService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public MerchantAdminService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<int> AddUpdateMerchantFromAdmins(List<MerchantAdmins> model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteMerchantAdminQuery, new { UserId = model.FirstOrDefault().adminUserId });
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

        public async Task<int> DeleteAdminMerchant(int UserId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteMerchantAdminQuery, new { UserId });
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
