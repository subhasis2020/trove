using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface ISMSService
    {
        Task<bool> SendSMS(string sid, string authToken, string toPhone, string message);
        Task<SMSTemplate> GetSMSTemplateByName(string name);
    }
}
