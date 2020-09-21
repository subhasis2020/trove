using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserNotificationSettingsService : IFoundryRepositoryBase<UserNotificationSettings>
    {
        Task<NotificationSettingsDto> UpdateUserNotificationsSettings(int notificationId, bool isNotificationEnabled, int userId, bool allAboveEnabled);
        Task<List<int>> GetUserNotificationSettingByNotificaction(List<int> userIds, int notificationId);
    }
}
