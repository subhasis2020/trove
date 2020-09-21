using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using Foundry.Identity;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Foundry.Domain.Constants;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using ElmahCore;
using Foundry.Api.Attributes;
using System.Globalization;
using Foundry.Api.Models;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include methods to add benefactor and other request like reload and invite.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BenefactorController : ControllerBase
    {
        private readonly IBenefactorService _benefactor;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IInvitationService _invite;
        private readonly IBenefactorNotifications _notify;
        private readonly IPrograms _program;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ISMSService _sMSService;
        private readonly IUserRelationsService _relationService;
        private readonly IGeneralSettingService _setting;
        private readonly IUserNotificationSettingsService _userNotificationSettingsService;
        private readonly IUserPushedNotificationService _userPushedNotificationService;

        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="benefactor"></param>
        /// <param name="userRepository"></param>
        /// <param name="emailService"></param>
        /// <param name="invite"></param>
        /// <param name="notify"></param>
        /// <param name="program"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        /// <param name="sMSService"></param>
        /// <param name="relationService"></param>
        /// <param name="setting"></param>
        /// <param name="userNotificationSettingsService"></param>
        /// <param name="userPushedNotificationService"></param>
        public BenefactorController(IBenefactorService benefactor, IUserRepository userRepository, IEmailService emailService,
              IInvitationService invite, IBenefactorNotifications notify, IPrograms program, IMapper mapper, IConfiguration configuration,
            ISMSService sMSService, IUserRelationsService relationService, IGeneralSettingService setting, IUserNotificationSettingsService userNotificationSettingsService, IUserPushedNotificationService userPushedNotificationService)
        {
            _benefactor = benefactor;
            _userRepository = userRepository;
            _emailService = emailService;
            _invite = invite;
            _notify = notify;
            _program = program;
            _mapper = mapper;
            _configuration = configuration;
            _sMSService = sMSService;
            _relationService = relationService;
            _setting = setting;
            _userNotificationSettingsService = userNotificationSettingsService;
            _userPushedNotificationService = userPushedNotificationService;
        }

        /// <summary>
        /// The GetRelationships API returns all the relations exists in the system.
        /// </summary>
        /// <param name="model">SessionCheckModel</param>
        /// <returns>RelatioshipViewModel</returns>
        [Route("GetRelationships")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetAllRelationships(SessionCheckModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identityForRel = User.Identity as ClaimsIdentity;
                var userIdClaimForRel = Convert.ToInt32(identityForRel.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForRel = identityForRel.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForRel, sessionIdClaimForRel)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get all the relations to return. */
                var relationsForRel = _mapper.Map<List<RelationshipDto>>(await _relationService.AllAsync());
                if (relationsForRel.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRelationsExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, relationsForRel));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetAllRelationships) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to invite benefactor from mobile app.
        /// </summary>
        /// <param name="model">BenefactorRegisterModel</param>
        /// <returns></returns>
        [Route("InviteBenefactor")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> InviteBenefactor(BenefactorRegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }

                var identityInviteBenefactor = User.Identity as ClaimsIdentity;
                var userIdClaimInviteBenefactor = Convert.ToInt32(identityInviteBenefactor.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimInviteBenefactor = identityInviteBenefactor.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimInviteBenefactor = Convert.ToInt32(identityInviteBenefactor.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                model.ProgramId = programIdClaimInviteBenefactor;
                model.UserId = userIdClaimInviteBenefactor;
                model.SessionId = sessionIdClaimInviteBenefactor;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimInviteBenefactor, sessionIdClaimInviteBenefactor)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var existingConnectionForInviteBenefactor = await _benefactor.CheckForExistingUserLinkingWithEmail(userIdClaimInviteBenefactor, model.EmailAddress);
                if (existingConnectionForInviteBenefactor != null && (bool)existingConnectionForInviteBenefactor?.IsRequestAccepted.Value && (bool)!existingConnectionForInviteBenefactor?.isDeleted.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ConnectionAlreadyExists));
                }
                var programCheckIdForInviteBenefactor = await _program.CheckProgramExpiration(programIdClaimInviteBenefactor, userIdClaimInviteBenefactor);
                if (programCheckIdForInviteBenefactor <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ProgramExpired));
                }
                var existingInvitation = await _invite.GetExistingInvitation(userIdClaimInviteBenefactor, model.EmailAddress);
                if (existingInvitation != null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvitationAlreadySent));
                }

                var inviteBenefactor = await _invite.AddUpdateInvitationByUser(model);
                if (inviteBenefactor > 0)
                {
                    RelationshipDto relationshipDetail = await InviteBenefactorRefactor(model, userIdClaimInviteBenefactor, programIdClaimInviteBenefactor);
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.InvitationSuccessfulSent, new BenefactorDto { BenefactorImage = model.BenefactorImagePath, EmailAddress = model.EmailAddress, FirstName = model.FirstName, LastName = model.LastName, MobileNumber = model.MobileNumber, BenefactorUserId = inviteBenefactor, IsInvitee = true, RelationshipName = relationshipDetail?.RelationName, IsReloadRequest = false }));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := InviteBenefactor) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
        }

        private async Task<RelationshipDto> InviteBenefactorRefactor(BenefactorRegisterModel model, int userIdClaim, int programIdClaim)
        {
            try
            {
                var userDetail = await _userRepository.GetUserById(userIdClaim);
                var programDetail = await _program.GetProgramById(programIdClaim);
                var emailSMSSettings = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMSEmail)).FirstOrDefault();
                if (emailSMSSettings != null)
                {
                    if (emailSMSSettings.Value == "1")
                    {
                        /* Email Sending */
                        var URLRedirectLink = string.Concat(_configuration["WebURL"], "Account/Index?id=", Cryptography.EncryptPlainToCipher(model.EmailAddress) + "&uid=" + Cryptography.EncryptPlainToCipher(userDetail.Id.ToString()) + "&pid=" + Cryptography.EncryptPlainToCipher(programDetail.Id.ToString()));
                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.BenefactorInvitation);
                      //  template.Body = template.Body.Replace("Trove", "Sodexo");
                       // template.Body = template.Body.Replace("background:linear-gradient(208.63deg, #3952d5 0%, rgba(20,4,185,0.8) 51.1%, #3a55d7 100%)", "");
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(model.FirstName, " ", model.LastName)).Replace("{SenderName}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{link}", URLRedirectLink).Replace("{ProgramName}", programDetail?.Name);
                        await _emailService.SendEmail(model.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    else
                    {
                        try
                        {
                            var smstemplate = await _sMSService.GetSMSTemplateByName(SMSTemplates.BenefactorInvitation);
                            smstemplate.body = smstemplate.body.Replace("{Name}", string.Concat(model.FirstName, " ", model.LastName)).Replace("{SenderName}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{ProgramName}", programDetail?.Name);
                            await _sMSService.SendSMS(_configuration["accountSID"], _configuration["authToken"], model.MobileNumber, smstemplate.body);
                        }
                        catch (Exception ex)
                        {
                            HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := InviteBenefactor) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := ForgotPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            var relationshipDetail = _mapper.Map<RelationshipDto>(await _benefactor.GetRelationById(model.RelationshipId));
            return relationshipDetail;
        }

        /// <summary>
        /// This Api is called to get all the connections(benefactor) of the user irrespective of added and invited.
        /// </summary>
        /// <param name="model">SessionCheckModel</param>
        /// <returns></returns>
        [Route("UserConnections")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetUserConnections(SessionCheckModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identityForUserConnections = User.Identity as ClaimsIdentity;
                var userIdClaimForUserConnections = Convert.ToInt32(identityForUserConnections.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForUserConnections = identityForUserConnections.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaim = Convert.ToInt32(identityForUserConnections.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForUserConnections, sessionIdClaimForUserConnections)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var connectionsForUserConnections = await _benefactor.GetUserConnections(userIdClaimForUserConnections, programIdClaim);
                if (connectionsForUserConnections.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoConnectionsExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, connectionsForUserConnections));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetUserConnections) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to delete the benefactor from mobile app.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteConnection")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> DeleteBenefactor(ConnectionDetailModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identityForDeleteBenefactor = User.Identity as ClaimsIdentity;
                var userIdClaimForDeleteBenefactor = Convert.ToInt32(identityForDeleteBenefactor.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForDeleteBenefactor = identityForDeleteBenefactor.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
               
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForDeleteBenefactor, sessionIdClaimForDeleteBenefactor)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var connectionsForDeleteBenefactor = await _benefactor.DeleteUserConnection(model.Type, userIdClaimForDeleteBenefactor, model.BenefactorId);
                if (connectionsForDeleteBenefactor <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserConnectionNotExists));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ConnectionDeletedSuccessfully, model.BenefactorId));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := DeleteBenefactor) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to reload balance request sent to the benefactor.
        /// </summary>
        /// <param name="model">ConnectionDetailModel</param>
        /// <returns></returns>
        [Route("ReloadBalanceRequest")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> ReloadRequest(ConnectionDetailModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identityForReloadRequest = User.Identity as ClaimsIdentity;
                var userIdClaimForReloadRequest = Convert.ToInt32(identityForReloadRequest.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForReloadRequest = identityForReloadRequest.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForReloadRequest = Convert.ToInt32(identityForReloadRequest.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForReloadRequest, sessionIdClaimForReloadRequest)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var reloadRequest = await _benefactor.ReloadBalanceRequest(userIdClaimForReloadRequest, model.BenefactorId, programIdClaimForReloadRequest, model.Amount);
                if (reloadRequest < 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ProgramExpired));
                }
                if (reloadRequest > 0)
                {
                    try
                    {
                        var usersForReloadRequest = await _userRepository.GetUsersDetailByIds(new List<int> { userIdClaimForReloadRequest, model.BenefactorId });
                        var userDetailForReloadRequest = usersForReloadRequest.FirstOrDefault(x => x.Id == userIdClaimForReloadRequest);
                        var benefactorForReloadRequest = usersForReloadRequest.FirstOrDefault(x => x.Id == model.BenefactorId);
                        var programDetailForReloadRequest = await _program.GetProgramById(programIdClaimForReloadRequest);
                        /* Email Sent */
                        var URLRedirectLink = string.Concat(_configuration["WebURL"], "Benefactor/ReloadRequest?id=", userDetailForReloadRequest.Id, "&reloadRequestId=", reloadRequest, "&programId=", programIdClaimForReloadRequest);
                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.ReloadBalanceRequest);
                        template.Subject = template.Subject.Replace("{SenderName}", string.Concat(userDetailForReloadRequest?.FirstName, " ", userDetailForReloadRequest?.LastName));
                        template.Body = template.Body.Replace("Trove", "Sodexo");
                        template.Body = template.Body.Replace("background:linear-gradient(208.63deg, #3952d5 0%, rgba(20,4,185,0.8) 51.1%, #3a55d7 100%)", "");
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(benefactorForReloadRequest?.FirstName, " ", benefactorForReloadRequest?.LastName)).Replace("{SenderName}", string.Concat(userDetailForReloadRequest?.FirstName, " ", userDetailForReloadRequest?.LastName)).Replace("{link}", URLRedirectLink).Replace("{ProgramName}", programDetailForReloadRequest?.Name).Replace("{AccountName}", "Discretionary");
                        await _emailService.SendEmail(benefactorForReloadRequest?.Email, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Account := ForgotPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }

                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ReloadRequestSuccessfully, model.BenefactorId));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ReloadRequestAlreadySent));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := ReloadRequest) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get user transactions of the users which can be seen by benefactor.
        /// </summary>
        /// <param name="linkedUserId"></param>
        /// <param name="dateMonth"></param>
        /// <param name="plan"></param>
        /// <returns></returns>
        [Route("LinkedUsersTransactions")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LinkedUsersTransactions(int linkedUserId, string dateMonth, string plan)
        {
            try
            {
                DateTime? dateFilter = null;
                if (!string.IsNullOrEmpty(dateMonth))
                {
                    dateFilter = Convert.ToDateTime(dateMonth);
                }

                var transactions = await _benefactor.GetLinkedUsersTransactions(linkedUserId, dateFilter, plan);
                if (transactions.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoTransactionsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, transactions, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := LinkedUsersTransactions) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get the connected users with benefactor.
        /// </summary>
        /// <param name="benefactorId"></param>
        /// <returns></returns>
        [Route("LinkedUsers")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLinkedUsers(int benefactorId)
        {
            try
            {
                var identityForLinkedUser = User.Identity as ClaimsIdentity;
                var userIdClaimForLinkedUser = Convert.ToInt32(identityForLinkedUser.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var linkedUsersForLinkedUser = await _benefactor.GetLinkedUsersOfBenefactor(userIdClaimForLinkedUser);
                if (linkedUsersForLinkedUser.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoLinkedUsersExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, linkedUsersForLinkedUser, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetLinkedUsers) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get the connected users with benefactor.
        /// </summary>
        /// <param name="benefactorId"></param>
        /// <returns></returns>
        [Route("BenefectorDetails")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BenefectorDetails(int benefactorId)
        {
            try
            {
                var identityForLinkedUser = User.Identity as ClaimsIdentity;
                var linkedUsersForLinkedUser = await _benefactor.BenefectorDetails(benefactorId);
                if (linkedUsersForLinkedUser.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoLinkedUsersExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, linkedUsersForLinkedUser, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetLinkedUsers) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        /// <summary>
        /// This Api is called to get linked all users information using benefactor id
        /// </summary>
        /// <param name="benefactorId"></param>
        /// <param name="islink"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("LinkedUsersInformation")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLinkedUsersInformation(int benefactorId, bool islink, int id)
        {
            try
            {
                var identityForLinkUserInfo = User.Identity as ClaimsIdentity;
                var userIdClaimForLinkUserInfo = Convert.ToInt32(identityForLinkUserInfo.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                if (islink)
                {
                    var benefactor = await _userRepository.GetUserBenefactorById(userIdClaimForLinkUserInfo);
                    var inviteCheck = await _invite.GetExistingInvitation(id, benefactor.EmailAddress);
                    //Automatically accept user invitation on create password.
                    if (inviteCheck.CreatedBy != 0 && inviteCheck.relationshipId.Value != 0)
                    {
                        await _invite.AcceptUserInvitation(inviteCheck.CreatedBy, userIdClaimForLinkUserInfo, inviteCheck.programId.Value);
                    }
                }
                var linkedUsersForLinkUserInfo = await _benefactor.GetLinkedUsersInformationOfBenefactor(userIdClaimForLinkUserInfo);
                if (linkedUsersForLinkUserInfo.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoLinkedUsersExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, linkedUsersForLinkUserInfo, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetLinkedUsersInformation) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to reload balance to the user account.
        /// </summary>
        /// <param name="model">ReloadRequestModel</param>
        /// <returns></returns>
        [Route("ReloadAmount")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReloadBalanceUser(ReloadRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                model.BenefactorUserId = userIdClaim;
                var reloadRequest = await _benefactor.ReloadUserBalance(model);
                if (reloadRequest > 0)
                {
                    try
                    {
                        var users = await _userRepository.GetUsersDetailByIds(new List<int> { model.ReloadUserId, model.BenefactorUserId });
                        var userDetail = users.FirstOrDefault(x => x.Id == model.ReloadUserId);
                        var benefactor = users.FirstOrDefault(x => x.Id == model.BenefactorUserId);
                        /* Email Sent */

                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.BalanceReload);
                        //  template.Subject = template.Subject.Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName));
                        //    template.Body = template.Body.Replace("Trove", "Sodexo");
                        //  template.Body = template.Body.Replace("background:linear-gradient(208.63deg, #3952d5 0%, rgba(20,4,185,0.8) 51.1%, #3a55d7 100%)", "");
                        if(model.ReloadUserId==model.BenefactorUserId)
                        {
                            template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName)).Replace("{Amount}", model.ReloadAmount.ToString());
                        }
                        else
                        {
                            template=await _emailService.GetEmailTemplateByName(EmailTemplates.BenefactorAddValue);
                            template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName)).Replace("{Amount}", model.ReloadAmount.ToString());
                        }
                        await _emailService.SendEmail(benefactor?.Email, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Account := ReloadBalanceUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }

                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ReloadBalanceSuccessfully, model.ReloadUserId, 1));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := ReloadBalanceUser) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get the user balance from user transaction table.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("CurrentBalanceUser")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckUserBalance(int userId)
        {
            try
            {
                var balance = await _benefactor.GetRemainingBalanceOfUser(userId);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.BalanceReturnedSuccessfully, Math.Round(balance, 2), 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := CheckUserBalance) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to check reload rule of the user that either it is set or not.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="benefactorId"></param>
        /// <returns></returns>
        [Route("ReloadRuleOfUser")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckReloadRuleOfUser(int userId, int benefactorId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var reloadRule = _mapper.Map<ReloadRuleDto>(await _benefactor.GetReloadRuleOfUser(userId, userIdClaim));
                if (reloadRule != null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ReloadRuleData, reloadRule, 1));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoReloadRuleData));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := CheckReloadRuleOfUser) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }




        /// <summary>
        /// This Api is called to check reload rule of the user that either it is set or not.
        /// </summary>
        /// <param name="userId"></param>
        
        /// <returns></returns>
        [Route("GetAllReloadRuleOfUser")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllReloadRuleOfUser(int userId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var reloadRule = await _benefactor.GetAllReloadRuleforUser(userId);
                if (reloadRule != null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ReloadRuleData, reloadRule, 1));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoReloadRuleData));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := CheckReloadRuleOfUser) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }



        /// <summary>
        /// This Api is called to get the notifications sent by users to benefactor for invite and reload balance request.
        /// </summary>
        /// <param name="benefactorId"></param>
        /// <returns></returns>
        [Route("BenefactorNotifications")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBenefactorNotifications(int benefactorId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var notifications = await _notify.GetBenefactorNotifications(userIdClaim);
                if (notifications.Count <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoNotificationsExist)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.NotificationsReturnedSuccessfully, notifications, 1));

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetBenefactorNotifications) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is called to delete the invitation sent by user for connection.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteInvitation")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteInvitation(InvitationConfirmationDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var notifications = await _invite.DeleteUserInvitation(model.UserId, userIdClaim);
                if (notifications > 0)
                {
                    try
                    {
                        string notificationMessage = MessagesConstants.RejectionConnectionNotificationMessage, notificationTitle = MessagesConstants.RejectionConnectionNotificationTitle;

                        var programNotificationSetCheck = await _program.FindAsync(new { id = model.ProgramId });
                        await DeleteInvitationRefactor(model, userIdClaim, notificationMessage, notificationTitle, programNotificationSetCheck);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := Push Notification Issue(Benefactor := DeleteInvitation) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));

                    }

                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.InvitationDeletedSuccessfully, notifications, 1));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := DeleteInvitation) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
        }

        private async Task DeleteInvitationRefactor(InvitationConfirmationDto model, int userIdClaim, string notificationMessage, string notificationTitle, Domain.DbModel.Program programNotificationSetCheck)
        {
            if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
            {
                var userDeviceIds = await _userRepository.GetUserDeviceTokenByUserIdNdProgram(model.ProgramId, model.UserId);
                if (userDeviceIds.Count > 0)
                {
                    var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Personal);
                    await DeleteInvitationRefactorInsideRefactor(notificationMessage, notificationTitle, userDeviceIds, usrNotify);
                }
            }
            await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
            {
                notificationMessage = notificationMessage,
                notificationTitle = notificationTitle,
                notificationType = (int)NotificationSettingsEnum.Personal,
                referenceId = 1,
                createdBy = userIdClaim,
                modifiedBy = userIdClaim,
                ProgramId = model.ProgramId,
                userId = model.UserId,
                IsRedirect = false,
                NotificationSubType = "UserConnection",
                CustomReferenceId = 0
            });
        }

        private async Task<List<UserDeviceDto>> DeleteInvitationRefactorInsideRefactor(string notificationMessage, string notificationTitle, List<UserDeviceDto> userDeviceIds, List<int> usrNotify)
        {
            if (usrNotify.Count > 0)
            {
                userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();
                var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                PushNotifications push = new PushNotifications();
                if (userDeviceIds.Count > 0)
                {
                    await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", "false", "personal", "icon", "personal", 1, (serverApiKey != null ? serverApiKey.Value : ""), false, "UserConnection", 0);

                }
            }

            return userDeviceIds;
        }

        /// <summary>
        /// This Api is called to accept the invitation sent by user for connection.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AcceptInvitation")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AcceptInvitation(InvitationConfirmationDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var notifications = await _invite.AcceptUserInvitation(model.UserId, userIdClaim, model.ProgramId);
                if (notifications > 0)
                {
                    try
                    {
                        string notificationMessage = MessagesConstants.NewConnectionAddedMessageNotify, notificationTitle = MessagesConstants.NewConnectionAddedTitleNotify;
                        var programNotificationSetCheck = await _program.FindAsync(new { id = model.ProgramId });
                        await AcceptInvitationRefactor(model, userIdClaim, notificationMessage, notificationTitle, programNotificationSetCheck);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := Push notificatio issue(Account := AcceptInvitation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                    try
                    {
                        /* Email Sent */
                        var users = await _userRepository.GetUsersDetailByIds(new List<int> { model.UserId, userIdClaim });
                        var userDetail = users.FirstOrDefault(x => x.Id == model.UserId);
                        var benefactor = users.FirstOrDefault(x => x.Id == userIdClaim);
                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.InvitationAcceptance);
                        template.Subject = template.Subject.Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName));
                        template.Body = template.Body.Replace("Trove", "Sodexo");
                        template.Body = template.Body.Replace("background:linear-gradient(208.63deg, #3952d5 0%, rgba(20,4,185,0.8) 51.1%, #3a55d7 100%)", "");
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName));
                        await _emailService.SendEmail(userDetail?.Email, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Account := AcceptInvitation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.InvitationAcceptedSuccessfully, notifications, 1));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := AcceptInvitation) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
        }

        private async Task AcceptInvitationRefactor(InvitationConfirmationDto model, int userIdClaim, string notificationMessage, string notificationTitle, Domain.DbModel.Program programNotificationSetCheck)
        {
            if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
            {
                var userDeviceIds = await _userRepository.GetUserDeviceTokenByUserIdNdProgram(model.ProgramId, model.UserId);
                if (userDeviceIds.Count > 0)
                {
                    var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Personal);
                    await AcceptInvitationRefactorInside(notificationMessage, notificationTitle, userDeviceIds, usrNotify);
                }
            }
            await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
            {
                notificationMessage = notificationMessage,
                notificationTitle = notificationTitle,
                notificationType = (int)NotificationSettingsEnum.Personal,
                referenceId = 1,
                createdBy = userIdClaim,
                modifiedBy = userIdClaim,
                ProgramId = model.ProgramId,
                userId = model.UserId,
                IsRedirect = true,
                NotificationSubType = "UserConnection",
                CustomReferenceId = 0
            });
        }

        private async Task<List<UserDeviceDto>> AcceptInvitationRefactorInside(string notificationMessage, string notificationTitle, List<UserDeviceDto> userDeviceIds, List<int> usrNotify)
        {
            if (usrNotify.Count > 0)
            {
                userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();
                var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                PushNotifications push = new PushNotifications();
                if (userDeviceIds.Count > 0)
                {
                    await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", "true", "personal", "icon", "personal", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "UserConnection", 0);
                }
            }

            return userDeviceIds;
        }

        /// <summary>
        /// This Api is called to delete user account by benefactor.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteLinkedAccounts")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteBenefactorLinkedAccounts(ConnectionDetailModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var connections = await _benefactor.DeleteLinkedAccounts(model.Type, model.UserId, model.BenefactorId);
                if (connections <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserConnectionNotExists));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ConnectionDeletedSuccessfully, userIdClaim, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := DeleteBenefactorLinkedAccounts) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is called to check for the privacy settings set by user for the benefactor.
        /// </summary>
        /// <returns></returns>
        [Route("PrivacySettings")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetPrivacySettings()
        {
            try
            {
               
                var userIdClaimPrivacySettings = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimPrivacySettings = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimPrivacySettings = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimPrivacySettings, sessionIdClaimPrivacySettings)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get Organisation Details. */
                var privacySettingForPrivacySettings = await _benefactor.GetPrivacySettings(userIdClaimPrivacySettings, programIdClaimPrivacySettings);
                if (privacySettingForPrivacySettings == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, privacySettingForPrivacySettings));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetPrivacySettings) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to update the privacy setting of benefactor to view transaction or not by user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EnablePrivacySettings")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> UpdatePrivacySettings(PrivacySettingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest));
                }
                var identityForUpdatePrivacySettings = User.Identity as ClaimsIdentity;
                var userIdClaimForUpdatePrivacySettings = Convert.ToInt32(identityForUpdatePrivacySettings.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForUpdatePrivacySettings = identityForUpdatePrivacySettings.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForUpdatePrivacySettings = Convert.ToInt32(identityForUpdatePrivacySettings.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForUpdatePrivacySettings, sessionIdClaimForUpdatePrivacySettings)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get Organisation Details. */
                var privacySettingForUpdatePrivacySettings = await _benefactor.UdatePrivacySettings(Convert.ToBoolean(model.isOnlyMe), Convert.ToInt32(model.id), userIdClaimForUpdatePrivacySettings, programIdClaimForUpdatePrivacySettings, Convert.ToBoolean(model.status));
                if (privacySettingForUpdatePrivacySettings == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, privacySettingForUpdatePrivacySettings));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := UpdatePrivacySettings) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is called to Add and update subscription /Reload rule.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateReloadRules")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> AddUpdateReloadRules(ReloadRulesModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest));
                }
                //var identityForUpdatePrivacySettings = User.Identity as ClaimsIdentity;
                //var userIdClaimForUpdatePrivacySettings = Convert.ToInt32(identityForUpdatePrivacySettings.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                //var sessionIdClaimForUpdatePrivacySettings = identityForUpdatePrivacySettings.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                //var programIdClaimForUpdatePrivacySettings = Convert.ToInt32(identityForUpdatePrivacySettings.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                ///* Checks the session of the user against its Id. */
                //if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForUpdatePrivacySettings, sessionIdClaimForUpdatePrivacySettings)))
                //{
                //    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                //}
                /* Get Organisation Details. */
                var result =  await _benefactor.AddUpdateReloadrule(model);

          

                var benefactordetail = await _userRepository.GetUserInfoById(model.BenefactorUserId);
                var Studentdetail= await _userRepository.GetUserInfoById(model.ReloadUserId);
                var template = await _emailService.GetEmailTemplateByName(EmailTemplates.SetSubscriptionRule);
                template.Subject = template.Subject.Replace("{SenderName}", string.Concat(benefactordetail?.FirstName, " ", benefactordetail?.LastName));

                template.Body = template.Body.Replace("{Amount}", (model.AutoReloadAmount).ToString());
              
                template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(Studentdetail?.FirstName, " ", Studentdetail?.LastName)).Replace("{SenderName}", string.Concat(benefactordetail?.FirstName, " ", benefactordetail?.LastName).ToString());
             
             //   Studentdetail.EmailAddress = "randhawa26harman@gmail.com";
                await _emailService.SendEmail(Studentdetail?.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);

                //for benefactor email
                var template1 = await _emailService.GetEmailTemplateByName(EmailTemplates.BenefactorSetSubscriptionRule);
                template1.Subject = template1.Subject.Replace("{SenderName}", string.Concat(benefactordetail?.FirstName, " ", benefactordetail?.LastName));
                template1.Body = template1.Body.Replace("{Amount}",  (model.AutoReloadAmount).ToString());
               
                template1.Body = template1.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(benefactordetail?.FirstName, " ", benefactordetail?.LastName)).Replace("{SenderName}", string.Concat(Studentdetail?.FirstName, " ", Studentdetail?.LastName).ToString());
              //  benefactordetail.EmailAddress = "harman@yopmail.com";
                await _emailService.SendEmail(benefactordetail?.EmailAddress, template1.Subject, template1.Body, template1.CCEmail, template1.BCCEmail);

                //
                await _benefactor.sendReloadSetUpNotification(string.Concat(benefactordetail?.FirstName, " ", benefactordetail?.LastName),Studentdetail.PartnerUserId,model.ReloadUserId, Convert.ToDecimal( model.ReloadAmount),model.CheckDroppedAmount);
              
                var list=   await _benefactor.GetUserReloadRule(model.ReloadUserId);
                if(list.Count ==1)
                {
                    await _benefactor.sendStatuschangeNotification( Studentdetail.PartnerUserId,model.ReloadUserId, "Regular", "VIP");
                }
              

                //var privacySettingForUpdatePrivacySettings = await _benefactor.UdatePrivacySettings(Convert.ToBoolean(model.isOnlyMe), Convert.ToInt32(model.id), userIdClaimForUpdatePrivacySettings, programIdClaimForUpdatePrivacySettings, Convert.ToBoolean(model.status));
                //if (privacySettingForUpdatePrivacySettings == null)
                //{
                //    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
                //}

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := UpdatePrivacySettings) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }


        /// <summary>
        /// This Api is used to cancel the subscription rule.
        /// </summary>
        /// <param name="model"></param>
     
        /// <returns></returns>
        [Route("CancelSubscriptionRule")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelSubscriptionRule(ReloadRequestModel model)
        {
            try
            {
                var Studentdetail = await _userRepository.GetUserInfoById(model.ReloadUserId);
                //  benefactoruserid = 3140;
                var result = await _benefactor.CancelReloadRule(model);

                var list = await _benefactor.GetUserReloadRule(model.ReloadUserId);
                if (list.Count <= 0)
                {
                    await _benefactor.sendStatuschangeNotification( Studentdetail.PartnerUserId, model.ReloadUserId, "VIP", "Regular");
                }
                //if (result <= 0)
                //{
                //    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataSuccessfullyReturned, "0"));
                //}

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.CancelSubscriptionRuleSuccessful, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := CancelSubscriptionRule)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }
    }

}
