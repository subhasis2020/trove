using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
  public  interface ISiteLevelOverrideSetting
    {
        Task<int> AddEditSiteLevelOverrideSettings(SiteLevelOverrideSettingApiViewModel model);
        Task<IEnumerable<SiteLevelOverrideSettingsDto>> GetSiteLevelOverrideSettings(int id);
        Task<IEnumerable<SiteLevelOverrideSettingsDto>> GetSiteLevelOverrideSettingsByUserProgId(int id);
    }
}
