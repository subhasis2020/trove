using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ElmahCore;
using System.Globalization;

namespace Foundry.Api.Controllers
{  /// <summary>
   /// 
   /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoyalityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILoyalityGlobalSetting _loyalityGlobalSetting;
        private readonly ISiteLevelOverrideSetting _siteLevelOverrideSetting;
        private readonly IUserLoyaltyPointsHistoryInfo _userLoyaltyPointsHistoryInfo;

        private readonly IUserRepository _userRepository;
        private readonly IGeneralSettingService _generalSettingService;
        private readonly ApiResponse someIssueInProcessing = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="loyalityGlobalSetting"></param>
        /// <param name="siteLevelOverrideSetting"></param>
        /// <param name="vconfiguration"></param>
        /// <param name="userLoyaltyPointsHistoryInfo"></param>
        ///<param name="userRepository"></param>
        ///<param name="generalSettingService"></param>
        ///<param name="userLeftLoyalityPointsInfo"></param>
        public LoyalityController(ILoyalityGlobalSetting loyalityGlobalSetting,ISiteLevelOverrideSetting siteLevelOverrideSetting, IUserLoyaltyPointsHistoryInfo userLoyaltyPointsHistoryInfo,IUserRepository userRepository, IGeneralSettingService generalSettingService, IConfiguration vconfiguration)
        { 
            _loyalityGlobalSetting = loyalityGlobalSetting;
            _siteLevelOverrideSetting = siteLevelOverrideSetting;
            _userLoyaltyPointsHistoryInfo = userLoyaltyPointsHistoryInfo;
            _userRepository = userRepository;
            _generalSettingService = generalSettingService;
            _configuration = vconfiguration;
           
        }
        /// <summary>
        /// This Api is used to add/update orgloyaltyglobalsettings info.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateOrgLoyaltyGlobalSettings")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateOrgLoyalityGlobalSettings(LoyalityGlobalSettingViewModel model)
        {
            try
            {

                var Id = await _loyalityGlobalSetting.AddEditLoyalityGlobalSettings(model);
                if (Id <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.LoyalityGlobalSuccessfulSetting, "0"));
                }
              
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.LoyalityGlobalSuccessfulSetting, Id));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyality := AddEditLoyalityGlobalSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api called to get the org loyality global settings.
        /// </summary>
        /// <param name="id"></param>
      
        /// <returns></returns>
        [Route("GetOrgLoyaltyGlobalSettings")]
        [HttpGet]
        public async Task<IActionResult> GetOrgLoyalityGlobalSettings(int id)
        {
            try
            {
                var getGlobalSettings = (await _loyalityGlobalSetting.GetOrgLoyalityGlobalSettings(id)).ToList();
                if (getGlobalSettings.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, getGlobalSettings, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyality := GetOrgLoyalityGlobalSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to add/update SiteLevel Override Settings info.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateSiteLevelOverrideSettings")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateSiteLevelOverrideSettings(SiteLevelOverrideSettingApiViewModel model)
        {
            try
            {
                var Id = await _siteLevelOverrideSetting.AddEditSiteLevelOverrideSettings(model);
                if (Id <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SiteLevelOverrideSuccessfulSetting, "0"));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.SiteLevelOverrideSuccessfulSetting, Id));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyality := AddEditSiteLevelOverrideSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api called to get the site level override settings.
        /// </summary>
        /// <param name="id"></param>

        /// <returns></returns>
        [Route("GetSiteLevelOverrideSettings")]
        [HttpGet]
        public async Task<IActionResult> GetSiteLevelOverrideSettings(int id)
        {
            try
            {
                var getSiteLevelSettings = (await _siteLevelOverrideSetting.GetSiteLevelOverrideSettings(id)).ToList();
                
                if (getSiteLevelSettings.Count <= 0)
                { 
                    SiteLevelOverrideSettingsDto ob = new SiteLevelOverrideSettingsDto();
                    //ob.siteLevelBitePayRatio = 0;
                    //ob.siteLevelUserStatusRegularRatio = 0;
                    //ob.siteLevelDcbFlexRatio = 0;
                    //ob.siteLevelUserStatusVipRatio = 0;
                    //ob.programId = 0;
                    //ob.createdDate = DateTime.Now;
                    //ob.modifiedDate=DateTime.Now;

                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned,ob));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, getSiteLevelSettings, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyality := GetSiteLevelOverrideSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to add user loyalty points based on transaction.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("LoyaltyCalculations")]
        [HttpPost]
       // [Authorize]
        public async Task<IActionResult> AddUserLoyaltyPoints(UserLoyaltyPointsHistoryViewModel model)
        {
            try
            {
                var usertype =await _userRepository.GetUserType(Convert.ToInt32( model.userId));
                decimal firsttransactionbonus = 0;                             
                var setting =(await _generalSettingService.GetGeneralSettingValueByKeyName(Constants.GeneralSettingsConstants.UserFirstTransactionBonus)).FirstOrDefault() ;
                firsttransactionbonus =Convert.ToDecimal( setting.Value);
                var  organisationId = Convert.ToInt32(_configuration["SodexhoOrgId"]);
                //get loyaltysettings
                var getGlobalSettings = (await _loyalityGlobalSetting.GetOrgLoyalityGlobalSettings(organisationId)).ToList().FirstOrDefault();
                var getsitelevelsettingsinfo= (await _siteLevelOverrideSetting.GetSiteLevelOverrideSettingsByUserProgId(Convert.ToInt32( model.userId))).ToList().FirstOrDefault();


                var getuserLoyaltypointsinfo = (await _userLoyaltyPointsHistoryInfo.GetUserLoyaltyPointsHistory(Convert.ToInt32( model.userId))).ToList();
             
                //points calculation
                if (usertype=="vip")
                {
                    if (getuserLoyaltypointsinfo.Count() > 0)
                    {
                        if (getsitelevelsettingsinfo!=null)
                        {
                            model.pointsEarned = (getsitelevelsettingsinfo.siteLevelUserStatusVipRatio * getsitelevelsettingsinfo.siteLevelBitePayRatio * model.transactionAmount);
                        }
                        else
                        {
                            model.pointsEarned = (getGlobalSettings.userStatusVipRatio * getGlobalSettings.bitePayRatio * model.transactionAmount);
                        }
                    }
                    else
                    {
                        if (getsitelevelsettingsinfo!=null)
                        {
                            model.pointsEarned = (getsitelevelsettingsinfo.siteLevelUserStatusVipRatio * getsitelevelsettingsinfo.siteLevelBitePayRatio * model.transactionAmount) + firsttransactionbonus;
                        }
                        else
                        {
                            model.pointsEarned = (getGlobalSettings.userStatusVipRatio * getGlobalSettings.bitePayRatio * model.transactionAmount) + firsttransactionbonus;

                        }
                    }
                    
                }
                else if(usertype=="bite pay")
                {
                    if (getuserLoyaltypointsinfo.Count() > 0)
                    {
                        if (getsitelevelsettingsinfo != null)
                        {
                            model.pointsEarned = (getsitelevelsettingsinfo.siteLevelUserStatusRegularRatio * getsitelevelsettingsinfo.siteLevelBitePayRatio * model.transactionAmount);
                        }
                        else
                        {
                            model.pointsEarned = (getGlobalSettings.userStatusRegularRatio * getGlobalSettings.bitePayRatio * model.transactionAmount);

                        }
                    }
                    else
                    {
                        if (getsitelevelsettingsinfo != null)
                        {
                            model.pointsEarned = (getsitelevelsettingsinfo.siteLevelUserStatusRegularRatio * getsitelevelsettingsinfo.siteLevelBitePayRatio * model.transactionAmount) + firsttransactionbonus;
                        }
                        else
                        {
                            model.pointsEarned = (getGlobalSettings.userStatusRegularRatio * getGlobalSettings.bitePayRatio * model.transactionAmount) + firsttransactionbonus;

                        }
                    }
                   
                }
                else
                {
                    if (getuserLoyaltypointsinfo.Count() > 0)
                    {
                        if (getsitelevelsettingsinfo != null)
                        {
                            model.pointsEarned = (getsitelevelsettingsinfo.siteLevelUserStatusRegularRatio * getsitelevelsettingsinfo.siteLevelDcbFlexRatio * model.transactionAmount);
                        }
                        else
                        {
                            model.pointsEarned = (getGlobalSettings.userStatusRegularRatio * getGlobalSettings.dcbFlexRatio * model.transactionAmount);

                        }
                    }
                    else
                    {
                        if (getsitelevelsettingsinfo != null)
                        {
                            model.pointsEarned = (getsitelevelsettingsinfo.siteLevelUserStatusRegularRatio * getsitelevelsettingsinfo.siteLevelDcbFlexRatio * model.transactionAmount) + firsttransactionbonus;
                        }
                        else
                        {
                            model.pointsEarned = (getGlobalSettings.userStatusRegularRatio * getGlobalSettings.dcbFlexRatio * model.transactionAmount) + firsttransactionbonus;

                        }
                    }
                }
                if (getuserLoyaltypointsinfo.Count > 0)
                {
                    foreach (var obj in getuserLoyaltypointsinfo)
                    {
                        if (obj.leftOverPoints == 0)
                        {
                            model.totalPoints = model.pointsEarned + obj.totalPoints;
                        }
                        else
                        {
                            model.totalPoints = model.pointsEarned + obj.leftOverPoints;
                            model.isThresholdReached = false;
                        }

                    }
                }
                else
                {
                    model.totalPoints = model.pointsEarned;
                    model.isThresholdReached = false;
                    model.leftOverPoints = 0;
                }
                // check total loyalty points with threshold value
                if(model.totalPoints >= getGlobalSettings.loyalityThreshhold)
                {
                    model.isThresholdReached = true;
                    model.leftOverPoints = model.totalPoints  - getGlobalSettings.loyalityThreshhold;
                    model.rewardAmount = (model.totalPoints / getGlobalSettings.loyalityThreshhold) * getGlobalSettings.globalReward < getGlobalSettings.globalReward ? 0 : (model.totalPoints / getGlobalSettings.loyalityThreshhold) * getGlobalSettings.globalReward;
                }

                var Id = await _userLoyaltyPointsHistoryInfo.AddUserLoyaltyPointsHistory(model);
                if (Id <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserLoyaltyPointshistorySuccessful, "0"));
                }
                 return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserLoyaltyPointshistorySuccessful, Id));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyalty := AddUserLoyaltyPoints)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api called to get the user loyalty points history.
        /// </summary>
        /// <param name="id"></param>

        /// <returns></returns>
        [Route("GetUserLoyaltyPointsHistory")]
        [HttpGet]
        public async Task<IActionResult> GetUserLoyaltyPointsHistory(int id)
        {
            try
            {

                var getinfo = (await _userLoyaltyPointsHistoryInfo.GetUserLoyaltyPointsHistory(id)).ToList();
                if (getinfo.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, getinfo, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyalty := GetUserLoyaltyPointsHistory)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


       

    }
}