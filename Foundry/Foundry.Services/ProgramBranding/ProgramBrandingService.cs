using AutoMapper;
using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.LogService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class ProgramBrandingService : FoundryRepositoryBase<ProgramBranding>, IProgramBrandingService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IPlanProgramAccountLinkingService _planProgramAccount;
        private readonly IAccountTypeService _accountType;
        private readonly IProgramAccountService _programAccount;
        private readonly IPhotos _photos;
        public ProgramBrandingService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger,
            IMapper mapper, IPlanProgramAccountLinkingService planProgramAccount, IAccountTypeService accountType, IProgramAccountService programAccount,
            IPhotos photos)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _planProgramAccount = planProgramAccount;
            _accountType = accountType;
            _programAccount = programAccount;
            _photos = photos;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// This method will get the list of Branding.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BrandingListingDto>> GetBrandingListing(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        ProgramId = programId,
                        IsDeleted = false,
                        PhotoType = PhotoEntityType.BrandingLogo
                    };
                    var accountListings = (await sqlConnection.QueryAsync<BrandingListingDto>(SQLQueryConstants.GetBrandingLisitngQuery, obj)).ToList();
                    if (accountListings.Count > 0)
                    {
                        for (int i = 0; i < accountListings.Count; i++)
                        {
                            accountListings[i].ImagePath = await _photos.GetAWSBucketFilUrl(accountListings[i].ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.BrandingDefaultImage));
                        }

                    }
                    return accountListings.ToList();
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
        public async Task<int> UpdateProgramAccountStatus(int Id, bool IsActive)
        {
            try
            {
                var progacc = await FindAsync(new { Id = Id });
                progacc.isActive = IsActive;
                return await UpdateAsync(progacc, new { progacc.id });
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<int> DeleteBrandById(int Id)
        {
            return await RemoveAsync(new { Id });
        }
        public async Task<BrandingDetailsWithMasterDto> GetBrandingInfoWithAccountType(int bId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var brandInfo = new BrandingDetailsWithMasterDto();
                    var programAccount = _mapper.Map<List<ProgramAccountDto>>(await _programAccount.GetDataAsync(new { IsDeleted = false, ProgramId = programId }));
                    var accountType = _mapper.Map<List<AccountTypeDto>>(await _accountType.GetDataAsync(new { IsDeleted = false }));
                    if (bId > 0)
                    {
                        brandInfo = _mapper.Map<BrandingDetailsWithMasterDto>(await GetDataByIdAsync(new { id = bId, IsActive = true, IsDeleted = false }));
                        brandInfo.accountType = accountType.Where(x => x.Id == brandInfo.accountTypeId).Select(x => x.AccountType).FirstOrDefault();
                        var ImageBranding = (await _photos.GetDataByIdAsync(new { entityId = brandInfo.id, photoType = PhotoEntityType.BrandingLogo }));
                        if (ImageBranding != null) { 
                        brandInfo.ImagePath = await _photos.GetAWSBucketFilUrl(ImageBranding.photoPath, null);
                        brandInfo.ImageFileName = ImageBranding.photoPath;
                        }
                    }
                    else
                    {
                        brandInfo.cardNumber = Cryptography.GetNextInt64().ToString();
                    }
                    brandInfo.programAccount = programAccount;
                    return brandInfo;
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

        public async Task<BrandingDetailsWithAccountType> GetBrandingInfoOnAccountSelection(int accountId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var brandInfo = new BrandingDetailsWithAccountType();
                    if (accountId > 0)
                    {
                        var brnds = await GetDataByIdAsync(new { programAccountId = accountId,IsDeleted=false,IsActive=true });
                        if (brnds != null)
                        {
                            brandInfo = _mapper.Map<BrandingDetailsWithAccountType>(brnds);
                            var ImageBranding = (await _photos.GetDataByIdAsync(new { entityId = brnds.id, photoType = PhotoEntityType.BrandingLogo })).photoPath;
                            brandInfo.ImagePath = await _photos.GetAWSBucketFilUrl(ImageBranding, null);
                            brandInfo.ImageFileName = ImageBranding;
                        }
                        else { brandInfo.cardNumber = Cryptography.GetNextInt64().ToString(); }
                        var programAccount = (await _programAccount.GetDataAsync(new { IsActive = true, IsDeleted = false, id = accountId })).FirstOrDefault();
                        var accountType = (await _accountType.GetDataAsync(new { IsDeleted = false, id = programAccount.accountTypeId })).FirstOrDefault();
                        if (accountType != null)
                        {
                            brandInfo.accountTypeId = accountType.id;
                            brandInfo.accountType = accountType.accountType;
                        }
                    }
                    return brandInfo;
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
        public async Task<int> AddEditBranding(ProgramBrandingViewModel model)
        {
            try
            {
                var chkExist = await FindAsync(new { id = model.id });
                if (chkExist == null)
                    chkExist = new ProgramBranding();
                chkExist.accountName = model.accountName;
                chkExist.accountTypeId = model.accountTypeId;
                chkExist.brandingColor = model.brandingColor;
                if (model.createdBy != null)
                    chkExist.createdBy = model.createdBy;
                if (model.createdDate != null)
                    chkExist.createdDate = model.createdDate;
                if (model.isActive != null)
                    chkExist.isActive = model.isActive;
                if (model.isDeleted != null)
                    chkExist.isDeleted = model.isDeleted;
                if (model.modifiedBy != null)
                    chkExist.modifiedBy = model.modifiedBy;
                if (model.modifiedDate != null)
                    chkExist.modifiedDate = model.modifiedDate;
                chkExist.programAccountID = model.programAccountID;
                chkExist.programId = model.programId;
                chkExist.cardNumber = model.cardNumber;
                chkExist.id = model.id;
                return await InsertOrUpdateAsync(chkExist, new { id = model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> AddEditBrandingDetails(ProgramBrandingViewModel model)
        {
            try
            {
                var bId = await AddEditBranding(model);
                if (bId > 0)
                {
                    if (!string.IsNullOrEmpty(model.ImagePath))
                    {
                        await _photos.SaveUpdateImage(model.ImagePath, bId, bId, (int)PhotoEntityType.BrandingLogo);
                    }
                    return Cryptography.EncryptPlainToCipher(bId.ToString());
                }
                return "0";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ProgramBranding> CheckCardExistence(string cardNumber)
        {
            return await FindAsync(new { cardNumber = cardNumber });
        }

        public async Task<List<ProgramBrandingDto>> GetBrandingsForMobile(int programId, int accountTypeId, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {

                    var obj = new
                    {
                        ProgramId = programId,
                        AccountTypeId = accountTypeId,
                        IsActive = true,
                        IsDeleted = false,
                        UserID = userId
                    };
                    var result = (await sqlConnection.QueryAsync<ProgramBrandingDto>(SQLQueryConstants.GetBrandingsMobileQueryByProgram, obj)).ToList();
                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].BrandingImagePath = await _photos.GetAWSBucketFilUrl(result[i].BrandingImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.BrandingDefaultImage));
                        }


                    }
                    return result;
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
