using AutoMapper;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Foundry.LogService;
using Foundry.Identity;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using System.Net.Http.Headers;
using Foundry.Services.AcquirerService;
using foundry;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include methods for importing user from excel and other methods.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImportUserController : ControllerBase
    {
        private readonly CustomUserManager _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUsersProgram _usersProgram;
        private readonly IPrograms _programs;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ISMSService _sMSService;
        private readonly IGeneralSettingService _setting;
        private readonly ApiResponse RecordNoFound = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRecordExists);
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="programs"></param>
        /// <param name="userRepository"></param>
        /// <param name="configuration"></param>
        /// <param name="emailService"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="usersProgram"></param>
        /// <param name="sMSService"></param>
        /// <param name="setting"></param>
        public ImportUserController(IPrograms programs, IUserRepository userRepository, IConfiguration configuration, IEmailService emailService,
            CustomUserManager userManager, RoleManager<Role> roleManager,
             IUsersProgram usersProgram, ISMSService sMSService, IGeneralSettingService setting)
        {
            _programs = programs;
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _usersProgram = usersProgram;
            _sMSService = sMSService;
            _setting = setting;
        }

        #region  Import User
        /// <summary>
        /// This Api is called to get maximum sheet rows in excel.
        /// </summary>
        /// <returns></returns>
        [Route("GetMaximumSheetRows")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMaximumSheetRows()
        {
            try
            {
                List<GeneralSettingDto> generalSettingObj = (await _programs.GetMaximumSheetRows()).ToList();
                if (generalSettingObj.Count <= 0)
                {
                    return Ok(RecordNoFound);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, generalSettingObj.FirstOrDefault()));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetMaximumSheetRows)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to add users from excel in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUserFromExcel")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserFromExcel(User model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                List<string> userCodesList = new List<string> { model.UserCode };
                var userCode = await _userRepository.GetUsersDetailByUserCode(userCodesList);
                if (userCode.Any())
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserCodeAlreadyExists, "0"));
                }
                var userId = await _userRepository.AddUserFromExcel(model);
                if (userId <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserUnSuccessfulRegistration, "0"));
                }
                string strRole = Roles.BasicUser;
                Role role = new Role();
                role.Name = strRole;
                if (await _roleManager.FindByNameAsync(strRole) == null)
                {
                    await _roleManager.CreateAsync(role);
                }
                model.Id = userId;
                await _userManager.AddUserRole(model.Email, strRole);
                await _userManager.AddClaimAsync(model, new Claim("userName", model.Email));
                await _userManager.AddClaimAsync(model, new Claim("firstName", model.FirstName));
                if (!string.IsNullOrEmpty(model.LastName))
                { await _userManager.AddClaimAsync(model, new Claim("lastName", model.LastName)); }
                await _userManager.AddClaimAsync(model, new Claim("email", model.Email));
                await _userManager.AddClaimAsync(model, new Claim("role", strRole));


                await _usersProgram.AddUserInProgram(userId, model.ProgramId.Value);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, userId));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := AddUpdateProgramLevelAdminUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is called to check user existence in the system by email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("CheckUserByEmail")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckUserByEmail(string email)
        {
            try
            {
                var userExistsByMail = (await _userRepository.CheckUserByEmail(email));
                if (userExistsByMail == null || userExistsByMail.Id <= 0)
                {
                    return Ok(RecordNoFound);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userExistsByMail));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetMaximumSheetRows)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called check the phone number existencen in the system.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [Route("CheckPhoneNumberExistence")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckPhoneNumberExistence(string phoneNumber)
        {
            try
            {
                var userExistWithNumberExistence = (await _userRepository.CheckPhoneNumberExistence(phoneNumber));
                if (userExistWithNumberExistence == null || userExistWithNumberExistence.Id <= 0)
                {
                    return Ok(RecordNoFound);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userExistWithNumberExistence));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetMaximumSheetRows)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get user details using alphanumeric user code.
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [Route("GetUserbyUserCode")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserbyUserCode(string userCode)
        {
            try
            {
                List<string> userCodeList = new List<string> { userCode };
                var userExistByUserCode = (await _userRepository.GetUsersDetailByUserCode(userCodeList));
                if (!userExistByUserCode.Any())
                {
                    return Ok(RecordNoFound);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userExistByUserCode.FirstOrDefault()));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetUserbyUserCode)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to send magic link to user on adding the user in program.
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [Route("SendMagicLinkInEmail")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMagicLinkInEmail(List<int> userIds)
        {
            try
            {
                string firebaseUrl = _configuration["FirebaseUrl"];
                string webApiKey = _configuration["WebApiKey"];
                string fbDomainUriPrefix = _configuration["DomainUriPrefix"];
                string androidPkgName = _configuration["AndroidPackageName"];
                string iosBndlId = _configuration["IosBundleId"];
                var objFirebase = await SendShortLinkinMail(userIds, firebaseUrl, webApiKey, fbDomainUriPrefix, androidPkgName, iosBndlId);
                if (objFirebase == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoErrorMessagesExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, objFirebase, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (ImportUser := SendMagicLinkInEmail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        private async Task<ApiResponse> SendShortLinkinMail(List<int> userIds, string firebaseUrl, string webApiKey, string fbDomainUriPrefix, string androidPkgName, string iosBndlId)
        {
            try
            {
                var usersDetail = await _userRepository.GetUserByIdWithProgramDetail(userIds);
                FirebaseDynamicLinkResponseModel response = new FirebaseDynamicLinkResponseModel();
                if (usersDetail.Count > 0)
                {

                    foreach (var userObj in usersDetail)
                    {
                        try
                        {
                            FirebaseDynamicLinkModel fbModel = new FirebaseDynamicLinkModel
                            {
                                dynamicLinkInfo = new Dynamiclinkinfo
                                {
                                    domainUriPrefix = fbDomainUriPrefix,
                                    link = fbDomainUriPrefix + "?studentID=" + userObj.Id + "&programID=" + userObj.ProgramCode,
                                    androidInfo = new Androidinfo
                                    {
                                        androidPackageName = androidPkgName
                                    },
                                    iosInfo = new Iosinfo
                                    {
                                        iosBundleId = iosBndlId
                                    }
                                }
                            };


                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri(firebaseUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            response = await SendShortLinkInMailRefactor(webApiKey, response, userObj, fbModel, client);
                        }
                        catch (Exception ex)
                        {
                            HttpContext.RiseError(new Exception(string.Concat("API := (ImportUser := SendShortLinkinMail - Inner Catch)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                        }
                    }


                }
                return new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserSuccessfulRegistration, response);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := SendShortLinkinMail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing);
            }


        }

        private async Task<FirebaseDynamicLinkResponseModel> SendShortLinkInMailRefactor(string webApiKey, FirebaseDynamicLinkResponseModel response, UserDto userObj, FirebaseDynamicLinkModel fbModel, HttpClient client)
        {
            HttpResponseMessage resp = client.PostAsJsonAsync("shortLinks?key=" + webApiKey, fbModel).Result;
            switch (resp.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    response = resp.Content.ReadAsAsync<FirebaseDynamicLinkResponseModel>().Result;
                    var emailSMSSettings = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMSEmail)).FirstOrDefault();
                    if (emailSMSSettings != null)
                    {
                        if (emailSMSSettings.Value == "1")
                        {
                            var template = await _emailService.GetEmailTemplateByName(EmailTemplates.MagicLink);

                            template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png").ToString())
                                .Replace("{Name}", string.Concat(userObj.FirstName, " ", userObj.LastName))
                                .Replace("{link}", response.shortLink).Replace("{UserCode}", userObj.UserCode);

                            await _emailService.SendEmail(userObj.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                        }
                        else
                        {
                            try
                            {
                                var smstemplate = await _sMSService.GetSMSTemplateByName(SMSTemplates.MagicLink);
                                smstemplate.body = smstemplate.body.Replace("{Name}", string.Concat(userObj.FirstName, " ", userObj.LastName)).Replace("{link}", response.shortLink);
                                await _sMSService.SendSMS(_configuration["accountSID"], _configuration["authToken"], userObj.PhoneNumber, smstemplate.body);
                            }
                            catch (Exception ex)
                            {
                                HttpContext.RiseError(new Exception(string.Concat("API := (ImportUser := SendShortLinkinMail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                            }
                        }
                    }
                    var userData = await _userRepository.GetDataByIdAsync(new { id = userObj.Id });
                    if (userData != null)
                    {
                        userData.InvitationStatus = 2;
                        await _userRepository.AddUpdateUserInvitationStatus(userData);
                    }

                    break;
                default:
                    break;
            }

            return response;
        }

        #endregion

    }
}

