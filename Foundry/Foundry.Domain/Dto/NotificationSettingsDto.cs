using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class NotificationSettingsDto
    {
        public bool AllAboveNotifications { get; set; }
        public List<UserNotificationSettingsDto> UserNotifications { get; set; }
    }

    public class UserPushedNotificationSettingDto
    {
        public int SrNo { get; set; }
        public int UserNotificationId { get; set; }
        public string NotificationMessage { get; set; }
        public int referenceId { get; set; }
        public string NotificationDate { get; set; }
        public string NotificationTime { get; set; }
        public int NotificationTypeId { get; set; }
        public string NotificationType { get; set; }
        public string ColorCode { get; set; }
        public bool IsRedirect { get; set; }
        public string NotificationSubType { get; set; }
        public int CustomReferenceId { get; set; }
        public bool IsNotificationRead { get; set; }
    }

    public class NotificationMaster
    {
        public int SrNo { get; set; }
        public int NotificationTypeId { get; set; }
        public string NotificationType { get; set; }
        public string ColorCode { get; set; }
    }

    public class NotificationMainDto
    {
        public List<UserPushedNotificationSettingDto> UserPushedNotificationList { get; set; }
        public List<NotificationMaster> NotificationMasterList { get; set; }
    }
}
