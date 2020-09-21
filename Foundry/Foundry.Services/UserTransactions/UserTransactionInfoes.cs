using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Microsoft.Extensions.Configuration;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class UserTransactionInfoes : FoundryRepositoryBase<UserTransactionInfo>, IUserTransactionInfoes
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IPhotos _photos;
        private readonly IConfiguration _configuration;
        public UserTransactionInfoes(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, IPhotos photos)
        : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _photos = photos;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<UserAvailableBalanceDto>> GetUserAvailableBalance(int userId, int programId)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QueryAsync<UserAvailableBalanceDto>(SQLQueryConstants.GetUserAvailableBalance, new { userId = userId, programId = programId });
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex; 
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        public async Task<UserAvailableBalanceDto> GetUserAvailableBalanceForVPL(int userId, int programId)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleAsync<UserAvailableBalanceDto>(SQLQueryConstants.GetUserAvailableBalanceForVPL, new { userId = userId });
                    return result;
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

        public async Task<List<LinkedUsersTransactionsDto>> GetRespectiveUsersTransactions(int userId, DateTime? dateMonth, int? programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<LinkedUsersTransactionsDto>(SQLQueryConstants.GetRespectiveUserTransaction, new { linkedUserId = userId, DateMonth = dateMonth, ProgramId = programId, photoType= (int)PhotoEntityType.Organisation, DefaultImagePath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.MerchantDefaultImage) })).ToList();
                    for (int i = 0; i < transactions.Count; i++)
                    {
                        transactions[i].ImagePath = await _photos.GetAWSBucketFilUrl(transactions[i].ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.MerchantDefaultImage));
                    }
                    return transactions;
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

         public async Task<List<UsersTransactionsAccountDto>> GetProgramAccountsDetailsByAccountIds(List<int> accountIds)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var ppal = await sqlConnection.QueryAsync<UsersTransactionsAccountDto>(SQLQueryConstants.GetUserTransactionUserDetailsByAccountIds, new { AccountId = accountIds });
                    return ppal.ToList();
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
