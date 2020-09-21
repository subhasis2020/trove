using System;
using Foundry.Domain.DbModel;
using System.Collections.Generic;
using Foundry.Domain.Dto;
using System.Threading.Tasks;
using Dapper;
using Foundry.Domain;
using System.Linq;

namespace Foundry.Services
{
    public class NotificationSettingsService : FoundryRepositoryBase<NotificationSettings>, INotificationSettingsService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public NotificationSettingsService(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<NotificationSettingsDto> GetUserNotificationsSettings(int userId)
        {
            var notificationSettings = new NotificationSettingsDto();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    notificationSettings.UserNotifications = (await sqlConnection.QueryAsync<UserNotificationSettingsDto>(SQLQueryConstants.GetUsersNotificationSettingsQuery, new { UserId = userId })).ToList();
                    if (notificationSettings.UserNotifications.Where(x => x.UserNotificationSet == true).Count() == (await AllAsync()).Count())
                    {
                        notificationSettings.AllAboveNotifications = true;
                    }
                    else { notificationSettings.AllAboveNotifications = false; }
                    return notificationSettings;
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
