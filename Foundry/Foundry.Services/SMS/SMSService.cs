using Foundry.Domain;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Linq;
using Foundry.Domain.DbModel;

namespace Foundry.Services
{
    public class SMSService : FoundryRepositoryBase<SMSTemplate>, ISMSService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IGeneralSettingService _setting;
        public SMSService(IGeneralSettingService setting, IDatabaseConnectionFactory databaseConnectionFactory)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _setting = setting;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<bool> SendSMS(string sid, string authToken, string toPhone, string body)
        {
            try
            {
                var fromPhone = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsConstants.SMS))
                    .ToList().Where(x => x.KeyName == "FromPhone").Select(x => x.Value).FirstOrDefault();
                TwilioClient.Init(sid, authToken);
                await MessageResource.CreateAsync(to: toPhone, from: fromPhone, body: body);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SMSTemplate> GetSMSTemplateByName(string name)
        {
            object obj = new { Name = name };
            return await GetDataByIdAsync(obj);
        }
    }
}
