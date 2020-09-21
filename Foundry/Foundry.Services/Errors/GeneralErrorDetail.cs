using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services.Errors
{
    public class GeneralErrorDetail : FoundryRepositoryBase<ErrorMessagesDetail>, IGeneralErrorDetail
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public GeneralErrorDetail(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<List<ErrorMessagesDetail>> GetGeneralErrors()
        {           
            var error = await AllAsync();
            return error.ToList();
        }
    }
}
