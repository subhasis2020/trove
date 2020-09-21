using Foundry.Domain;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Foundry.Services
{
    public class UserRoleRepository : FoundryRepositoryBase<UserRole>, IUserRoleRepository
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public UserRoleRepository(IDatabaseConnectionFactory databaseConnectionFactory)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<int> AddUserRole(UserRole userRorle)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    await DeleteEntityAsync(new { UserId = userRorle.UserId });
                    await sqlConnection.ExecuteAsync(SQLQueryConstants.AddUserRoleQuery, new { UserId = userRorle.UserId, RoleId = userRorle.RoleId });

                    return 1;
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

        public async Task<int> DeleteUserRole(UserRole userRorle)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                   
                    await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteUserRoleQuery, new { UserId = userRorle.UserId });

                    return 1;
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
