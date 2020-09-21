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
    public class LoyalityGlobalSetting : FoundryRepositoryBase<OrgLoyalityGlobalSettings>, ILoyalityGlobalSetting
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public LoyalityGlobalSetting(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration )
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));          
            _configuration = configuration;
          
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddEditLoyalityGlobalSettings(LoyalityGlobalSettingViewModel model)
        {          
                try
                {
                    var settingsInfo = await FindAsync(new { id = model.id });
                    if (settingsInfo == null)
                    {
                    settingsInfo = new OrgLoyalityGlobalSettings();
                    settingsInfo.createdDate = DateTime.Now;

                }
                settingsInfo.bitePayRatio = model.bitePayRatio;
             
                settingsInfo.dcbFlexRatio = model.dcbFlexRatio;
                settingsInfo.globalRatePoints = model.globalRatePoints;
                settingsInfo.globalReward = model.globalReward;
                settingsInfo.modifiedDate = DateTime.Now;
                settingsInfo.organisationId = model.organisationId;
                settingsInfo.userStatusRegularRatio = model.userStatusRegularRatio;
                settingsInfo.userStatusVipRatio = model.userStatusVipRatio;
                settingsInfo.loyalityThreshhold = model.loyalityThreshhold;
                settingsInfo.FirstTransactionBonus = model.FirstTransactionBonus;

                  var Id = await InsertOrUpdateAsync(settingsInfo, new { id = model.id > 0 ? model.id : 0 });
                return Id;
                }
                catch (Exception)
                {
                    throw;
                }
            
    
        }

        public async Task<IEnumerable<OrgLoyalityGlobalSettingsDto>> GetOrgLoyalityGlobalSettings(int id)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var settings = (await sqlConnection.QueryAsync<OrgLoyalityGlobalSettingsDto>(SQLQueryConstants.GetLoyalityGlobalSettingQuery, new { id = id }));
                    
                    return settings;
                }
                catch (Exception)
                {
                    throw;
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
