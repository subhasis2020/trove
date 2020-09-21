using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IGatewayCardWebHookTokenService : IFoundryRepositoryBase<GatewayCardWebHookToken>
    {
        Task<GatewayCardWebhookTokenDto> GetLatestWebhookToken(int creditUserId, int debitUserId);
        Task<GatewayCardWebhookTokenDto> GetLatestWebhookTokenByClientToken(string clientToken);
        Task<BinDataDto> CheckBinNumberIsValid(long bin);
    }
}
