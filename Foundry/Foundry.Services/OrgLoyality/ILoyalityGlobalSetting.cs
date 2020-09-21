using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
   public interface ILoyalityGlobalSetting
    {
         Task<int> AddEditLoyalityGlobalSettings(LoyalityGlobalSettingViewModel model);
        Task<IEnumerable<OrgLoyalityGlobalSettingsDto>> GetOrgLoyalityGlobalSettings(int id);
    }
}
