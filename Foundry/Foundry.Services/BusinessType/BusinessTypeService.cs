using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class BusinessTypeService : FoundryRepositoryBase<BusinessType>, IBusinessTypeService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public BusinessTypeService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<List<BusinessType>> GetAllBusinessTypeService()
        {
            object obj = new { };
            return (await GetDataAsync(obj)).ToList();
        }
    }
}
