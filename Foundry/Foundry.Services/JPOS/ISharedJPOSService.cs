using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface ISharedJPOSService
    {
        Task<List<OrganizationJPOSDto>> GetAllDataJPOS(string UrlCall, string clientIpAddress, string entityName);
        Task<OrganizationJPOSDto> GetRespectiveDataJPOS(string UrlCall, string id, string clientIpAddress, string entityName);
        Task<int> PostRespectiveDataJPOS(string UrlCall, object data, string id, string clientIpAddress, string entityName);
        Task<int> DeleteRespectiveDataJPOS(string UrlCall, object data, string id, string clientIpAddress, string entityName);
    }
}
