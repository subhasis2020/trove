using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using Foundry.Domain.Dto;
using Foundry.Domain;
using Dapper;
namespace Foundry.Services
{
   public class SiteLevelOverrideSetting : FoundryRepositoryBase<SiteLevelOverrideSettings>, ISiteLevelOverrideSetting
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public SiteLevelOverrideSetting(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));

            _configuration = configuration;

        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddEditSiteLevelOverrideSettings(SiteLevelOverrideSettingApiViewModel model)
        {
            try
            {
                var overridesettingsInfo = await FindAsync(new { id = model.id });
                if (overridesettingsInfo == null)
                {
                    overridesettingsInfo = new SiteLevelOverrideSettings();
                    overridesettingsInfo.createdDate = DateTime.Now;
                }
                overridesettingsInfo.programId = model.programId;             
                overridesettingsInfo.siteLevelBitePayRatio = model.siteLevelBitePayRatio;
                overridesettingsInfo.siteLevelDcbFlexRatio = model.siteLevelDcbFlexRatio;
                overridesettingsInfo.siteLevelUserStatusRegularRatio = model.siteLevelUserStatusRegularRatio;
                overridesettingsInfo.modifiedDate = DateTime.Now;
                overridesettingsInfo.siteLevelUserStatusVipRatio = model.siteLevelUserStatusVipRatio;
                overridesettingsInfo.FirstTransactionBonus = model.FirstTransactionBonus;
               
                var Id = await InsertOrUpdateAsync(overridesettingsInfo, new { id = model.id > 0 ? model.id : 0 });
                return Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<SiteLevelOverrideSettingsDto>> GetSiteLevelOverrideSettings(int id)
        {
            
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var settings = (await sqlConnection.QueryAsync<SiteLevelOverrideSettingsDto>(SQLQueryConstants.GetSiteLevelOverrideSettingQuery1, new { id = id }));
              
                    return settings;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        public async Task<IEnumerable<SiteLevelOverrideSettingsDto>> GetSiteLevelOverrideSettingsByUserProgId(int id)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var settings = (await sqlConnection.QueryAsync<SiteLevelOverrideSettingsDto>(SQLQueryConstants.GetSitelevelOverrideSettingsbyUserProgramId, new { Id = id }));

                    return settings;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }
    }
}
