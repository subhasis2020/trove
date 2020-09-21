using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This API contains all the methods which mobile app uses to get the rewards data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IPromotions _promotions;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRewardsProgressLinkingService _userRewardsProgressLinking;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="promotions"></param>
        /// <param name="userRepository"></param>
        /// <param name="configuration"></param>
        /// <param name="userRewardsProgressLinking"></param>
        public RewardsController(IPromotions promotions, IUserRepository userRepository, IConfiguration configuration, IUserRewardsProgressLinkingService userRewardsProgressLinking)
        {
            _promotions = promotions;
            _userRepository = userRepository;
            _configuration = configuration;
            _userRewardsProgressLinking = userRewardsProgressLinking;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("UserAchievedRewards")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAchievedRewardsByUser()
        {
            try
            {
                var userIdClaimForAchievedRewards = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForAchievedRewards = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForAchievedRewards = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForAchievedRewards, sessionIdClaimForAchievedRewards)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get All Organisations With Program and Account Type. */
                var prmDetailsForAchievedRewards = (await _promotions.GetAchievedRewardDetailInformation(userIdClaimForAchievedRewards, programIdClaimForAchievedRewards, _configuration["ServiceAPIURL"])).ToList();
                if (prmDetailsForAchievedRewards == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prmDetailsForAchievedRewards, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Rewards := GetAchievedRewardsByUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("AllRewardsInfo")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllRewardsForUser()
        {
            try
            {
                var userIdClaimAllRewards = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimAllRewards = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimAllRewards = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimAllRewards, sessionIdClaimAllRewards)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get All Organisations With Program and Account Type. */
                var prmDetailsAllRewards = (await _promotions.GetAllRewardsInformation(userIdClaimAllRewards, programIdClaimAllRewards, _configuration["ServiceAPIURL"])).ToList();
                if (prmDetailsAllRewards == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prmDetailsAllRewards, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Rewards := GetAllRewardsForUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This method is used to post the reward for redeem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RedeemReward")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostRewardForRedeem(RewardModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimPostRewards = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimPostRewards = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimPostRewards, sessionIdClaimPostRewards)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var oModelPostRewards = await _userRewardsProgressLinking.GetDataByIdAsync(new { Id = model.RewardProgressId });
                if (oModelPostRewards != null)
                {
                    oModelPostRewards.isRedeemed = true;
                }
                var prmDetailsPostRewards = await _userRewardsProgressLinking.UpdateAsync(oModelPostRewards, new { Id = model.RewardProgressId });
                if (prmDetailsPostRewards <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prmDetailsPostRewards, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Rewards := GetAllRewardsForUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }
    }
}