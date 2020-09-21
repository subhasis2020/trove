using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Microsoft.Extensions.Configuration;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class Promotions : FoundryRepositoryBase<Promotion>, IPromotions
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IOfferTypeService _offerType;
        private readonly IOfferSubTypeService _offerSubType;
        private readonly IPhotos _photos;
        private readonly IMapper _mapper;
        public Promotions(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration,
            IMapper mapper, IOfferTypeService offerType, IOfferSubTypeService offerSubType, IPhotos photos)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _mapper = mapper;
            _offerType = offerType;
            _offerSubType = offerSubType;
            _photos = photos;
        }
        public async Task<int> AddEditPromotion(MerchantRewardViewModel model)
        {
            try
            {
                var chkExist = await FindAsync(new { id = model.id });
                if (chkExist == null)
                {
                    chkExist = new Promotion();
                }
                chkExist.amounts = model.amounts;
                chkExist.backgroundColor = model.backgroundColor;
                chkExist.bannerDescription = model.bannerDescription;
                chkExist.bannerTypeId = model.bannerTypeId;
                chkExist.businessTypeId = model.businessTypeId;

                if (model.createdBy != null)
                {
                    chkExist.createdBy = model.createdBy;
                }
                if (model.createdDate != null)
                {
                    chkExist.createdDate = model.createdDate;
                }
                chkExist.description = model.description;
                chkExist.endDate = model.endDate;
                chkExist.endTime = TimeSpan.Parse(model.endTime.ToString());
                chkExist.firstGradiantColor = model.firstGradiantColor;
                chkExist.IsPublished = model.IsPublished;
                if (model.isActive != null)
                {
                    chkExist.isActive = model.isActive;
                }
                if (model.isDeleted != null)
                {
                    chkExist.isDeleted = model.isDeleted;
                }
                chkExist.MerchantId = model.MerchantId;
                if (model.modifiedBy != null)
                {
                    chkExist.modifiedBy = model.modifiedBy;
                }
                if (model.modifiedDate != null)
                {
                    chkExist.modifiedDate = model.modifiedDate;
                }
                chkExist.name = model.name;
                chkExist.noOfVisits = model.noOfVisits;
                if (model.IsPromotion)
                {
                    var offerType = await _offerType.GetSingleDataByConditionAsync(new { offerType = "Promotions" });
                    if (offerType != null)
                    {
                        chkExist.offerTypeId = offerType.id;
                    }
                    if (model.IsDaily)
                    {
                        var offerSubType = await _offerSubType.GetSingleDataByConditionAsync(new { title = "Daily Promotion" });
                        if (offerSubType != null)
                        {
                            chkExist.offerSubTypeId = offerSubType.id;
                        }
                    }
                    else
                    {
                        var offerSubType = await _offerSubType.GetSingleDataByConditionAsync(new { title = "Multi Day Promotion" });
                        if (offerSubType != null)
                        {
                            chkExist.offerSubTypeId = offerSubType.id;
                        }
                    }
                }
                else
                {
                    chkExist.offerSubTypeId = model.offerSubTypeId;
                    chkExist.offerTypeId = model.offerTypeId;
                }
                if (model.IsPromotion)
                {
                    if (model.IsDaily)
                    {
                        chkExist.promotionDay = model.promotionDay;
                    }
                }
                else
                {
                    chkExist.promotionDay = model.promotionDay;
                }
                chkExist.startDate = model.startDate;
                chkExist.startTime = TimeSpan.Parse(model.startTime.ToString());
                chkExist.id = model.id;
                var promotionId = await InsertOrUpdateAsync(chkExist, new { id = model.id });
                if (promotionId > 0)
                {
                    await _photos.SaveUpdateImage(model.ImagePath, promotionId, promotionId, (int)PhotoEntityType.OffersPromotions);
                }
                return promotionId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> EditPromotionStatus(int id, bool status)
        {
            try
            {
                /*var chkExist = await GetDataByIdAsync(SQLQueryConstants.GetPromotionDataByIdQuery, new { id = id });*/
                var chkExist = await GetDataByIdAsync(new { id = id, isDeleted = false, isActive = true });
                if (chkExist == null)
                {
                    chkExist = new Promotion();
                }
                chkExist.isActive = status;
                chkExist.id = id;
                return await UpdateAsync(chkExist, new { id });
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<PromotionsDto>> GetAllPromotionsOfMerchant(int merchantId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.QueryAsync<PromotionsDto>(SQLQueryConstants.GetAllPromotionsOfMerchantQuery, new { MerchantId = merchantId });
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

        public async Task<PromotionsDto> GetPromotionDetailById(int promotionId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<PromotionsDto>(SQLQueryConstants.GetPromotionsOfMerchantByIdQuery, new { PromotionId = promotionId })).FirstOrDefault();
                    if (result != null)
                    {
                        result.ImageFileName = result.ImagePath;
                        result.ImagePath = await _photos.GetAWSBucketFilUrl(result.ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.PromotionDefaultImage));
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

        public async Task<List<RewardsDto>> GetAchievedRewardDetailInformation(int userId, int programId, string baseURL)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<RewardsDto>(SQLQueryConstants.GetAchievedRewardDetailInformation, new { UserId = userId, OfferTypeId = Constants.OfferType.Rewards, ProgramId = programId, BaseURL = baseURL.TrimEnd('/') })).ToList();
                    result.ForEach(x => x.IconPath = string.Concat(x.IconPath.Split('.')[0], "-white.", x.IconPath.Split('.')[1]));
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

        public async Task<List<RewardsDto>> GetAllRewardsInformation(int userId, int programId, string baseURL)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<RewardsDto>(SQLQueryConstants.GetAllRewardsInformation, new { UserId = userId, OfferTypeId = Constants.OfferType.Rewards, ProgramId = programId, BaseURL = baseURL.TrimEnd('/') })).ToList();
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

        public async Task<List<RewardsOnDate>> GetUserRewardsBasedOnCurrentDate()
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<RewardsOnDate>(SQLQueryConstants.GetUserRewardsBasedOnCurrentDate, new { OfferTypeId = Constants.OfferType.Rewards })).ToList();
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

        public async Task<List<RewardsOnDate>> GetUserOffersBasedOnCurrentDate()
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<RewardsOnDate>(SQLQueryConstants.GetUserOffersBasedOnCurrentDate, new { OfferTypeId = Constants.OfferType.Promotions })).ToList();
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

        public async Task<List<RewardsOnDate>> GetUsersToAheadCompleteRewards()
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<RewardsOnDate>(SQLQueryConstants.GetUsersToAheadCompleteRewards, new { OfferTypeId = Constants.OfferType.Rewards })).ToList();
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

        public async Task<List<RewardsOnDate>> GetUsersCompletedRewardsNotify()
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<RewardsOnDate>(SQLQueryConstants.GetUsersCompletedRewardsForNotifications, new { OfferTypeId = Constants.OfferType.Rewards })).ToList();
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
