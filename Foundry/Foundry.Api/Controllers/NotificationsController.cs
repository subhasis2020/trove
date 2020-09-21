using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ElmahCore;
using Foundry.Api.Attributes;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Foundry.Services.PartnerNotificationsLogs;
namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include methods for notification data fetching.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationSettingsService _notification;
        private readonly IUserNotificationSettingsService _userNotification;
        private readonly IUserRepository _userRepository;
        private readonly IUserPushedNotificationService _userPushedNotificationService;
        private readonly IPartnerNotificationsLogServicer _partnerNotificationsLogServicer;
        private readonly ApiResponse NoSessionMatchExist=new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true);
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="userRepository"></param>
        /// <param name="userNotification"></param>
        /// <param name="userPushedNotificationService"></param>
        public NotificationsController(INotificationSettingsService notification, IUserRepository userRepository, IUserNotificationSettingsService userNotification, IUserPushedNotificationService userPushedNotificationService,IPartnerNotificationsLogServicer partnerNotificationsLogServicer  )
        {
            _notification = notification;
            _userRepository = userRepository;
            _userNotification = userNotification;
            _userPushedNotificationService = userPushedNotificationService;
            _partnerNotificationsLogServicer = partnerNotificationsLogServicer;
        }

        /// <summary>
        /// This Api is called to get user notification settings from user.
        /// </summary>
        /// <returns></returns>
        [Route("UserNotifications")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> UserNotifications()
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok();
                }
                var userNotifications = await _notification.GetUserNotificationsSettings(userIdClaim);
                if (userNotifications == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationNotExists));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserNotificationExists, userNotifications));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notifications := UserNotifications)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to update user notification settings for the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateUserNotificationSettings")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> UpdateUserNotificationSettings(UserNotificationSettingsModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimForUserNotifications = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForUserNotifications = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;

                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForUserNotifications, sessionIdClaimForUserNotifications)))
                {
                    return Ok(NoSessionMatchExist);
                }
                /*Mapping of data from UserNotificationsSettingModel to UserNotificationsSettings*/
                var updateUserNotificationsResponse = await _userNotification.UpdateUserNotificationsSettings(model.notificationId, Convert.ToBoolean(model.isNotificationEnabled), userIdClaimForUserNotifications, Convert.ToBoolean(model.allAboveEnabled));
                if (updateUserNotificationsResponse == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserNotificationSettingsUpdated, updateUserNotificationsResponse));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notifications := UpdateUserNotificationSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
            }

        }

        /// <summary>
        /// Get the user pushed notification list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UserPushedNotificationList")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetUserPushedNotificationList(UserPushedNotificatiosModel model)
        {
            try
            {
                var userIdClaimUserpushnotlst = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimUserpushnotlst = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimUserpushnotlst = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var userDeviceIdUserpushnotlst = Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "UserDeviceId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var UserDeviceTypeUserpushnotlst = Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "UserDeviceType".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimUserpushnotlst, sessionIdClaimUserpushnotlst)))
                {
                    return Ok(NoSessionMatchExist);
                }
                /*Mapping of data from UserNotificationsSettingModel to UserNotificationsSettings*/
                var userNotifications = await _userPushedNotificationService.GetUserPushNotificationsList(model.notificationTypeId, model.pageNumber, model.pageSize, userIdClaimUserpushnotlst, programIdClaimUserpushnotlst, userDeviceIdUserpushnotlst, UserDeviceTypeUserpushnotlst);
                if (userNotifications == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserNotificationSettingsUpdated, userNotifications));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notifications := UpdateUserNotificationSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("UserPushedNotificationCount")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetUserPushedNotificationListCount()
        {
            try
            {
                var userIdClaimGetUserpushCount = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimGetUserpushCount = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimGetUserpushCount = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var userDeviceIdGetUserpushCount = Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "UserDeviceId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var UserDeviceTypeGetUserpushCount = Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "UserDeviceType".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimGetUserpushCount, sessionIdClaimGetUserpushCount)))
                {
                    return Ok(NoSessionMatchExist);
                }
                /*Mapping of data from UserNotificationsSettingModel to UserNotificationsSettings*/
                var userNotifications = await _userPushedNotificationService.GetUserPushNotifictaionsUnreadCount(userIdClaimGetUserpushCount, programIdClaimGetUserpushCount, userDeviceIdGetUserpushCount, UserDeviceTypeGetUserpushCount);

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserNotificationSettingsUpdated, userNotifications));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notifications := GetUserPushedNotificationListCount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
            }
        }

        /// <summary>
        /// Get all notification logs.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        ///  <param name="sortColumnName"></param>
        ///   <param name="sortOrderDirection"></param>
        /// <returns></returns>
        [Route("GetAllNotificationLogs")]
        [HttpGet]
        //[Authorize]
      //  [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetAllNotificationLogs(int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection)
        {
            try
            {

                var userNotifications = await _userPushedNotificationService.GetPushNotificationLogs(pageNumber, pageSize, sortColumnName, sortOrderDirection);
                if (userNotifications == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userNotifications));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notifications := NotificationLogsList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
            }
        }

        /// <summary>
        /// Get all notification logs with filter.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        ///  <param name="sortColumnName"></param>
        ///   <param name="sortOrderDirection"></param>
        /// <returns></returns>
        [Route("GetAllNotificationLogsWithFilter")]
        [HttpGet]
    
        public async Task<IActionResult> GetAllNotificationLogsWithFilter(int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection,string apiname,string status,string date,string programid)
        {
            try
            {

                var userNotifications = await _userPushedNotificationService.GetPushNotificationLogsWithFilter(pageNumber, pageSize, sortColumnName, sortOrderDirection,apiname,status,date,programid);
                if (userNotifications == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotificationSettingsNotUpdated));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userNotifications));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notifications := NotificationLogsList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
            }
        }


        /// <summary>
        /// This Api is called to get the All api names.
        /// </summary>
        /// <returns></returns>
        [Route("GetAllApiNames")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllApiNames()
        {
            try
            {
                var names =await _partnerNotificationsLogServicer.GetApiNames();
                if (names == null )
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, names, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, names, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notification := GetAllApiNames)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }

        /// <summary>
        /// This Api is called to get the All api names.
        /// </summary>
        /// <returns></returns>
        [Route("GetAllPrograms")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllPrograms(int organisationId)
        {
            try
            {
                var prg = await _partnerNotificationsLogServicer.GetAllPrograms(organisationId);
                if (prg == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, prg, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prg, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notification := GetAllApiNames)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }
        /// <summary>
        /// This Api is called to get the All status.
        /// </summary>
        /// <returns></returns>
        [Route("GetAllStatus")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllStatus()
        {
            try
            {
                var status = await _partnerNotificationsLogServicer.GetAllStatus();
                if (status == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, status, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, status, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Notification := GetAllStatus)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }
    }
}