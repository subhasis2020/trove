using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class I2CCardBankAccountService : FoundryRepositoryBase<i2cCardBankAccount>, II2CCardBankAccountService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public I2CCardBankAccountService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }


        /// <summary>
        /// This method will get the list of Plans.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<List<I2CCardBankAccountModel>> GetBankAccountListing(int byUserId, int toUserId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        ByUserId = byUserId,
                        ToUserId = toUserId
                    };
                    var accountListings = await sqlConnection.QueryAsync<I2CCardBankAccountModel>(SQLQueryConstants.GetI2CBankAccountsByUserQuery, obj);
                    return accountListings.ToList();
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
