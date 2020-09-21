using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class OfferTypeService : FoundryRepositoryBase<OfferType>, IOfferTypeService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;

        public OfferTypeService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration
            )
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
        }
    }
}
