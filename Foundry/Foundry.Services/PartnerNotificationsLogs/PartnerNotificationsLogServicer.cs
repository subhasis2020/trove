using Foundry.Domain;
using Foundry.Domain.ApiModel.PartnerApiModel;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
namespace Foundry.Services.PartnerNotificationsLogs
{
     public class PartnerNotificationsLogServicer : FoundryRepositoryBase<PartnerNotificationsLog>, IPartnerNotificationsLogServicer
    {

        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public PartnerNotificationsLogServicer(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;

        }




        public async Task<PartnerNotificationsLog> PartnerNotificationsLogRequest(string apiName, string apiUrl, string request, int userid)
        {
            PartnerNotificationsLog objRequestLog = new PartnerNotificationsLog()
            {
                UserId = userid,
                ApiName = apiName,
                ApiUrl = apiUrl,
                Request = request,
                CreatedDate = DateTime.UtcNow
            };

            objRequestLog.Id = await AddAsync(objRequestLog);
            return objRequestLog;
        }

        public async Task PartnerNotificationsLogResponse(PartnerNotificationsLog objRequestLog, string xml)
        {
            objRequestLog.Response = xml;
            objRequestLog.UpdatedDate = DateTime.UtcNow;
            await UpdateAsync(objRequestLog, new { id = objRequestLog.Id });
        }

        public async Task<IEnumerable<PartnerNotificationsLogModel>> GetApiNames()
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var names = (await sqlConnection.QueryAsync<PartnerNotificationsLogModel>(SQLQueryConstants.GetApiNameQuery ));

                    return names.ToList();
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

        public async Task<IEnumerable <PartnerNotificationsLogModel>> GetAllStatus()
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var names = (await sqlConnection.QueryAsync<PartnerNotificationsLogModel>(SQLQueryConstants.GetStatusQuery));

                    return names;
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

        public async Task<IEnumerable<ProgramModel>> GetAllPrograms(int organisationId)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var prg = (await sqlConnection.QueryAsync<ProgramModel>(SQLQueryConstants.GetAllProgramQuery,new { organisationId= organisationId }));

                    return prg;
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

        //public async Task<int> AddEditPartnerNotificationsLog(PartnerNotificationsLogModel model)
        //{
        //    try
        //    {
        //        var NotificationsLog = await FindAsync(new { id = model.Id });
        //        if (NotificationsLog == null)
        //        {
        //            NotificationsLog = new PartnerNotificationsLog();
        //            NotificationsLog.CreatedDate = DateTime.Now;

        //        }
        //        NotificationsLog.ApiUrl = model.ApiUrl;
        //        NotificationsLog.ApiName = model.ApiName;
        //        //NotificationsLog.globalReward = model.globalReward;
        //        //NotificationsLog.modifiedDate = DateTime.Now;
        //        //NotificationsLog.organisationId = model.organisationId;
        //        //NotificationsLog.userStatusRegularRatio = model.userStatusRegularRatio;
        //        //NotificationsLog.userStatusVipRatio = model.userStatusVipRatio;
        //        //NotificationsLog.loyalityThreshhold = model.loyalityThreshhold;

        //        //var Id = await InsertOrUpdateAsync(settingsInfo, new { id = model.id > 0 ? model.id : 0 });
        //      //  return Id;
        //        return 12;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }


        //}

    }
}
