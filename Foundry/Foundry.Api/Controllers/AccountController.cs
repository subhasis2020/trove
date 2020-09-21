using AutoMapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using Foundry.Identity;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Microsoft.Extensions.Configuration;
using ElmahCore;
using Foundry.Api.Attributes;
using System.Globalization;
using Foundry.Api.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Foundry.Services.PartnerNotificationsLogs;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// Account class for all the API related to User.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CustomUserManager _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IResetPassword _resetPassword;
        private readonly IEmailService _emailService;
        private readonly IPhotos _entityPhotos;
        private readonly IUsersProgram _usersProgram;
        private readonly IInvitationService _invite;
        private readonly IMapper _mapper;
        private readonly IUsersGroup _ugroup;
        private readonly IBenefactorsProgram _bPrgm;
        private readonly IConfiguration _configuration;
        private readonly ISMSService _sMSService;
        private readonly IOrganisation _organisation;
        private readonly IPrograms _program;
        private readonly IPlanProgramAccountLinkingService _planProgramAccountLinkingService;
        private readonly IUserTransactionInfoes _userTransactionInfoes;
        private readonly IGeneralSettingService _setting;
        private readonly IUserNotificationSettingsService _userNotificationSettingsService;
        private readonly IUserPushedNotificationService _userPushedNotificationService;
        private readonly IUserPushedNotificationsStatusService _userPushedNotificationsStatusService;
        private readonly ISharedJPOSService _sharedPJposService;
        private readonly IUserAgreementHistoryService _userAgreementHistoryService;
        private readonly ApiResponse someIssueInProcessing = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing);
        private readonly IPartnerNotificationsLogServicer _partnerNotificationsLogRepository;
        private readonly IGeneralSettingService _generalRepository;
        private readonly IUserLoyaltyPointsHistoryInfo _userLoyaltyPointsHistoryInfo;
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="userRepository"></param>
        /// <param name="resetPassword"></param>
        /// <param name="emailService"></param>
        /// <param name="photos"></param>
        /// <param name="usersProgram"></param>
        /// <param name="invite"></param>
        /// <param name="mapper"></param>
        /// <param name="ugroup"></param>
        /// <param name="bPrgm"></param>
        /// <param name="configuration"></param>
        /// <param name="sMSService"></param>
        /// <param name="organisation"></param>
        /// <param name="program"></param>
        /// <param name="planProgramAccountLinkingService"></param>
        /// <param name="userTransactionInfoes"></param>
        /// <param name="setting"></param>
        /// <param name="userNotificationSettingsService"></param>
        /// <param name="userPushedNotificationService"></param>
        /// <param name="userPushedNotificationsStatusService"></param>
        /// <param name="sharedPJposService"></param>
        /// <param name="userAgreementHistoryService"></param>
        public AccountController(CustomUserManager userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IUserRepository userRepository, IResetPassword resetPassword, IEmailService emailService, IPhotos photos,
            IUsersProgram usersProgram, IInvitationService invite, IMapper mapper, IUsersGroup ugroup, IBenefactorsProgram bPrgm, IConfiguration configuration,
            ISMSService sMSService, IOrganisation organisation, IPrograms program, IPlanProgramAccountLinkingService planProgramAccountLinkingService,
            IUserTransactionInfoes userTransactionInfoes, IGeneralSettingService setting, IUserNotificationSettingsService userNotificationSettingsService,
            IUserPushedNotificationService userPushedNotificationService, IUserPushedNotificationsStatusService userPushedNotificationsStatusService,
            ISharedJPOSService sharedPJposService, IUserAgreementHistoryService userAgreementHistoryService,
            IPartnerNotificationsLogServicer vpartnerNotificationsLogRepository, IGeneralSettingService vgeneralRepository, IUserLoyaltyPointsHistoryInfo userLoyaltyPointsHistoryInfo
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _resetPassword = resetPassword;
            _emailService = emailService;
            _entityPhotos = photos;
            _usersProgram = usersProgram;
            _invite = invite;
            _mapper = mapper;
            _ugroup = ugroup;
            _bPrgm = bPrgm;
            _configuration = configuration;
            _sMSService = sMSService;
            _organisation = organisation;
            _program = program;
            _planProgramAccountLinkingService = planProgramAccountLinkingService;
            _userTransactionInfoes = userTransactionInfoes;
            _setting = setting;
            _userNotificationSettingsService = userNotificationSettingsService;
            _userPushedNotificationService = userPushedNotificationService;
            _userPushedNotificationsStatusService = userPushedNotificationsStatusService;
            _sharedPJposService = sharedPJposService;
            _userAgreementHistoryService = userAgreementHistoryService;
            _partnerNotificationsLogRepository = vpartnerNotificationsLogRepository;
            _generalRepository = vgeneralRepository;
            _userLoyaltyPointsHistoryInfo = userLoyaltyPointsHistoryInfo;
        }

        /// <summary>
        /// This Api is called to register the user in the system
        /// </summary>
        /// <param name="model">RegisterViewModel</param>-
        /// <returns>User</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(ModelState);
                }
                var user = new User { UserName = model.UserName, FirstName = model.FirstName, LastName = model.LastName, NormalizedUserName = model.FirstName, Email = model.Email, PasswordHash = model.Password };
                // Calling a function to add/update user.
                var result = await _userRepository.AddUpdateUser(user);
                string strRole = Roles.BasicUser;
                Role role = new Role();
                role.Name = strRole;
                if (result > 0)
                {
                    //Checking the role exists.
                    if (await _roleManager.FindByNameAsync(strRole) == null)
                    {
                        await _roleManager.CreateAsync(role);
                    }
                    await _userManager.AddToRoleAsync(user, strRole);
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("firstName", user.FirstName));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("lastName", user.LastName));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", strRole));
                    return Ok(new ProfileViewModel(user));
                }
                return Ok("Error");
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := Register)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok("Error");
            }
        }

        /// <summary>
        /// This Api is used to return the claims of authorized user.
        /// </summary>
        /// <returns>Claims</returns>
        [Route("claims")]
        [HttpGet]
        public ActionResult<List<ClaimDto>> Claims()
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                List<ClaimDto> claims = new List<ClaimDto>();
                foreach (Claim c in identity.Claims)
                {
                    claims.Add(new ClaimDto { Subject = c.Subject.Name, ClaimType = c.Type, ClaimValue = c.Value });
                }
                return claims;
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := Claims)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return null;
            }

        }

        /// <summary>
        /// Checks for the username and password and returns user detail.
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <param name="isWeb">Bool Value to check for the source of the request.</param>
        /// <returns>User detail</returns>
        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, bool isWeb = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest));
                }
                // Checking the user exists.
                var user = await _userManager.FindByNameAsync(model.EmailAddress);
                if (user == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotExist.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (!user.IsActive.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserAccountDeactivated.Replace("{supportemail}", _configuration["SupportEmail"])));
                }
                // Setting the password for user.
                var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, true, true);
                if (result != null)
                {
                    if (result.Succeeded)
                    {
                        try
                        {
                            user.UserDeviceId = model.DeviceId ?? null;
                            user.UserDeviceType = model.DeviceType ?? null;
                            user.Location = model.Location ?? null;
                            user.SessionId = Guid.NewGuid().ToString();
                            // Update the user.
                            await _userManager.UpdateAsync(user);
                            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserLoggedIn, await _userRepository.GetUserById(user.Id)));
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                    if (!result.Succeeded)
                    {
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.LogInPasswordIncorrect.Replace("{supportemail}", _configuration["SupportEmail"])));
                    }
                    if (result.IsLockedOut)
                    {
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.LogInUserLockedOut.Replace("{supportemail}", _configuration["SupportEmail"])));
                    }
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotExist.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := Login)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is called to generate the token and return it in case of password recovery.
        /// </summary>
        /// <remarks>
        /// example:
        /// {
        /// "emailAddress": "abc@mailinator.com"
        /// }
        /// </remarks>
        /// <param name="model">ForgotPasswordModel</param>
        /// <returns>Token</returns>
        [Route("ForgotPassword")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                // Checking the user for resetting password.
                var usrCheck = await _resetPassword.CheckUser(model.EmailAddress);
                if (usrCheck == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (!usrCheck.EmailConfirmed)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.EmailNotConfirmed.Replace("{supportemail}", _configuration["SupportEmail"])));
                }
                // Generating a token for resetting password.
                var generateForgotPasswordToken = await _resetPassword.GenerateForgotPasswordToken(model.EmailAddress);
                string getSMSEmailSettings = await GetSMSEmailSettings(model, generateForgotPasswordToken);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, string.Concat(MessagesConstants.VerificationCodeSent, getSMSEmailSettings == "1" ? "registered email address." : "registered phone number."), _mapper.Map<ResetPasswordDto>(generateForgotPasswordToken)));  // 200
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := ForgotPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task<string> GetSMSEmailSettings(ForgotPasswordModel model, ResetUserPassword token)
        {
            string smsemailSettingDefaultValue = "1";
            if (token != null)
            {
                try
                {
                    // Getting user's detail by Email Id.
                    var usersForSMSEmailSettings = await _userRepository.GetUsersDetailByEmailIds(new List<string>() { model.EmailAddress });
                    var userDetailForSMSEmailSettings = usersForSMSEmailSettings.FirstOrDefault();
                    if (userDetailForSMSEmailSettings != null)
                    {
                        var emailSMSSettingsForSMSEmailSettings = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMSEmail)).FirstOrDefault();
                        if (emailSMSSettingsForSMSEmailSettings != null)
                        {
                            smsemailSettingDefaultValue = emailSMSSettingsForSMSEmailSettings.Value;
                            if (emailSMSSettingsForSMSEmailSettings.Value == "1")
                            {
                                /*Email*/
                                // Get email template by name.
                                var template = await _emailService.GetEmailTemplateByName(EmailTemplates.ResetPassword);
                                template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetailForSMSEmailSettings.FirstName, " ", userDetailForSMSEmailSettings.LastName)).Replace("{token}", token.resetToken);
                                // Calling a method to send an email.
                                await _emailService.SendEmail(model.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                            }
                            else
                            {
                                //Get sms template by name.
                                var smstemplate = await _sMSService.GetSMSTemplateByName(SMSTemplates.ResetPassword);
                                smstemplate.body = smstemplate.body.Replace("{Name}", string.Concat(userDetailForSMSEmailSettings?.FirstName, " ", userDetailForSMSEmailSettings?
                                    .LastName)).Replace("{token}", token.resetToken);
                                // Calling a method to send an SMS.
                                await _sMSService.SendSMS(_configuration["accountSID"], _configuration["authToken"], userDetailForSMSEmailSettings?.PhoneNumber, smstemplate.body);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := ForgotPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
            }

            return smsemailSettingDefaultValue;
        }

        /// <summary>
        /// This Api is called to verify the token which is sent in the email for password recovery.
        /// </summary>
        /// <param name="model">VerifyPasswordModel</param>
        /// <returns>User detail</returns>
        [Route("VerifyResetPasswordToken")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VerifyTokenForPasswordReset(VerifyPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                // Checking the reset token for reset password.
                var resetPwdForVfyToken = await _resetPassword.VerifyPasswordReset(model.EmailAddress, model.Token);
                if (resetPwdForVfyToken == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidVerificationCode.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (resetPwdForVfyToken.validTill < DateTime.UtcNow)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.VerificationCodeExpired));
                }
                if (resetPwdForVfyToken.isPasswordReset.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OneTimeUserVerificationCode));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.VerificationCodeMatched, _mapper.Map<ResetPasswordDto>(resetPwdForVfyToken)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := VerifyTokenForPasswordReset)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to reset the password.
        /// </summary>
        /// <param name="model">ResetPasswordModel</param>
        /// <returns>bool - to check if the password is set or not.</returns>
        [Route("ResetPassword")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                // Reset user for password.
                var isResetUserPassword = await _resetPassword.ResetUserPassword(model.EmailAddress, model.Password);
                if (!isResetUserPassword)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PasswordChangedSuccess, isResetUserPassword));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := ResetPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is called for registering (updating) user using user detail along with program id
        /// and the send the email for the same.
        /// </summary>
        /// <param name="model">SignUpViewModel</param>
        /// <returns>User updated details</returns>
        [Route("RegisterWithProgram")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterUserWithProgram(SignUpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest));
                }
                // Check phone number existence for user.
                var userPhoneCheckNumberExistence = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (userPhoneCheckNumberExistence != null && !userPhoneCheckNumberExistence.Email.Trim().Equals(model.EmailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PhoneNumberExists, false));
                }
                var userRegister = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.EmailAddress,
                    PasswordHash = model.Password,
                    UserDeviceId = model.DeviceId,
                    UserDeviceType = model.DeviceType,
                    Location = model.location,
                    EmailConfirmed = true,
                    IsMobileRegistered = true

                };
                // Register(Update) user with program.
                if (!string.IsNullOrEmpty(model.PhotoPath) && model.PhotoPath.Contains("foundry-pre-prod.s3.ca-central-1.amazonaws.com"))
                {
                    var photoPath = model.PhotoPath;
                    var photoSplit = photoPath.Split("com/");
                    var photoSeparatorValue = photoSplit[1];
                    var photoSeparatorQuery = photoSeparatorValue.Split('?');
                    if (photoSeparatorQuery.Length > 0)
                        model.PhotoPath = photoSeparatorQuery[0].Replace("%20", " ");
                }
                var usrforRegisterUser = await _userRepository.RegisterUserWithProgram(userRegister, model.ProgramCodeId, model.PhotoPath);
                if (usrforRegisterUser == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                try
                {
                    // Getting the email template by its name.
                    var templateForRegisterUser = await _emailService.GetEmailTemplateByName(EmailTemplates.RegistrationFoundry);
                    templateForRegisterUser.Body = templateForRegisterUser.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(model.FirstName, " ", model.LastName)).Replace("{email}", model.EmailAddress).Replace("{password}", model.Password);
                    // Calling a method to send an email.
                    await _emailService.SendEmail(usrforRegisterUser.EmailAddress, templateForRegisterUser.Subject, templateForRegisterUser.Body, templateForRegisterUser.CCEmail, templateForRegisterUser.BCCEmail);
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := RegisterUserWithProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, usrforRegisterUser));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := RegisterUserWithProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to get the details of the user.
        /// </summary>
        /// <param name="model">UserProgramModel</param>
        /// {
        /// UserId
        /// ProgramId
        /// }
        /// <returns>User detail.</returns>
        [Route("User")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetUser(UserProgramModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identityForGetUser = User.Identity as ClaimsIdentity;
                var userIdClaimForGetUser = Convert.ToInt32(identityForGetUser.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForGetUser = identityForGetUser.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForGetUser = Convert.ToInt32(identityForGetUser.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForGetUser, sessionIdClaimForGetUser)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                // Get user with program code.
                var user = await _userRepository.GetUserWithProgramCode(userIdClaimForGetUser, programIdClaimForGetUser);
                if (user == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetSuccessfulUserDetail, user));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to get the user detail based on other token.
        /// </summary>
        /// <param name="model">UserProgramModel</param>
        /// {
        /// UserId
        /// ProgramId
        /// }
        /// <returns>User detail</returns>
        [Route("UserWithPublicToken")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetUserWithPublicToken(UserProgramModel model)
        {
            try
            {
                // Get user with program code irrespective of claims.
                var userWithPublicToken = await _userRepository.GetUserWithProgramCodeBeforeRegister(model.UserId, model.ProgramId);
                if (userWithPublicToken == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetSuccessfulUserDetail, userWithPublicToken));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetUserWithPublicToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to register (update) user.
        /// </summary>
        /// <param name="model">SignUpViewModel</param>
        /// <returns>User updated detail.</returns>
        [Route("RegisterWithId")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterUserWithId(SignUpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                // Checking phone number existence for user.
                var userPhoneCheckForRegisterUser = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (userPhoneCheckForRegisterUser != null && !userPhoneCheckForRegisterUser.Email.Trim().Equals(model.EmailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PhoneNumberExists, false)); // 404
                }
                var userRegister = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.EmailAddress,
                    PasswordHash = model.Password,
                    UserDeviceId = model.DeviceId,
                    UserDeviceType = model.DeviceType,
                    Location = model.location,
                    EmailConfirmed = true,
                    IsMobileRegistered = true
                };
                // Register(Update) user with program code and id.
                if (!string.IsNullOrEmpty(model.PhotoPath) && model.PhotoPath.Contains("foundry-pre-prod.s3.ca-central-1.amazonaws.com"))
                {
                    var photoPath = model.PhotoPath;
                    var photoSplit = photoPath.Split("com/");
                    var photoSeparatorValue = photoSplit[1];
                    var photoSeparatorQuery = photoSeparatorValue.Split('?');
                    if (photoSeparatorQuery.Length > 0)
                        model.PhotoPath = photoSeparatorQuery[0].Replace("%20", " ");
                }
                var usr = await _userRepository.RegisterUserWithProgram(userRegister, model.ProgramCodeId, model.PhotoPath);
                if (usr == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                try
                {
                    // Get the email template by its name.
                    var template = await _emailService.GetEmailTemplateByName(EmailTemplates.RegistrationFoundry);
                    template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(model.FirstName, " ", model.LastName)).Replace("{email}", model.EmailAddress).Replace("{password}", model.Password);
                    // Calling a method to send an email.
                    await _emailService.SendEmail(usr.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := RegisterUserWithId)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, usr));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := RegisterUserWithId)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to link account user with program and it checks for verification.
        /// </summary>
        /// <param name="model">LinkAccountModel</param>
        /// <returns></returns>
        [Route("LinkAccount")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkAccountValidation(LinkAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                // This method is used to check existence of email.
                var usrLinkAccount = _mapper.Map<UserDto>(await _userRepository.CheckUserByEmail(model.EmailAddress));
                if (usrLinkAccount == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                // This method is used to get user program linking by email and its id.
                var checkUserProgramLinkAccount = await _usersProgram.GetUserProgramLinkingByEmailNID(model.ProgramId, model.UserId.Trim(), model.EmailAddress.Trim());
                if (checkUserProgramLinkAccount == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoProgramAssociationWithId));
                }
                // This method is used to update user program.
                var token = _mapper.Map<UserProgramDto>(await _usersProgram.UpdateUserProgramNUserReturn(model.UserId, model.ProgramId, model.EmailAddress));
                string smsemailSettingForAccountLinkValidation = "1";
                smsemailSettingForAccountLinkValidation = await SendSMSOrEmailOnLinkAccount(model, token, smsemailSettingForAccountLinkValidation);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, string.Concat(MessagesConstants.ValidationCodeSent, smsemailSettingForAccountLinkValidation == "1" ? string.Format("email address {0}.", model.EmailAddress) : "registered phone number."), token));  // 200
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := LinkAccountValidation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task<string> SendSMSOrEmailOnLinkAccount(LinkAccountModel model, UserProgramDto token, string smsemailSettingLnkAccount)
        {
            if (token != null)
            {
                try
                {
                    // Get the user's detail by its id.
                    var usersForSMSLinkAccount = await _userRepository.GetUsersDetailByUserCode(new List<string>() { model.UserId });
                    var userDetailForSMSLinkAccount = usersForSMSLinkAccount.FirstOrDefault();
                    if (userDetailForSMSLinkAccount != null)
                    {
                        var smsemailSettingForLinkAccount = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMSEmail)).FirstOrDefault();
                        if (smsemailSettingForLinkAccount != null)
                        {
                            smsemailSettingLnkAccount = smsemailSettingForLinkAccount.Value;
                            if (smsemailSettingForLinkAccount.Value == "1")
                            {
                                /*Email*/
                                // Get the email template by name.
                                var templateForSMSLinkAccount = await _emailService.GetEmailTemplateByName(EmailTemplates.LinkAccountVerification);
                                templateForSMSLinkAccount.Body = templateForSMSLinkAccount.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetailForSMSLinkAccount.FirstName, " ", userDetailForSMSLinkAccount.LastName)).Replace("{token}", token.linkAccountVerificationCode);
                                // This method is called to send an email.
                                await _emailService.SendEmail(model.EmailAddress, templateForSMSLinkAccount.Subject, templateForSMSLinkAccount.Body, templateForSMSLinkAccount.CCEmail, templateForSMSLinkAccount.BCCEmail);
                            }
                            else
                            {
                                // Get the sms template by name.
                                var smstemplateForSMSLinkAccount = await _sMSService.GetSMSTemplateByName(SMSTemplates.LinkAccountVerification);
                                smstemplateForSMSLinkAccount.body = smstemplateForSMSLinkAccount.body.Replace("{Name}", string.Concat(userDetailForSMSLinkAccount?.FirstName, " ", userDetailForSMSLinkAccount?.LastName)).Replace("{token}", token.linkAccountVerificationCode);
                                // This method is called to send sms.
                                await _sMSService.SendSMS(_configuration["accountSID"], _configuration["authToken"], userDetailForSMSLinkAccount?.PhoneNumber, smstemplateForSMSLinkAccount.body);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := LinkAccountValidation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
            }

            return smsemailSettingLnkAccount;
        }

        /// <summary>
        /// This Api is called to check for the validation code sent by mobile app for program linking.
        /// </summary>
        /// <param name="model">LinkAccountValidateModel</param>
        /// <returns></returns>
        [Route("ValidateLinkVerificationCode")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkAccountValidationCode(LinkAccountValidateModel model)
        {
            try
            {
                // This method is called to validate program linking with verification code.
                var validateLinkAccount = await _usersProgram.ValidateLinkAccountCode(model.UserId, model.ProgramId, model.VerificationCode);
                if (validateLinkAccount == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidVerificationCode.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (validateLinkAccount.verificationCodeValidTill < DateTime.UtcNow)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.VerificationCodeExpired));
                }
                if (validateLinkAccount.isVerificationCodeDone.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OneTimeUserVerificationCode));
                }
                // This method is called to update validation code.
                var userLinkAccount = _mapper.Map<UserDto>(await _usersProgram.UpdateValidationCodeNUserDetailReturn(model.UserId, model.ProgramId, false));
                if (userLinkAccount == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.LinkingProgramWithUserAlreadyExists));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetLinkingProgramSuccessMessage, userLinkAccount));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := LinkAccountValidationCode)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to check for the inactivity of the user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uid"></param>
        /// <param name="pid"></param>
        /// <param name="ptype"></param>
        /// <returns></returns>
        [Route("InactivityCheck")]
        [HttpGet]
        //  [Authorize]
        public async Task<IActionResult> CheckUserInactivity(string id, string uid, string pid, string ptype)
        {
            try
            {
                // This method is called to check user is active or not.
                var userInactiveCheck = await _userRepository.CheckUserInactivity(Cryptography.DecryptCipherToPlain(id));
                if (!string.IsNullOrEmpty(ptype))
                {
                    // This method is used to find a user by its id.
                    var userInvited = await _userRepository.FindAsync(new { email = Cryptography.DecryptCipherToPlain(id) });
                    if (userInactiveCheck == false)
                    {
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotExist.Replace("{supportemail}", _configuration["SupportEmail"]), _mapper.Map<UserDto>(userInvited), 0));
                    }
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetSuccessfulUserDetail, _mapper.Map<UserDto>(userInvited), 1, userInactiveCheck.ToString()));
                }
                else
                {
                    // This method is user to get existing invitation with email.
                    var userInvited = await _invite.GetExistingInvitationWithEmailUserProgram(Cryptography.DecryptCipherToPlain(id), Convert.ToInt32(Cryptography.DecryptCipherToPlain(uid)), Convert.ToInt32(Cryptography.DecryptCipherToPlain(pid)));
                    if (userInactiveCheck == false)
                    {
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotExist.Replace("{supportemail}", _configuration["SupportEmail"]), _mapper.Map<InvitationDto>(userInvited), 0));
                    }
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetSuccessfulUserDetail, _mapper.Map<InvitationDto>(userInvited), 1, userInactiveCheck.ToString()));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := CheckUserInactivity)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is called from Web to send the link in case you forget the password.
        /// </summary>
        /// <param name="model">ForgotPasswordModel</param>
        /// <returns></returns>
        [Route("ForgotPasswordResetLink")]
        [HttpPost]
        // [Authorize]
        public async Task<IActionResult> SentLinkForForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var usrCheck = await _resetPassword.CheckUser(model.EmailAddress);
                if (usrCheck == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]), 0)); // 404
                }
                if (!usrCheck.EmailConfirmed)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.EmailNotConfirmed.Replace("{supportemail}", _configuration["SupportEmail"]), 0));
                }
                var token = await _resetPassword.GenerateForgotPasswordToken(model.EmailAddress, true);
                if (token != null)
                {
                    try
                    {
                        var users = await _userRepository.GetUsersDetailByEmailIds(new List<string>() { model.EmailAddress });
                        var userDetail = users.FirstOrDefault();
                        var URLRedirectLink = string.Concat(_configuration["WebURL"], "Account/ResetPasswordCheck?id=", Cryptography.EncryptPlainToCipher(model.EmailAddress));
                        var template = await _emailService.GetEmailTemplateByName(Constants.EmailTemplates.ForgotPassword);
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", userDetail != null ? string.Concat(userDetail.FirstName, " ", userDetail.LastName) : "").Replace("{link}", URLRedirectLink);
                        await _emailService.SendEmail(model.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Account := SentLinkForForgotPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ResentPasswordLinkSent, _mapper.Map<ResetPasswordDto>(token), 1));  // 200
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := SentLinkForForgotPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called from Web to validate the user on restting password.
        /// </summary>
        /// <param name="model">ResetPasswordCheckDto</param>
        /// <returns></returns>
        [Route("CheckUseResetPasswordLink")]
        [HttpPost]
        //  [Authorize]
        public async Task<IActionResult> CheckResetPasswordLink(ResetPasswordCheckDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var resetPwd = await _resetPassword.VerifyPasswordResetWeb(model.EmailAddress);
                if (resetPwd == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (resetPwd.validTill < DateTime.UtcNow)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ResetPasswordLinkExpired));
                }
                if (resetPwd.isPasswordReset.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OneTimeResetLinkUse));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.LinkIsActiveForUse, 1, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := CheckResetPasswordLink)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to check the user if the user is new then it creates a user else it resets the password of the user.
        /// </summary>
        /// <param name="model">ResetPasswordModel</param>
        /// <returns></returns>
        [Route("CheckUserForResetPassword")]
        [HttpPost]
        //  [Authorize]
        public async Task<IActionResult> CheckUserNResetPassword(ResetPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var usrCheck = await _resetPassword.CheckUser(model.EmailAddress);
                return await CheckUserNResetPasswordRefactor(model, usrCheck);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := CheckUserNResetPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task<IActionResult> CheckUserNResetPasswordRefactor(ResetPasswordModel model, User usrCheck)
        {
            if (usrCheck == null)
            {
                var inviteCheck = await _invite.GetExistingInvitationWithEmail(model.EmailAddress, model.UserId);
                if (inviteCheck == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]), 0));
                }
                var user = new User()
                {
                    FirstName = inviteCheck.FirstName,
                    LastName = inviteCheck.LastName,
                    AccessFailedCount = 0,
                    CreatedBy = inviteCheck.CreatedBy,
                    Email = inviteCheck.Email,
                    EmailConfirmed = true,
                    NormalizedEmail = inviteCheck.Email,
                    NormalizedUserName = inviteCheck.Email,
                    PhoneNumber = inviteCheck.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    UserName = model.EmailAddress,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user);
                await _resetPassword.ResetUserPassword(model.EmailAddress.Trim(), model.Password.Trim());
                string strRole = Roles.Benefactor;
                Role role = new Role();
                role.Name = strRole;
                int? userId;
                if (result.Succeeded)
                {
                    if (await _roleManager.FindByNameAsync(strRole) == null)
                    {
                        await _roleManager.CreateAsync(role);
                    }
                    await _userManager.AddUserRole(user.Email, strRole);
                    var userInfo = await _userRepository.GetSingleDataByConditionAsync(new { UserName = user.UserName });
                    if (userInfo != null)
                    {
                        user.Id = userInfo.Id;
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("firstName", user.FirstName));
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("lastName", user.LastName));
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                        if (!string.IsNullOrEmpty(inviteCheck.ImagePath))
                        {
                            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("image", inviteCheck.ImagePath));
                        }
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", strRole));
                    }
                    userId = userInfo != null ? userInfo.Id : 0;

                    var student = await _userRepository.GetUserById(model.UserId);
                    if (!string.IsNullOrWhiteSpace(student.PartnerUserId))
                    {
                        await sendInviteAcceptedNotification(student, user.FirstName + " " + (string.IsNullOrWhiteSpace(user.LastName) ? "" : user.LastName), SodexoBiteNotification.SodexoBiteBaseUrl);


                        List<GeneralSetting> i2cSettings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                        if (i2cSettings[0].Value == "1")
                        {
                            await sendInviteAcceptedNotification(student, user.FirstName + " " + (string.IsNullOrWhiteSpace(user.LastName) ? "" : user.LastName), SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                        }
                    }

                }
                else
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, result.Errors.ToString(), 0));
                }
                if (!string.IsNullOrEmpty(inviteCheck.ImagePath))
                {
                    var imageSave = await _entityPhotos.SaveUpdateImage(inviteCheck.ImagePath, userId.Value, 0, (int)PhotoEntityType.Benefactor);
                    if (!imageSave)
                    {
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ImageSavingIssue, 0));
                    }
                }
                if (userId.Value != 0)
                {
                    await _bPrgm.AddBenefactorInProgram(userId.Value, inviteCheck.programId.Value);
                    var groupDetail = await _ugroup.GetGroupIdByName(Groups.Benefactors);
                    if (groupDetail != null) { await _ugroup.AddUpdateUserGroup(userId.Value, groupDetail.id); }
                }
                //Automatically accept user invitation on create password.
                if (userId.Value != 0 && inviteCheck.relationshipId.Value != 0)
                {
                    await _invite.AcceptUserInvitation(inviteCheck.CreatedBy, userId.Value, inviteCheck.programId.Value);
                    try
                    {
                        string notificationMessage = string.Empty, notificationTitle = string.Empty;
                        var programNotificationSetCheck = await _program.FindAsync(new { id = inviteCheck.programId.Value });
                        if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
                        {
                            var userDeviceIds = await _userRepository.GetUserDeviceTokenByUserIdNdProgram(inviteCheck.programId.Value, inviteCheck.CreatedBy);
                            if (userDeviceIds.Any())
                            {
                                var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Personal);
                                if (usrNotify.Any())
                                {
                                    userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();

                                    var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                                    PushNotifications push = new PushNotifications();

                                    if (userDeviceIds.Any())
                                    {
                                        notificationMessage = MessagesConstants.NewConnectionAddedMessageNotify;
                                        notificationTitle = MessagesConstants.NewConnectionAddedTitleNotify;
                                        await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", "true", "personal", "icon", "personal", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "UserConnection", 0);
                                    }

                                }
                            }
                        }
                        await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
                        {
                            notificationMessage = notificationMessage,
                            notificationTitle = notificationTitle,
                            notificationType = (int)NotificationSettingsEnum.Personal,
                            referenceId = 1,
                            createdBy = inviteCheck.CreatedBy,
                            modifiedBy = inviteCheck.CreatedBy,
                            ProgramId = inviteCheck.programId.Value,
                            userId = inviteCheck.CreatedBy,
                            IsRedirect = true,
                            NotificationSubType = "UserConnection",
                            CustomReferenceId = 0
                        });

                        
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := Push notification issues(Account := CheckUserNResetPassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }

                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PasswordCreatedSuccess, true, 1));
            }
            else
            {
                var isReset = await _resetPassword.ResetUserPassword(model.EmailAddress, model.Password);
                if (!isReset)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]), 0)); // 404
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PasswordChangedSuccess, isReset, 1));
            }
        }

        private async Task sendInviteAcceptedNotification(UserDto user, string name, string key)
        {
            PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
            try
            {
                using (var client = new HttpClient())
                {
                    object obj = new
                    {
                        UserObjectId = user.PartnerUserId,
                        BenefactorName = name
                    };
                    List<GeneralSetting> i2cSettings = await GetGeneralSettings(key);

                    var url = i2cSettings[0].Value;
                    var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.BitePayBenefactorInviteAccepted);

                    string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("BenefactorInviteAccepted", hostURL.ToString(), myJSON, user.Id);
                    HttpContent stringContent = new StringContent(myJSON);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                    // var content = new StringContent(myJSON.ToString(), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(hostURL, stringContent);

                    PartnerNotificationsLogReq.Status = result.StatusCode.ToString();
                    var content = await result.Content.ReadAsStringAsync();
                    await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, content);

                }
            }
            catch (Exception ex)
            {

                PartnerNotificationsLogReq.Status = "Error";
                await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, ex.Message);


            }
        }

        private async Task<List<GeneralSetting>> GetGeneralSettings(string key)
        {
            var settings = await _generalRepository.GetDataAsync(new { KeyName = key });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }
    



    /// <summary>
    /// This Api is called to generate the token and return it in case of password recovery.
    /// </summary>
    /// <remarks>
    /// example:
    /// 
    /// {
    /// "emailAddress": "abc@mailinator.com"
    ///}
    /// </remarks>
    /// <param name="model">ForgotPasswordModel</param>
    /// <returns>Token</returns>
    [Route("ChangePasswordEmailLoggedUser")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> ChangePasswordEmailInput(ForgotPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var identityForChangePassword = User.Identity as ClaimsIdentity;
                var userIdClaimForChangePassword = Convert.ToInt32(identityForChangePassword.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForChangePassword = identityForChangePassword.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForChangePassword, sessionIdClaimForChangePassword)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var usrCheckForChangePassword = await _resetPassword.CheckUser(model.EmailAddress);
                if (usrCheckForChangePassword == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (!usrCheckForChangePassword.EmailConfirmed)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.EmailNotConfirmed.Replace("{supportemail}", _configuration["SupportEmail"])));
                }
                var tokenForChangePassword = await _resetPassword.GenerateForgotPasswordToken(model.EmailAddress);
                string smsemailSettingForChangePassword = await ChangePasswordEmailInputRefactor(model, tokenForChangePassword);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, string.Concat(MessagesConstants.VerificationCodeSent, smsemailSettingForChangePassword == "1" ? "registered email address." : "registered phone number."), _mapper.Map<ResetPasswordDto>(tokenForChangePassword)));  // 200
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := ChangePasswordEmailInput)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task<string> ChangePasswordEmailInputRefactor(ForgotPasswordModel model, ResetUserPassword token)
        {
            string smsemailSettingForChgEmailInputRefactor = "1";
            if (token != null)
            {
                try
                {
                    var usersForEmailInputRefactor = await _userRepository.GetUsersDetailByEmailIds(new List<string>() { model.EmailAddress });
                    var userDetailForEmailInputRefactor = usersForEmailInputRefactor.FirstOrDefault();
                    if (userDetailForEmailInputRefactor != null)
                    {
                        var emailSMSSettingsForEmailInputRefactor = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMSEmail)).FirstOrDefault();
                        if (emailSMSSettingsForEmailInputRefactor != null)
                        {
                            smsemailSettingForChgEmailInputRefactor = emailSMSSettingsForEmailInputRefactor.Value;
                            if (emailSMSSettingsForEmailInputRefactor.Value == "1")
                            {
                                /*Email*/
                                var templateForEmailInputRefactor = await _emailService.GetEmailTemplateByName(EmailTemplates.ResetPassword);
                                templateForEmailInputRefactor.Body = templateForEmailInputRefactor.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetailForEmailInputRefactor.FirstName, " ", userDetailForEmailInputRefactor.LastName)).Replace("{token}", token.resetToken);
                                await _emailService.SendEmail(model.EmailAddress, templateForEmailInputRefactor.Subject, templateForEmailInputRefactor.Body, templateForEmailInputRefactor.CCEmail, templateForEmailInputRefactor.BCCEmail);
                            }
                            else
                            {
                                var smstemplateForEmailInputRefactor = await _sMSService.GetSMSTemplateByName(SMSTemplates.ResetPassword);
                                smstemplateForEmailInputRefactor.body = smstemplateForEmailInputRefactor.body.Replace("{Name}", string.Concat(userDetailForEmailInputRefactor?.FirstName, " ", userDetailForEmailInputRefactor?.LastName)).Replace("{token}", token.resetToken);
                                await _sMSService.SendSMS(_configuration["accountSID"], _configuration["authToken"], userDetailForEmailInputRefactor?.PhoneNumber, smstemplateForEmailInputRefactor.body);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := ChangePasswordEmailInput)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
            }

            return smsemailSettingForChgEmailInputRefactor;
        }

        /// <summary>
        /// This Api is called to verify the token for changing password of the user.
        /// </summary>
        /// <param name="model">VerifyPasswordModel</param>
        /// <returns></returns>
        [Route("VerifyTokenForChangePasswordLoggedUser")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> VerifyTokenForChangePassword(VerifyPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimForVerifyToken = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForVerifyToken = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForVerifyToken, sessionIdClaimForVerifyToken)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var resetPwdForVerifyToken = await _resetPassword.VerifyPasswordReset(model.EmailAddress, model.Token);
                if (resetPwdForVerifyToken == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidVerificationCode.Replace("{supportemail}", _configuration["SupportEmail"])));
                }
                if (resetPwdForVerifyToken.validTill < DateTime.UtcNow)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.VerificationCodeExpired));
                }
                if (resetPwdForVerifyToken.isPasswordReset.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OneTimeUserVerificationCode));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.VerificationCodeMatched, _mapper.Map<ResetPasswordDto>(resetPwdForVerifyToken)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := VerifyTokenForChangePassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to change the password of the user after doing some form of verification
        /// </summary>
        /// <param name="model">ResetPasswordModel</param>
        /// <returns></returns>
        [Route("ChangePasswordSet")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> SubmitChangePassword(ResetPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimForSubmitChangePassword = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForSubmitChangePassword = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForSubmitChangePassword = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForSubmitChangePassword, sessionIdClaimForSubmitChangePassword)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var isResetForSubPwd = await _resetPassword.ResetUserPassword(model.EmailAddress, model.Password);
                if (!isResetForSubPwd)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ForgotPasswordEmailAddressIncorrect.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                string notificationMessage = MessagesConstants.PasswordChangedNotificationMessage, notificationTitle = MessagesConstants.PasswordChangedNotificationTitle; var programNotificationSetCheck = await _program.FindAsync(new { id = programIdClaimForSubmitChangePassword });
                if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
                {
                    var userDeviceIds = await _userRepository.GetDataByIdAsync(new { Id = programIdClaimForSubmitChangePassword });
                    if (userDeviceIds != null)
                    {
                        var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                        PushNotifications push = new PushNotifications();
                        var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(new List<int> { userDeviceIds.Id }, (int)NotificationSettingsEnum.Personal);
                        if (usrNotify.Count > 0)
                        {
                            await push.SendPushBulk(new List<string> { userDeviceIds.UserDeviceId }, notificationTitle, notificationMessage, "", programIdClaimForSubmitChangePassword.ToString(), "personal", "icon", "personal", 1, (serverApiKey != null ? serverApiKey.Value : ""), false, "PasswordChange", 0);
                        }
                    }
                }
                await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
                {
                    notificationMessage = notificationMessage,
                    notificationTitle = notificationTitle,
                    notificationType = (int)NotificationSettingsEnum.Personal,
                    referenceId = 1,
                    createdBy = programIdClaimForSubmitChangePassword,
                    modifiedBy = programIdClaimForSubmitChangePassword,
                    ProgramId = programIdClaimForSubmitChangePassword,
                    userId = programIdClaimForSubmitChangePassword,
                    IsRedirect = false,
                    NotificationSubType = "PasswordChange",
                    CustomReferenceId = 0
                });
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PasswordChangedSuccess, isResetForSubPwd));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := SubmitChangePassword)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to check the phonenumber duplicacy in user table.
        /// </summary>
        /// <param name="phoneNumber">PhoneNumber</param>
        /// <param name="emailAddress">EmailAddress</param>
        /// <returns></returns>
        [Route("CheckPhoneNumberExistence")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CheckPhoneNumberExistence(string phoneNumber, string emailAddress = "")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userPhoneCheck = await _userRepository.CheckPhoneNumberExistence(phoneNumber);
                if (userPhoneCheck != null)
                {
                    if (!string.IsNullOrEmpty(emailAddress))
                    {
                        if (!userPhoneCheck.Email.Trim().Equals(emailAddress, StringComparison.OrdinalIgnoreCase))
                        {
                            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PhoneNumberExists, false));
                        }
                    }
                    else if (userPhoneCheck.Email.Trim().Equals(emailAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        userPhoneCheck.IsMobileRegistered = true;
                        await _userRepository.UpdateAsync(userPhoneCheck, new { id = userPhoneCheck.Id });
                    }
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PhoneNumberNotExists, true, 1, userPhoneCheck != null ? userPhoneCheck.Id.ToString() : "0"));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := CheckPhoneNumberExistence)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called by mobile app to return user profile.
        /// </summary>
        /// <returns>User details</returns>
        [Route("ProfileContent")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetProfileContent()
        {
            try
            {
                
                var userIdClaimForProfileContent = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForProfileContent = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForProfileContent, sessionIdClaimForProfileContent)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var usrForProfileContent = await _userRepository.GetUserById(userIdClaimForProfileContent);
                if (usrForProfileContent == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetSuccessfulUserDetail, usrForProfileContent));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetProfileContent)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to edit the profile of the user from mobile app.
        /// </summary>
        /// <param name="model">EditProfileModel</param>
        /// <returns>User updated details</returns>
        [Route("EditProfile")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> EditProfile(EditProfileModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimAccountProfile = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimAccountProfile = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimAccountProfile = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimAccountProfile, sessionIdClaimAccountProfile)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var userPhoneCheckForEditProfile = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (userPhoneCheckForEditProfile != null && userPhoneCheckForEditProfile.Id != userIdClaimAccountProfile)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PhoneNumberExists, false));
                }
                var userRegister = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.EmailAddress,
                    Id = programIdClaimAccountProfile
                };
                if (!string.IsNullOrEmpty(model.PhotoPath) && model.PhotoPath.Contains("foundry-pre-prod.s3.ca-central-1.amazonaws.com"))
                {
                    var photoPath = model.PhotoPath;
                    var photoSplit = photoPath.Split("com/");
                    var photoSeparatorValue = photoSplit[1];
                    var photoSeparatorQuery = photoSeparatorValue.Split('?');
                    if (photoSeparatorQuery.Length > 0)
                        model.PhotoPath = photoSeparatorQuery[0].Replace("%20", " ");
                }

                var usr = await _userRepository.EditUserProfile(userRegister, programIdClaimAccountProfile, model.PhotoPath);
                if (usr == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                await EditProfileRefactor(userIdClaimAccountProfile, programIdClaimAccountProfile);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ProfileUpdatedSuccessfully, usr));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := EditProfile)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task EditProfileRefactor(int userIdClaim, int programIdClaim)
        {
            string notificationMessage = MessagesConstants.ProfileUpdationNotificationMessage, notificationTitle = MessagesConstants.ProfileUpdatedNotifyTitle;

            var programNotificationSetCheck = await _program.FindAsync(new { id = programIdClaim });
            if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
            {
                var userDeviceIds = await _userRepository.GetDataByIdAsync(new { Id = userIdClaim });
                if (userDeviceIds != null)
                {
                    var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                    PushNotifications push = new PushNotifications();
                    var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(new List<int> { userDeviceIds.Id }, (int)NotificationSettingsEnum.Personal);
                    if (usrNotify.Count > 0)
                    {
                        await push.SendPushBulk(new List<string> { userDeviceIds.UserDeviceId }, notificationTitle, notificationMessage, "", userIdClaim.ToString(), "personal", "icon", "personal", 1, (serverApiKey != null ? serverApiKey.Value : ""), false, "EditProfile", 0);
                    }
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
                ProgramId = programIdClaim,
                userId = userIdClaim,
                IsRedirect = false,
                NotificationSubType = "EditProfile",
                CustomReferenceId = 0
            });
        }

        /// <summary>
        /// This Api is called to link the user account with program with verification code sent to user via email/sms.
        /// </summary>
        /// <param name="model">LinkAccountModel</param>
        /// <returns></returns>
        [Route("LinkAccountForProgram")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> LinkAccountValidationProgram(LinkAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimLinkAccount = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimLinkAccount = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimLinkAccount, sessionIdClaimLinkAccount)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var usr = _mapper.Map<UserDto>(await _userRepository.CheckUserByEmail(model.EmailAddress));
                if (usr == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidUser.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                var checkUserProgramLinkAccount = await _usersProgram.GetUserProgramLinkingByEmailNID(model.ProgramId, model.UserId.Trim(), model.EmailAddress.Trim());
                if (checkUserProgramLinkAccount == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoProgramAssociationWithId));
                }
                var tokenLinkAccount = _mapper.Map<UserProgramDto>(await _usersProgram.UpdateUserProgramNUserReturn(model.UserId, model.ProgramId, model.EmailAddress));
                string smsemailSettingLinkAccount = await LinkAccountValidationProgramRefactor(model, tokenLinkAccount);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, string.Concat(MessagesConstants.ValidationCodeSent, smsemailSettingLinkAccount == "1" ? string.Format("email address {0}.", model.EmailAddress) : "registered phone number."), tokenLinkAccount));  // 200
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := LinkAccountValidationProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task<string> LinkAccountValidationProgramRefactor(LinkAccountModel model, UserProgramDto token)
        {
            string smsemailSettingProgramRefactor = "1";
            if (token != null)
            {
                try
                {
                    var users = await _userRepository.GetUsersDetailByUserCode(new List<string> { model.UserId });
                    var userDetail = users.FirstOrDefault();
                    if (userDetail != null)
                    {
                        var emailSMSSettingsLinkAccountValidation = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMSEmail)).FirstOrDefault();
                        if (emailSMSSettingsLinkAccountValidation != null)
                        {
                            smsemailSettingProgramRefactor = emailSMSSettingsLinkAccountValidation.Value;
                            if (emailSMSSettingsLinkAccountValidation.Value == "1")
                            { /*Email*/
                                var template = await _emailService.GetEmailTemplateByName(EmailTemplates.LinkAccountVerification);
                                template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetail.FirstName, " ", userDetail.LastName)).Replace("{token}", token.linkAccountVerificationCode);
                                await _emailService.SendEmail(model.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                            }
                            else
                            {  /* SMS */
                                var smstemplate = await _sMSService.GetSMSTemplateByName(SMSTemplates.LinkAccountVerification);
                                smstemplate.body = smstemplate.body.Replace("{Name}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{token}", token.linkAccountVerificationCode);
                                await _sMSService.SendSMS(_configuration["accountSID"], _configuration["authToken"], userDetail?.PhoneNumber, smstemplate.body);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := LinkAccountValidationProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
            }
            return smsemailSettingProgramRefactor;
        }

        /// <summary>
        /// This Api is used to validate verification code for linking program with user.
        /// </summary>
        /// <param name="model">LinkAccountValidateModel</param>
        /// <returns></returns>
        [Route("ValidateLinkVerificationCodeForProgram")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> LinkAccountValidationCodeForProgram(LinkAccountValidateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimLnkValidation = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimLnkValidation = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimLnkValidation, sessionIdClaimLnkValidation)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var validateLnkValidation = await _usersProgram.ValidateLinkAccountCode(model.UserId, model.ProgramId, model.VerificationCode);
                if (validateLnkValidation == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.InvalidVerificationCode.Replace("{supportemail}", _configuration["SupportEmail"]))); // 404
                }
                if (validateLnkValidation.verificationCodeValidTill < DateTime.UtcNow)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.VerificationCodeExpired));
                }
                if (validateLnkValidation.isVerificationCodeDone.Value)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OneTimeUserVerificationCode));
                }
                var userLnkValidation = _mapper.Map<UserDto>(await _usersProgram.UpdateValidationCodeNUserDetailReturn(model.UserId, model.ProgramId, true));
                if (userLnkValidation == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.LinkingProgramWithUserAlreadyExists));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.GetLinkingProgramSuccessMessage, userLnkValidation));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := LinkAccountValidationCodeForProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to update user device and location from mobile app.
        /// </summary>
        /// <param name="model">DeviceLocationModel</param>
        /// <returns></returns>
        [Route("UpdateUserDeviceLocationInfo")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUserDeviceLocationInfo(DeviceLocationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var user = _mapper.Map<User>(model);
                var userNotifications = await _userRepository.UpdateUserDeviceAndLocationInfo(user);
                if (userNotifications <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserDevicecNLocationSettingsNotUpdated));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserDevicecNLocationSettingsUpdated, userNotifications));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := UpdateUserDeviceLocationInfo)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to add/update admins from web for organisation/merchant.
        /// </summary>
        /// <param name="model">OrganisationAdminViewDetail</param>
        /// <returns></returns>
        [Route("AddUpdateAdminUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateAdminUser(OrganisationAdminViewDetail model)
        {
            try
            {
                var userPhoneCheckForUpdateAdminUser = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (userPhoneCheckForUpdateAdminUser != null && !string.IsNullOrEmpty(model.EmailAddress) && !userPhoneCheckForUpdateAdminUser.Email.Trim().Equals(model.EmailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PhoneNumberExists, "0"));
                }
                var userEmailCheck = await _userRepository.CheckUserExistence(model.EmailAddress);
                if (userEmailCheck != null)
                {
                    if (model.UserId > 0 && model.UserId == userEmailCheck.Id) { /*Empty*/ }
                    else
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.EmailExists, "0"));
                }

                var userId = await _userRepository.AddUpdateAdminUser(model);
                if (userId <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserUnSuccessfulRegistration, "0"));
                }
                await AddUpdateAdminUserRefactor(model, userId);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, userId));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := AddUpdateAdminUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task AddUpdateAdminUserRefactor(OrganisationAdminViewDetail model, int userId)
        {
            if (model.UserId <= 0)
            {
                try
                {
                    var URLRedirectLink = string.Concat(_configuration["WebURL"], "Account/Index?id=", Cryptography.EncryptPlainToCipher(model.EmailAddress) + "&uid=" + Cryptography.EncryptPlainToCipher(userId.ToString()) + "&pid=" + Cryptography.EncryptPlainToCipher("0") + "&ptype=" + Cryptography.EncryptPlainToCipher("1"));
                    var orgDetail = await _organisation.GetDataByIdAsync(new { Id = model.OrganisationId });
                    if (orgDetail.organisationType != Convert.ToInt32(Constants.OrganasationType.Merchant))
                    {
                        var emailtTemplateForAdminUserRefactor = await _emailService.GetEmailTemplateByName(EmailTemplates.OrganizationAdminIntimation);
                        emailtTemplateForAdminUserRefactor.Body = emailtTemplateForAdminUserRefactor.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(model.Name, " ", model.LastName)).Replace("{link}", URLRedirectLink).Replace("{roleName}", model.RoleId == 3 ? Roles.OrganizationFull.Replace("Organization ", "").Replace("Organisation ", "") : Roles.OrganizationReporting.Replace("Organization ", "").Replace("Organisation ", "")).Replace("{orgName}", orgDetail.OrganisationSubTitle != null ? orgDetail.OrganisationSubTitle : "");
                        await _emailService.SendEmail(model.EmailAddress, emailtTemplateForAdminUserRefactor.Subject, emailtTemplateForAdminUserRefactor.Body, emailtTemplateForAdminUserRefactor.CCEmail, emailtTemplateForAdminUserRefactor.BCCEmail);
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Account := AddUpdateAdminUser (Email Part))", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }
            }
        }

        /// <summary>
        /// This Api is called to add/update admins from web for program.
        /// </summary>
        /// <param name="model">ProgramLevelAdminViewDetail</param>
        /// <returns></returns>
        [Route("AddUpdateProgramLevelAdminUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateProgramLevelAdminUser(ProgramLevelAdminViewDetail model)
        {
            try
            {
                var userPhoneCheckProgramLevelAdminUser = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (userPhoneCheckProgramLevelAdminUser != null && !string.IsNullOrEmpty(model.EmailAddress) && !userPhoneCheckProgramLevelAdminUser.Email.Trim().Equals(model.EmailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PhoneNumberExists, "0"));
                }
                var userEmailCheck = await _userRepository.CheckUserExistence(model.EmailAddress);
                if (userEmailCheck != null)
                {
                    if (model.UserId > 0 && model.UserId == userEmailCheck.Id) { /*Empty*/ }
                    else
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.EmailExists, "0"));
                }

                var userId = await _userRepository.AddUpdateProgramLevelAdminUser(model);
                if (userId <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserUnSuccessfulRegistration, "0"));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, userId));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := AddUpdateProgramLevelAdminUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api called to delete admin user from web.
        /// </summary>
        /// <param name="model">UserDto</param>
        /// <returns></returns>
        [Route("DeleteAdminUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAdminUser(UserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var user = await _userRepository.DeleteAdminUser(model.Id);
                if (user <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotDeletedSuccessfully));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserDeletedSuccessfully, user));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := DeleteAdminUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to invite merchant level admin via email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("InviteAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> InviteAdmin(string email, int id)
        {
            try
            {
                var orgAdmin = (await _organisation.GetOrganisationsAdminsList(id)).ToList();
                if (!string.IsNullOrEmpty(email))
                {
                    orgAdmin = orgAdmin.Where(x => x.EmailAddress == email).ToList();
                }
                foreach (var item in orgAdmin)
                {
                    try
                    {
                        var lst = new List<string>();
                        lst.Add(item.EmailAddress);
                        var userId = (await _userRepository.GetUsersDetailByEmailIds(lst)).FirstOrDefault().Id;
                        var userDetails = orgAdmin.FirstOrDefault(x => x.UserId == userId);
                        if (userDetails.IsAdmin && userDetails.InvitationStatus == 0 && !userDetails.EmailConfirmed)
                        {
                            var obj = (await _userRepository.FindAsync(new { id = userId }));
                            obj.InvitationStatus = 1;
                            await _userRepository.UpdateAsync(obj, new { id = userId });
                        }
                        else if (userDetails.IsAdmin && userDetails.InvitationStatus == 1 && !userDetails.EmailConfirmed)
                        {
                            var obj = (await _userRepository.FindAsync(new { id = userId }));
                            obj.InvitationStatus = 2;
                            await _userRepository.UpdateAsync(obj, new { id = userId });
                        }
                        var URLRedirectLink = string.Concat(_configuration["WebURL"], "Account/Index?id=", Cryptography.EncryptPlainToCipher(item.EmailAddress) + "&uid=" + Cryptography.EncryptPlainToCipher(userId.ToString()) + "&pid=" + Cryptography.EncryptPlainToCipher("0") + "&ptype=" + Cryptography.EncryptPlainToCipher("1"));
                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.MerchantAdminIntimation);
                        var orgDetail = await _organisation.GetDataByIdAsync(new { Id = id });
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", userDetails.Name).Replace("{link}", URLRedirectLink).Replace("{roleName}", userDetails.RoleName == "Merchant Full" ? Roles.MerchantFull.Replace("Merchant ", "").Replace("Merchant ", "") : Roles.MerchantReporting.Replace("Merchant ", "").Replace("Merchant ", "")).Replace("{merchantName}", orgDetail != null ? orgDetail.name : "");
                        await _emailService.SendEmail(item.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Accouont := InviteAdmin) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.InvitationSuccessfulSent, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Accouont := InviteAdmin) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.InvitationSuccessfulSent));
        }

        /// <summary>
        /// This Api is called to delete account holders from the web.
        /// </summary>
        /// <param name="model">AccountHolderDto</param>
        /// <returns></returns>
        [Route("DeleteAccountHolder")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAccountHolder(AccountHolderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Delete User */
                var DeleteUserDetail = await _userRepository.RemoveAsync(new { Id = model.Id });
                if (DeleteUserDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserNotDeletedSuccessfully));
                }

                if (!string.IsNullOrEmpty(model.Jpos_AccountEncId))
                {
                    model.Jpos_AccountHolderId = Cryptography.DecryptCipherToPlain(model.Jpos_AccountEncId);
                    var oACHJpos = new IssuerJPOSDto()
                    {
                        active = false
                    };
                    await _sharedPJposService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.AccountHolder, oACHJpos, model.Jpos_AccountHolderId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.AccountHolder);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserDeletedSuccessfully, DeleteUserDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := DeleteAccountHolder)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api called to get the list of account holders based on parameters.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="programId"></param>
        /// <param name="searchValue"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortColumnName"></param>
        /// <param name="sortOrderDirection"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [Route("GetAccountHolders")]
        [HttpGet]
        public async Task<IActionResult> GetAccountHoldersList(int organisationId, int programId, string searchValue, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, int? planId)
        {
            try
            {
                /* Account holders users */
                var UsersAccounts = (await _userRepository.GetAccountHoldersList(organisationId, programId, searchValue, pageNumber, pageSize, sortColumnName, sortOrderDirection, planId)).ToList();
                if (UsersAccounts.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, UsersAccounts, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetAccountHoldersList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="programId"></param>
        /// <param name="searchValue"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortColumnName"></param>
        /// <param name="sortOrderDirection"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [Route("GetAccountHoldersListByOrganization")]
        [HttpGet]
        public async Task<IActionResult> GetAccountHoldersListByOrganization(int organisationId, int programId, string searchValue, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, int? planId)
        {
            try
            {
                /* Account holders users */
                var UsersAccounts = (await _userRepository.GetAccountHoldersListByOrganization(organisationId, programId, searchValue, pageNumber, pageSize, sortColumnName, sortOrderDirection, planId)).ToList();
                if (UsersAccounts.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, UsersAccounts, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetAccountHoldersList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is called to register the user in the system
        /// </summary>
        /// <param name="model">RegisterViewModel</param>
        /// <returns>User</returns>
        [Route("RegisterAccountHolder")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAccountHolder(RegisterAccountHolderModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(ModelState);
                }
                var chkUserByEmail = await _userRepository.CheckUserByEmail(model.PrimaryEmail);
                var chkUserByPhone = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (chkUserByEmail != null && model.Id < 0 && model.Id != chkUserByEmail.Id)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.EmailExists));
                }
                if (chkUserByPhone != null && model.Id < 0 && model.Id != chkUserByPhone.Id)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PhoneNumberExists));
                }

                var userModel = new User
                {
                    UserCode = model.AccountHolderUniqueId,
                    customInfo = model.CustomFields,
                    dateOfBirth = model.DateOfBirth,
                    FirstName = model.FirstName,
                    genderId = model.GenderId,
                    Id = model.Id,
                    LastName = model.LastName,
                    OrganisationId = model.OrganizationId,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.PrimaryEmail,
                    ProgramId = model.ProgramId,
                    secondaryEmail = model.SecondaryEmail,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    IsMobileRegistered = model.IsMobileRegistered,
                    InvitationStatus = model.InvitationStatus
                };
                List<UserPlans> planIdsList = new List<UserPlans>();
                foreach (var item in model.PlanIds)
                {
                    planIdsList.Add(new UserPlans() { programPackageId = item });
                }
                string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var issuerId = (await _program.FindAsync(new { Id = model.ProgramId }))?.JPOS_IssuerId;
                var result = await _userRepository.AddUpdateAccountHolderDetail(userModel, planIdsList, model.UserImagePath, clientIpAddress, issuerId);
                if (result > 0 && model.Id <= 0)
                {
                    await RegisterAccountHolderRefactor(model, userModel, planIdsList, result);
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AccountholderAddedSuccessfully, Cryptography.EncryptPlainToCipher(result.ToString()), 1));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AccountholderAddedSuccessfully, Cryptography.EncryptPlainToCipher(result.ToString()), 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := RegisterAccountHolder)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private async Task RegisterAccountHolderRefactor(RegisterAccountHolderModel model, User userModel, List<UserPlans> planIdsList, int result)
        {
            string strRole = Roles.BasicUser;
            Role role = new Role();
            role.Name = strRole;
            if (await _roleManager.FindByNameAsync(strRole) == null)
            {
                await _roleManager.CreateAsync(role);
            }
            userModel.Id = result;
            var programAccount = await _planProgramAccountLinkingService.GetProgramAccountsDetailsByPlanIds(planIdsList.Select(m => m.programPackageId.Value).ToList());
            List<UserTransactionInfo> userTransactionList = new List<UserTransactionInfo>();
            foreach (var item in programAccount)
            {
                if (item.AccountTypeId != 3 && item.AccountTypeId != 4)
                {
                    var userTransaction = new UserTransactionInfo()
                    {
                        accountTypeId = item.AccountTypeId,
                        creditUserId = result,
                        isActive = true,
                        isDeleted = false,
                        programId = item.ProgramId,
                        transactionAmount = item.InitialBalance,
                        transactionDate = DateTime.UtcNow,
                        CreditTransactionUserType = TransactionUserEnityType.User,
                        DebitTransactionUserType = TransactionUserEnityType.Admin,
                        programAccountId = item.ProgramAccountId,
                        createdBy = model.CreatedBy,
                        modifiedBy = model.ModifiedBy,
                        createdDate = DateTime.UtcNow,
                        modifiedDate = DateTime.UtcNow,
                        organisationId = model.OrganizationId,
                        planId = item.PlanId,
                        debitUserId = model.CreatedBy,
                        transactionStatus = 0
                    };
                    userTransactionList.Add(userTransaction);
                }
            }
            await _userTransactionInfoes.AddAsync(userTransactionList);
            await _userManager.AddUserRole(userModel.Email, strRole);
            await _userManager.AddClaimAsync(userModel, new System.Security.Claims.Claim("userName", userModel.Email));
            await _userManager.AddClaimAsync(userModel, new System.Security.Claims.Claim("firstName", userModel.FirstName));
            if (!string.IsNullOrEmpty(userModel.LastName))
            { await _userManager.AddClaimAsync(userModel, new System.Security.Claims.Claim("lastName", userModel.LastName)); }
            await _userManager.AddClaimAsync(userModel, new System.Security.Claims.Claim("email", userModel.Email));
            await _userManager.AddClaimAsync(userModel, new System.Security.Claims.Claim("role", strRole));
            await _usersProgram.AddUserInProgram(result, model.ProgramId);

            List<UserNotificationSettings> oLstUserNotify = new List<UserNotificationSettings>();
            for (var i = 1; i <= 4; i++)
            {
                var oUserNotify = new UserNotificationSettings()
                {
                    IsNotificationEnabled = true,
                    notificationId = Convert.ToInt32(i),
                    userId = result
                };
                oLstUserNotify.Add(oUserNotify);
            }
            await _userNotificationSettingsService.AddAsync(oLstUserNotify);

            var userPushedNotificationStatus = new UserPushedNotificationsStatus()
            {
                userId = result,
                notificationId = 0,
                notificationReadDate = DateTime.UtcNow,
                userDeviceId = "",
                userDeviceType = ""
            };
            await _userPushedNotificationsStatusService.AddAsync(userPushedNotificationStatus);
        }

        /// <summary>
        /// This Api is called to get the account holder detail of the user.
        /// </summary>
        /// <param name="userEncId"></param>
        /// <param name="programEncId"></param>
        /// <returns></returns>
        [Route("GetAccountHolderDetail")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAccountHolderDetail(string userEncId, string programEncId)
        {
            try
            {
                int userId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(userEncId));
                int programId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(programEncId));
                string programCustomFields = string.Empty; string customColumnName = string.Empty;
                if (programId > 0)
                {
                    var prgDetail = await _program.GetSingleDataByConditionAsync(new { Id = programId });
                    programCustomFields = prgDetail?.programCustomFields;
                    customColumnName = prgDetail?.customName;
                }
                /* Account holders users */
                var UsersAccountDetail = (await _userRepository.GetUserInfoWithUserPlans(Convert.ToInt32(Cryptography.DecryptCipherToPlain(userEncId)), Convert.ToInt32(Cryptography.DecryptCipherToPlain(programEncId))));
                if (UsersAccountDetail == null && userId > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, UsersAccountDetail, 0, programCustomFields, false, true, customColumnName));
                }
                if (userId <= 0)
                {
                    UsersAccountDetail = new AccountHolderDto();
                    UsersAccountDetail.AccountHolderID = string.Concat("AHD1000-", Convert.ToString(await _userRepository.GetPrimaryMaxAsync() + 1));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, UsersAccountDetail, 1, programCustomFields, false, true, customColumnName));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetAccountHolderDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to update user device and location from mobile app.
        /// </summary>
        /// <param name="model">DeviceLocationModel</param>
        /// <returns></returns>
        [Route("UpdateUserCardholderAgreementInfo")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUserCardholderAgreementInfo(UserCardholderAgreementModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }

                var identityUpdateUser = User.Identity as ClaimsIdentity;
                var userIdClaimUpdateUser = Convert.ToInt32(identityUpdateUser.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimUpdateUser = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimUpdateUser = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimUpdateUser, sessionIdClaimUpdateUser)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                model.Id = userIdClaimUpdateUser;
                var user = new User()
                {
                    Id = model.Id,
                    AgreementVersionNo = model.AgreementVersionNo,
                    IsAgreementRead = model.IsAgreementRead
                };
                var userAgreement = await _userRepository.UpdateUserCardHolderAgreementReadDetail(user);
                if (userAgreement <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserCardholderAgreementSettingsNotUpdated));
                }
                var userAgreementHistory = new UserAgreementHistory()
                {
                    cardHolderAgreementVersionNo = model.AgreementVersionNo,
                    createdBy = userIdClaimUpdateUser,
                    modifiedBy = userIdClaimUpdateUser,
                    programId = programIdClaimUpdateUser,
                    userId = userIdClaimUpdateUser,
                    dateAccepted = DateTime.UtcNow,
                    createdOn = DateTime.UtcNow,
                    modifiedOn = DateTime.UtcNow
                };
                await _userAgreementHistoryService.AddAsync(userAgreementHistory);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserCardholderAgreementSettingsUpdated, userAgreement));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := UpdateUserCardholderAgreementInfo)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to get the Fiserv response webhook.
        /// </summary>
        /// <returns></returns>
        [Route("FiservResponse")]
        [HttpGet]
        public async Task<IActionResult> GetFiservResponse()
        {
            try
            {
                await Task.FromResult(1);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.FiservResponseExist, null));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetFiservResponse)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to get the User loyalty tracking balance.
        /// </summary>
        /// <returns></returns>
        [Route("GetBiteUserLoyaltyTrackingBalance")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBiteUserLoyaltyTrackingBalance(int userId)
        {
            try
            {
              
               
                var UsersAccountDetail = (await _userRepository.GetBiteUserLoyaltyTrackingBalance(userId));
                if (UsersAccountDetail == null && userId > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, UsersAccountDetail,0));
                }
             return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, UsersAccountDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetBiteUserLoyaltyTrackingBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is called to get the User loyalty tracking transactions.
        /// </summary>
        /// <returns></returns>
        [Route("GetUserLoyaltyTrackingTransactions")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserLoyaltyTrackingTransactions(int userId,int pageNumber,int pageSize,string sortColumnName,string  sortOrderDirection)
        {
            try
            {


                var Usertransactions = (await _userLoyaltyPointsHistoryInfo.GetUserLoyaltyTrackingHistory(userId,pageNumber,pageSize,sortColumnName,sortOrderDirection));
                if (Usertransactions == null && userId > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, Usertransactions, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, Usertransactions, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetUserLoyaltyTrackingTransactions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }
    }
}

