using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class UserNotificationSettingsModel
    {
        public int notificationId { get; set; }
        public int isNotificationEnabled { get; set; }
        public int allAboveEnabled { get; set; }
    }


    public class UserPushedNotificatiosModel
    {
        public int notificationTypeId { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
