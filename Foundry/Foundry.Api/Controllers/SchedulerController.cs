using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
using Foundry.Api.Models;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.LogService;
using Foundry.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This API is used to schedule the rewards notification.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly IUserNotificationSettingsService _userNotificationSettingsService;
        private readonly IUserPushedNotificationService _userPushedNotificationService;
        private readonly IUserRepository _userRepository;
        private readonly IPromotions _promotions;
        private readonly IUserFavoriteService _userFavorite;
        private readonly IGeneralSettingService _setting;
        private readonly IPrograms _program;
        private IHttpContextAccessor _accessor;
        private readonly ILoggerManager _logger;
        private IConfiguration _configuration;
        string notificationMessage = string.Empty, notificationTitle = string.Empty;
        /// <summary>
        /// This constructor is used to inject services in API.
        /// </summary>
        /// <param name="userNotificationSettingsService"></param>
        /// <param name="userPushedNotificationService"></param>
        /// <param name="userRepository"></param>
        /// <param name="userFavorite"></param>
        /// <param name="promotions"></param>
        /// <param name="setting"></param>
        /// <param name="program"></param>
        /// <param name="Configuration"></param>
        /// <param name="accessor"></param>
        public SchedulerController(IUserNotificationSettingsService userNotificationSettingsService, IUserPushedNotificationService userPushedNotificationService,
            IUserRepository userRepository, IUserFavoriteService userFavorite, IPromotions promotions
            , IGeneralSettingService setting, IPrograms program, IHttpContextAccessor accessor, IConfiguration Configuration)
        {
            _userNotificationSettingsService = userNotificationSettingsService;
            _userPushedNotificationService = userPushedNotificationService;
            _userRepository = userRepository;
            _userFavorite = userFavorite;
            _promotions = promotions;
            _setting = setting;
            _program = program;
            _accessor = accessor;
            _configuration = Configuration;
        }

        /// <summary>
        /// This API is called to get the Scheduler for Offers and Rewards.
        /// </summary>
        /// <returns></returns>
        [Route("ScheduleOfferNRewards")]
        [HttpGet]
        public async Task<IActionResult> GetSchedulerForOffersNRewards()
        {
            // need to crosscheck file path
            if (IsAllowedIp())
            {
                _logger.LogDebug("Scheduler :calling ScheduleOfferNRewards Method");
                await NewRewardSchedule();
                await NewOffersSchedule();
                await RewardAheadToCompleteSchedule();
                await WonRewardsSchedule();
            }
            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, "", 0, ""));
        }

        [NonAction]
        private async Task<int> NewRewardSchedule()
        {
            //Rewards Scheduler
            try
            {
                var userRewards = await _promotions.GetUserRewardsBasedOnCurrentDate();
                if (userRewards.Count > 0)
                {
                    var programsLst = await _program.GetProgramListBasedOnIds(userRewards.Select(x => x.ProgramId).ToList());
                    await NewRewardScheduleRefactor(userRewards, programsLst);
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return 1;
        }

        private async Task NewRewardScheduleRefactor(List<Domain.Dto.RewardsOnDate> userRewards, List<Domain.Dto.ProgramInfoDto> programsLst)
        {
            if (programsLst.Count > 0)
            {
                foreach (var prg in programsLst)
                {
                    if (prg.IsAllNotificationShow)
                    {
                        var userDeviceIds = await _userRepository.GetUserDeviceTokenBasedOnProgram(prg.id);
                        await NewRewardScheduleRefactorInside(userRewards, prg, userDeviceIds);
                    }
                }
            }
        }

        private async Task<List<Domain.Dto.UserDeviceDto>> NewRewardScheduleRefactorInside(List<Domain.Dto.RewardsOnDate> userRewards, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds)
        {
            if (userDeviceIds.Count > 0)
            {
                var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                PushNotifications push = new PushNotifications();
                var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Awards);
                if (usrNotify.Count > 0)
                {
                    notificationMessage = MessagesConstants.NewRewardNotificationMessage;
                    notificationTitle = MessagesConstants.NewRewardNotificationTitle;
                    userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();
                    if (userDeviceIds.Count > 0)
                    {
                        await NewRewardScheduleRefactorInsideAgain(userRewards, prg, userDeviceIds, serverApiKey, push);
                    }
                }
            }

            return userDeviceIds;
        }

        private async Task NewRewardScheduleRefactorInsideAgain(List<Domain.Dto.RewardsOnDate> userRewards, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds, GeneralSetting serverApiKey, PushNotifications push)
        {
            foreach (var item in userRewards)
            {
                if (item.ProgramId == prg.id)
                {
                    try
                    {
                        await SendPushBulkWithAddPushAsync(userDeviceIds, serverApiKey, push, item);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                }
            }
        }

        private async Task SendPushBulkWithAddPushAsync(List<Domain.Dto.UserDeviceDto> userDeviceIds, GeneralSetting serverApiKey, PushNotifications push, Domain.Dto.RewardsOnDate item)
        {
            await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", item.RewardId.ToString(), "awards", "icon", "awards", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "ProgressRewards", item.MerchantId);
            await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
            {
                notificationMessage = notificationMessage,
                notificationTitle = notificationTitle,
                notificationType = (int)NotificationSettingsEnum.Awards,
                referenceId = item.RewardId,
                createdBy = item.CreatedBy,
                modifiedBy = item.CreatedBy,
                ProgramId = item.ProgramId,
                IsRedirect = true,
                NotificationSubType = "ProgressRewards",
                CustomReferenceId = item.MerchantId
            });
        }

        [NonAction]
        private async Task<int> NewOffersSchedule()
        {
            // Offers
            try
            {
                var userOffers = await _promotions.GetUserOffersBasedOnCurrentDate();
                if (userOffers.Count > 0)
                {
                    var programsLst = await _program.GetProgramListBasedOnIds(userOffers.Select(x => x.ProgramId).ToList());
                    await NewOffersScheduleRefactor(userOffers, programsLst);
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return 1;
        }

        private async Task NewOffersScheduleRefactor(List<Domain.Dto.RewardsOnDate> userOffers, List<Domain.Dto.ProgramInfoDto> programsLst)
        {
            if (programsLst.Count > 0)
            {
                foreach (var prg in programsLst)
                {
                    if (prg.IsAllNotificationShow)
                    {
                        var userDeviceIds = await _userRepository.GetUserDeviceTokenBasedOnProgram(prg.id);
                        await NewOffersScheduleRefactorInside(userOffers, prg, userDeviceIds);
                    }
                }
            }
        }

        private async Task<List<Domain.Dto.UserDeviceDto>> NewOffersScheduleRefactorInside(List<Domain.Dto.RewardsOnDate> userOffers, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds)
        {
            if (userDeviceIds.Count > 0)
            {
                var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                PushNotifications push = new PushNotifications();
                var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Offers);
                if (usrNotify.Count > 0)
                {
                    userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();
                    if (userDeviceIds.Count > 0)
                    {
                        await NewOffersScheduleRefactorInsideAgain(userOffers, prg, userDeviceIds, serverApiKey, push);
                    }
                }
            }

            return userDeviceIds;
        }
        /// <summary>
        /// NewOffersScheduleRefactorInsideAgain
        /// </summary>
        /// <param name="userOffers"></param>
        /// <param name="prg"></param>
        /// <param name="userDeviceIds"></param>
        /// <param name="serverApiKey"></param>
        /// <param name="push"></param>
        /// <returns></returns>
        private async Task NewOffersScheduleRefactorInsideAgain(List<Domain.Dto.RewardsOnDate> userOffers, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds, GeneralSetting serverApiKey, PushNotifications push)
        {
            try
            {
                foreach (var item in userOffers)
                {
                    if (item.ProgramId == prg.id)
                    {
                        var chkFavorites = await _userFavorite.GetUsersListForFavoriteMerchant(userDeviceIds.Select(m => m.Id).ToList(), item.MerchantId);
                        var userFavorites = userDeviceIds.Where(x => chkFavorites.Contains(x.Id)).ToList();
                        await PushToFavorites(serverApiKey, push, item, userFavorites);
                        var userAll = userDeviceIds.Where(x => !chkFavorites.Contains(x.Id)).ToList();
                        await UserAllPushNotifications(serverApiKey, push, item, userAll);
                        await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
                        {
                            notificationMessage = notificationMessage,
                            notificationTitle = notificationTitle,
                            notificationType = (int)NotificationSettingsEnum.Offers,
                            referenceId = item.RewardId,
                            createdBy = item.CreatedBy,
                            modifiedBy = item.CreatedBy,
                            ProgramId = item.ProgramId,
                            IsRedirect = true,
                            NotificationSubType = "Offers",
                            CustomReferenceId = item.MerchantId
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));

            }

        }

        private async Task UserAllPushNotifications(GeneralSetting serverApiKey, PushNotifications push, Domain.Dto.RewardsOnDate item, List<Domain.Dto.UserDeviceDto> userAll)
        {
            if (userAll.Count > 0)
            {
                notificationMessage = MessagesConstants.NewOfferNotificationMessage;
                notificationTitle = MessagesConstants.NewOfferNotificationTitle;
                await push.SendPushBulk(userAll.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", item.RewardId.ToString(), "offer", "icon", "offer", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "Offers", item.MerchantId);
            }
        }

        private async Task PushToFavorites(GeneralSetting serverApiKey, PushNotifications push, Domain.Dto.RewardsOnDate item, List<Domain.Dto.UserDeviceDto> userFavorites)
        {
            if (userFavorites.Count > 0)
            {
                notificationMessage = MessagesConstants.NewOfferFavNotificationMessage;
                notificationTitle = MessagesConstants.NewOfferFavNotificationTitle;
                await push.SendPushBulk(userFavorites.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", item.RewardId.ToString(), "favorite", "icon", "offer", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "Offers", item.MerchantId);
            }
        }

        [NonAction]
        private async Task<int> RewardAheadToCompleteSchedule()
        {
            //Ahead To Completion of Rewards
            //Rewards Scheduler
            try
            {
                var userAheadToCompleteRewards = await _promotions.GetUsersToAheadCompleteRewards();
                if (userAheadToCompleteRewards.Count > 0)
                {
                    var programsLst = await _program.GetProgramListBasedOnIds(userAheadToCompleteRewards.Select(x => x.ProgramId).ToList());
                    await RewardAheadToCompleteScheduleRefactor(userAheadToCompleteRewards, programsLst);
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return 1;
        }

        private async Task RewardAheadToCompleteScheduleRefactor(List<Domain.Dto.RewardsOnDate> userAheadToCompleteRewards, List<Domain.Dto.ProgramInfoDto> programsLst)
        {
            if (programsLst.Count > 0)
            {
                foreach (var prg in programsLst)
                {
                    if (prg.IsAllNotificationShow)
                    {
                        var userDeviceIds = await _userRepository.GetUserDeviceTokenBasedOnProgram(prg.id);
                        await RewardAheadToCompleteScheduleRefactorInside(userAheadToCompleteRewards, prg, userDeviceIds);
                    }
                }
            }
        }

        private async Task<List<Domain.Dto.UserDeviceDto>> RewardAheadToCompleteScheduleRefactorInside(List<Domain.Dto.RewardsOnDate> userAheadToCompleteRewards, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds)
        {
            if (userDeviceIds.Count > 0)
            {
                var serverApiKeyForCompleteSchedule = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                PushNotifications pushForCompleteSchedule = new PushNotifications();
                var usrNotifyForCompleteSchedule = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Awards);
                if (usrNotifyForCompleteSchedule.Count > 0)
                {
                    userDeviceIds = userDeviceIds.Where(x => usrNotifyForCompleteSchedule.Contains(x.Id)).ToList();
                    if (userDeviceIds.Count > 0)
                    {
                        await RewardAheadToCompleteScheduleRefactorInsideAgain(userAheadToCompleteRewards, prg, userDeviceIds, serverApiKeyForCompleteSchedule, pushForCompleteSchedule);
                    }
                }
            }

            return userDeviceIds;
        }

        private async Task RewardAheadToCompleteScheduleRefactorInsideAgain(List<Domain.Dto.RewardsOnDate> userAheadToCompleteRewards, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds, GeneralSetting serverApiKey, PushNotifications push)
        {
            foreach (var item in userAheadToCompleteRewards)
            {
                if (item.ProgramId == prg.id)
                {
                    try
                    {
                        if (item.OfferSubTypeId == 3)
                        {
                            notificationMessage = MessagesConstants.AheadRewardNotificationMessage + item.MerchantName;
                            notificationTitle = MessagesConstants.AheadRewardNotificationTitle;
                        }
                        else
                        {
                            notificationMessage = MessagesConstants.AheadRewardNotificationForAmtMessage + item.MerchantName;
                            notificationTitle = MessagesConstants.AheadRewardNotificationForAmtTitle;
                        }
                        await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", item.RewardId.ToString(), "awards", "icon", "awards", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "ProgressRewards", item.MerchantId);
                        await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
                        {
                            notificationMessage = notificationMessage,
                            notificationTitle = notificationTitle,
                            notificationType = (int)NotificationSettingsEnum.Awards,
                            referenceId = item.RewardId,
                            createdBy = item.CreatedBy,
                            modifiedBy = item.CreatedBy,
                            ProgramId = item.ProgramId,
                            IsRedirect = true,
                            NotificationSubType = "ProgressRewards",
                            CustomReferenceId = item.MerchantId
                        });

                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));

                    }
                }
            }
        }

        [NonAction]
        private async Task<int> WonRewardsSchedule()
        {

            // Won Awards
            try
            {
                var userCompletedRewards = await _promotions.GetUsersCompletedRewardsNotify();
                if (userCompletedRewards.Count > 0)
                {
                    var programsLst = await _program.GetProgramListBasedOnIds(userCompletedRewards.Select(x => x.ProgramId).ToList());
                    await WonRewardsScheduleRefactor(userCompletedRewards, programsLst);
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return 1;
        }

        private async Task WonRewardsScheduleRefactor(List<Domain.Dto.RewardsOnDate> userCompletedRewards, List<Domain.Dto.ProgramInfoDto> programsLst)
        {
            if (programsLst.Count > 0)
            {
                foreach (var prg in programsLst)
                {
                    if (prg.IsAllNotificationShow)
                    {
                        var userDeviceIds = await _userRepository.GetUserDeviceTokenBasedOnProgram(prg.id);
                        await WonRewardsScheduleRefactorInside(userCompletedRewards, prg, userDeviceIds);
                    }
                }
            }
        }

        private async Task<List<Domain.Dto.UserDeviceDto>> WonRewardsScheduleRefactorInside(List<Domain.Dto.RewardsOnDate> userCompletedRewards, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds)
        {
            if (userDeviceIds.Count > 0)
            {
                var serverApiKeyForWonRewards = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                PushNotifications pushForWinRewards = new PushNotifications();
                var usrNotifyForWinRewards = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Awards);
                if (usrNotifyForWinRewards.Count > 0)
                {
                    userDeviceIds = userDeviceIds.Where(x => usrNotifyForWinRewards.Contains(x.Id)).ToList();
                    if (userDeviceIds.Count > 0)
                    {
                        await WonRewardsScheduleRefactorInsideAgain(userCompletedRewards, prg, userDeviceIds, serverApiKeyForWonRewards, pushForWinRewards);
                    }
                }
            }
            return userDeviceIds;
        }

        private async Task WonRewardsScheduleRefactorInsideAgain(List<Domain.Dto.RewardsOnDate> userCompletedRewards, Domain.Dto.ProgramInfoDto prg, List<Domain.Dto.UserDeviceDto> userDeviceIds, GeneralSetting serverApiKey, PushNotifications push)
        {
            foreach (var item in userCompletedRewards)
            {
                if (item.ProgramId == prg.id)
                {
                    try
                    {
                        notificationMessage = MessagesConstants.WonRewardNotificationMessage + item.MerchantName;
                        notificationTitle = MessagesConstants.WonRewardNotificationTitle;

                        await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", item.RewardId.ToString(), "awards", "icon", "awards", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "CompleteRewards", item.MerchantId);
                        await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
                        {
                            notificationMessage = notificationMessage,
                            notificationTitle = notificationTitle,
                            notificationType = (int)NotificationSettingsEnum.Awards,
                            referenceId = item.RewardId,
                            createdBy = item.CreatedBy,
                            modifiedBy = item.CreatedBy,
                            ProgramId = item.ProgramId,
                            IsRedirect = true,
                            NotificationSubType = "CompleteRewards",
                            CustomReferenceId = item.MerchantId
                        });
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Scheduler := GeSchedulerForOffersNRewards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                }
            }
        }

        private bool IsAllowedIp()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInfo("IP Address : " + ip);
            var getIPs = Convert.ToString(_configuration["AppSettings:IPs"]);
            if (!String.IsNullOrEmpty(ip) && ip.Length > 7 && getIPs.Contains(ip))
            {
                return true;
            }
            return false;
        }
    }
}