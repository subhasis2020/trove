using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using Foundry.Domain.Dto;
using Foundry.Domain;
using Dapper;
using System.Data;

namespace Foundry.Services
{
    public class UserLoyaltyPointsHistoryInfo : FoundryRepositoryBase<UserLoyaltyPointsHistory>, IUserLoyaltyPointsHistoryInfo
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public UserLoyaltyPointsHistoryInfo(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;

        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddUserLoyaltyPointsHistory (UserLoyaltyPointsHistoryViewModel model)
        {
            try
            {
                var userloyalityInfo = await FindAsync(new { id = model.id });
                if (userloyalityInfo == null)
                {
                    userloyalityInfo = new UserLoyaltyPointsHistory();
                }
                userloyalityInfo.transactionId = model.transactionId;
                userloyalityInfo.transactionAmount = model.transactionAmount;
                userloyalityInfo.transactionDate = DateTime.Now;
                userloyalityInfo.rewardAmount = model.rewardAmount;
                userloyalityInfo.pointsEarned = model.pointsEarned;
                userloyalityInfo.totalPoints = model.totalPoints;
                userloyalityInfo.leftOverPoints = model.leftOverPoints;
                userloyalityInfo.createdDate = DateTime.Now;
                userloyalityInfo.isThresholdReached = model.isThresholdReached;
                userloyalityInfo.userId = model.userId;
                
                var Id = await InsertOrUpdateAsync(userloyalityInfo, new { id = model.id > 0 ? model.id : 0 });
                return Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
   
        public async Task<IEnumerable<UserLoyaltyPointsHistoryDto>> GetUserLoyaltyPointsHistory(int id)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var settings = (await sqlConnection.QueryAsync<UserLoyaltyPointsHistoryDto>(SQLQueryConstants.GetUserLoyaltyPointsHistoryQuery, new { id = id }));

                    return settings;
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
        public async Task<IEnumerable<UserLoyaltyPointsHistoryDto>> GetUserLoyaltyTrackingHistory1(int id)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var settings = (await sqlConnection.QueryAsync<UserLoyaltyPointsHistoryDto>(SQLQueryConstants.GetUserLoyaltyTrackingHistoryQuery, new { userId = id }));

                    return settings;
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

        public async Task<IEnumerable<UserLoyaltyPointsHistoryDto>> GetUserLoyaltyTrackingHistory(int id,int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<UserLoyaltyPointsHistoryDto>(SQLQueryConstants.GetUserLoyaltyTrackingTransactionsBySP, new { userId = id, PageNumber = pageNumber, PageSize = pageSize, SortColumnName = sortColumnName, SortOrderDirection = sortOrderDirection }, commandType: CommandType.StoredProcedure));
                  
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


    }
}
