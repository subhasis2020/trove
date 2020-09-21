using System.Data;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
        IDbConnection CreateConnection();
    }
}
