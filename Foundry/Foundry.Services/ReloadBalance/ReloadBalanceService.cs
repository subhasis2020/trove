using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class ReloadBalanceService : FoundryRepositoryBase<ReloadBalanceRequest>, IReloadBalanceService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        /// <summary>
        /// Constructor for assigning value to database connection.
        /// </summary>
        /// <param name="databaseConnectionFactory"></param>
        public ReloadBalanceService(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<List<ReloadBalanceRequest>> GetAllReloadBalance()
        {           
            return (await AllAsync()).ToList();
        }
        public async Task<ReloadBalanceRequest> GetReloadBalanceById(int Id)
        {
            object obj = new { Id };
            return await GetDataByIdAsync(obj);
        }
    }
}
