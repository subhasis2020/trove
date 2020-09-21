using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IGeneralSettingService : IFoundryRepositoryBase<GeneralSetting>
    {
        Task<List<GeneralSetting>> GetGeneratSettingValueByKeyGroup(string keyGroup);
        Task<List<GeneralSetting>> GetGeneralSettingValueByKeyName(string keyName);
    }
}
