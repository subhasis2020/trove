using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class UserNotificationSettingsService : FoundryRepositoryBase<UserNotificationSettings>, IUserNotificationSettingsService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly INotificationSettingsService _notifications;
        public UserNotificationSettingsService(IDatabaseConnectionFactory databaseConnectionFactory, INotificationSettingsService notifications)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _notifications = notifications;
        }

        public async Task<NotificationSettingsDto> UpdateUserNotificationsSettings(int notificationId, bool isNotificationEnabled, int userId, bool allAboveEnabled)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    if (allAboveEnabled)
                    {
                        var notifications = await _notifications.AllAsync();
                        if (notifications.ToList().Count > 0)
                        {
                            foreach (var item in notifications)
                            {
                                var result = await FindAsync(new { userId, notificationId = item.id });
                                if (result != null)
                                {
                                    result.IsNotificationEnabled = true;
                                    await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateUsersNotificationSettingsQuery, new { UserId = userId, NotificationId = item.id, IsNotificationEnabled = true });
                                }
                                else
                                {
                                    var userNotification = new UserNotificationSettings();
                                    userNotification.notificationId = item.id;
                                    userNotification.userId = userId;
                                    userNotification.IsNotificationEnabled = true;
                                    await AddAsync(userNotification);
                                }
                            }
                        }
                    }
                    else if (notificationId <= 0)
                    {
                        var result = await GetDataAsync(new { userId });
                        if (result.ToList().Count > 0)
                        {
                            foreach (var item in result)
                            {
                                item.IsNotificationEnabled = false;
                                await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateUsersNotificationSettingsQuery, new { UserId = userId, NotificationId = item.notificationId, IsNotificationEnabled = item.IsNotificationEnabled });
                            }
                        }
                    }
                    else
                    {
                        var result = await FindAsync(new { userId, notificationId });
                        if (result != null)
                        {
                            result.IsNotificationEnabled = isNotificationEnabled;
                            await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateUsersNotificationSettingsQuery, new { UserId = userId, NotificationId = notificationId, IsNotificationEnabled = isNotificationEnabled });
                        }
                        else
                        {
                            var userNotification = new UserNotificationSettings();
                            userNotification.notificationId = notificationId;
                            userNotification.userId = userId;
                            userNotification.IsNotificationEnabled = isNotificationEnabled;
                            await AddAsync(userNotification);
                        }
                    }
                    return await _notifications.GetUserNotificationsSettings(userId);
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

        public async Task<List<int>> GetUserNotificationSettingByNotificaction(List<int> userIds, int notificationId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return (await sqlConnection.QueryAsync<int>(SQLQueryConstants.GetUserNotificationSettingByNotificaction, new { UserIds = userIds, NotificationId = notificationId })).ToList();
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

