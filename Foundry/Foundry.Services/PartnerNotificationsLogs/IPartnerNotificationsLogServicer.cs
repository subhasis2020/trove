using Foundry.Domain;
using Foundry.Domain.ApiModel.PartnerApiModel;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services.PartnerNotificationsLogs
{
    public interface IPartnerNotificationsLogServicer :IFoundryRepositoryBase<PartnerNotificationsLog>
    {
        Task<PartnerNotificationsLog> PartnerNotificationsLogRequest(string apiName, string apiUrl, string request, int userid);
        Task PartnerNotificationsLogResponse(PartnerNotificationsLog objRequestLog, string xml);
         Task<IEnumerable<PartnerNotificationsLogModel>> GetApiNames();
         Task<IEnumerable<PartnerNotificationsLogModel>> GetAllStatus();
        Task<IEnumerable<ProgramModel>> GetAllPrograms(int organisationId);
    }
}
