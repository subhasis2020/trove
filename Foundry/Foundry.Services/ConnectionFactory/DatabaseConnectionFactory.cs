using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public DatabaseConnectionFactory(string connectionString) { _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString)); SqlConnection.ClearAllPools(); }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            try
            {
                var sqlConnection = new SqlConnection(_connectionString);              
                await sqlConnection.OpenAsync();
                return sqlConnection;
            }
            catch
            {
                throw;
            }
        }

        public IDbConnection CreateConnection()
        {
            try
            {

                var sqlConnection = new SqlConnection(_connectionString);
                sqlConnection.Open();
                return sqlConnection;
            }
            catch
            {
                throw;
            }
        }
    }
}
