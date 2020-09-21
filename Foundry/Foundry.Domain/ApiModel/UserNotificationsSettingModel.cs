using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class UserNotificationsSettingModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int NotificationId { get; set; }
    }
}
