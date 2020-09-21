using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class UserNotificationSettingsDto
    {
        public int NotificationId { get; set; }
        public string NotificationName { get; set; }
        public string NotificationDescription { get; set; }
        public string ColorCode { get; set; }
        public bool UserNotificationSet { get; set; }
    }
}
