using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class OfferCodeService : FoundryRepositoryBase<OfferCode>, IOfferCodeService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;

        public OfferCodeService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration
            )
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
        }

        public async Task<IEnumerable<OfferCode>> GetAllCodeOffers()
        {
            return await AllAsync();
        }
    }
}
