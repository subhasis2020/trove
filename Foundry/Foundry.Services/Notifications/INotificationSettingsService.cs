using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface INotificationSettingsService : IFoundryRepositoryBase<NotificationSettings>
    {
        Task<NotificationSettingsDto> GetUserNotificationsSettings(int userId);
    }
}
