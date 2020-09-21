using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserPushedNotificationService : IFoundryRepositoryBase<UserPushedNotifications>
    {
        Task<IEnumerable<UserPushedNotificationSettingDto>> GetUserPushNotifictaionsList(int userId, int programId, int pageNumber, int pageSize, string userDeviceId, string userDeviceType);
        Task<NotificationMainDto> GetUserPushNotificationsList(int notificationType, int pageNumber, int pageSize, int userId, int programId, string userDeviceId, string userDeviceType);
        Task<int> GetUserPushNotifictaionsUnreadCount(int userId, int programId, string userDeviceId, string userDeviceType);
        Task<IEnumerable<PartnerNotificationsLogDto>> GetPushNotificationLogs(int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection);
        Task<IEnumerable<PartnerNotificationsLogDto>> GetPushNotificationLogsWithFilter(int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, string apiname, string status, string date, string programid);
    }
}
