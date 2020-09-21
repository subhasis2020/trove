using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class ProgramAccountLinkService: FoundryRepositoryBase<ProgramAccountLinking>, IProgramAccountLinkService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Constructor for assigning value to database connection.
        /// </summary>
        /// <param name="databaseConnectionFactory"></param>
        public ProgramAccountLinkService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
