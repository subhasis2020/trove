using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IProgramBrandingService : IFoundryRepositoryBase<ProgramBranding>
    {
        Task<IEnumerable<BrandingListingDto>> GetBrandingListing(int programId);
        Task<BrandingDetailsWithMasterDto> GetBrandingInfoWithAccountType(int bId, int programId);
        Task<int> DeleteBrandById(int Id);
        Task<string> AddEditBrandingDetails(ProgramBrandingViewModel model);
        Task<ProgramBranding> CheckCardExistence(string cardNumber);
        Task<List<ProgramBrandingDto>> GetBrandingsForMobile(int programId, int accountTypeId, int userId);
        Task<BrandingDetailsWithAccountType> GetBrandingInfoOnAccountSelection(int accountId);
    }
}
