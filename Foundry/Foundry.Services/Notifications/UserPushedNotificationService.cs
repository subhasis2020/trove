using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class UserPushedNotificationService : FoundryRepositoryBase<UserPushedNotifications>, IUserPushedNotificationService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserPushedNotificationService(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<UserPushedNotificationSettingDto>> GetUserPushNotifictaionsList(int userId, int programId, int pageNumber, int pageSize, string userDeviceId, string userDeviceType)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var userPushNotifyLst = (await sqlConnection.QueryAsync<UserPushedNotificationSettingDto>(SQLQueryConstants.GetUserPushedNotificationListSP, new { IsActive = true, IsDeleted = false, UserId = userId, ProgramId = programId, PageNumber = pageNumber, PageSize = pageSize, UserDeviceId = userDeviceId, UserDeviceType = userDeviceType }, commandType: CommandType.StoredProcedure)).ToList();
                    return userPushNotifyLst;
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

        public async Task<NotificationMainDto> GetUserPushNotificationsList(int notificationType, int pageNumber, int pageSize, int userId, int programId,string userDeviceId,string userDeviceType)
        {
            NotificationMainDto notificationMain = new NotificationMainDto();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetUserPushedNotificationListSP, new { IsActive = true, IsDeleted = false, UserId = userId, ProgramId = programId, PageNumber = pageNumber, PageSize = pageSize, NotificationType = notificationType, UserDeviceId=userDeviceId, UserDeviceType=userDeviceType }, commandType: CommandType.StoredProcedure).Result)
                    {
                        try
                        {
                            notificationMain.UserPushedNotificationList = new List<UserPushedNotificationSettingDto>();
                            notificationMain.NotificationMasterList = new List<NotificationMaster>();
                            notificationMain.UserPushedNotificationList = multi.Read<UserPushedNotificationSettingDto>().ToList();
                            notificationMain.NotificationMasterList = multi.Read<NotificationMaster>().ToList();
                           
                            return notificationMain;
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

        public async Task<int> GetUserPushNotifictaionsUnreadCount(int userId, int programId, string userDeviceId, string userDeviceType)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var userPushNotifyLstCnt = (await sqlConnection.ExecuteScalarAsync<int>(SQLQueryConstants.GetUserPushedUnreadNotificationCount, new { IsActive = true, IsDeleted = false, UserId = userId, ProgramId = programId, UserDeviceId = userDeviceId, UserDeviceType = userDeviceType }));
                    return userPushNotifyLstCnt;
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

        public async Task<IEnumerable<PartnerNotificationsLogDto>> GetPushNotificationLogs( int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<PartnerNotificationsLogDto>(SQLQueryConstants.GetNotificationLogsBySP, new {  PageNumber = pageNumber, PageSize = pageSize, SortColumnName = sortColumnName, SortOrderDirection = sortOrderDirection }, commandType: CommandType.StoredProcedure));

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

        public async Task<IEnumerable<PartnerNotificationsLogDto>> GetPushNotificationLogsWithFilter(int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection,string apiname,string status,string date,string programid)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<PartnerNotificationsLogDto>(SQLQueryConstants.GetNotificationLogsWithFilterBySP, new { PageNumber = pageNumber, PageSize = pageSize, SortColumnName = sortColumnName, SortOrderDirection = sortOrderDirection, ApiName=apiname,Status=status,Date=date, ProgramId=programid }, commandType: CommandType.StoredProcedure));

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
