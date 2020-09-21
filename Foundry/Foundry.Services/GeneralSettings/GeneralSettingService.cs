using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class GeneralSettingService : FoundryRepositoryBase<GeneralSetting>, IGeneralSettingService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public GeneralSettingService(IDatabaseConnectionFactory databaseConnectionFactory)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<List<GeneralSetting>> GetGeneratSettingValueByKeyGroup(string keyGroup)
        {
            var obj = new { KeyGroup = keyGroup };
            var settings = (await GetDataAsync(obj));
            return settings.ToList();
        }
        public async Task<List<GeneralSetting>> GetGeneralSettingValueByKeyName(string keyName)
        {
            var obj = new { keyName = keyName };
            var settings = (await GetDataAsync(obj));
            return settings.ToList();
        }
    }
}
