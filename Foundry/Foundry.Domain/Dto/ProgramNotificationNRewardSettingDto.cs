using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramNotificationNRewardSettingDto
    {
        public int ProgramId { get; set; }
        public bool IsNotificationToShow { get; set; } = true;
        public bool IsRewardToShow { get; set; } = true;
    }
}
