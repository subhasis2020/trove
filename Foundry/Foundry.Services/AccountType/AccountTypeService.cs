using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class AccountTypeService : FoundryRepositoryBase<AccountType>, IAccountTypeService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public AccountTypeService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<List<AccountType>> GetAllAccountTypes()
        {
            object obj = new { };
            return (await AllAsync()).ToList();
        }
    }
}
