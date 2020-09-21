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
using Foundry.Api.Attributes;
using System.Globalization;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class include methods related to program and plans.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IUsersProgram _usersProgram;
        private readonly IPrograms _programs;
        private readonly IProgramTypeService _programType;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IPlanService _planService;
        private readonly IProgramAccountService _programAccountService;
        private readonly IProgramBrandingService _programBrandingService;
        private readonly IOrganisation _organisation;
        private readonly IAccountMerchantRulesService _accMerchantRule;
        private readonly IAccountTypeService _accountType;
        private readonly ILoggerManager _logger;
        private readonly IPlanProgramAccountLinkingService _planProgramAccountLinkingService;
        private readonly IUserPlanService _userPlanService;
        private readonly IUserTransactionInfoes _userTransactionService;
        private readonly IOrganisationProgram _organisationProgramService;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICardHolderAgreementService _cardHolderAgreementService;
        private readonly II2CAccountDetailService _i2cAccountDetailRepository;
        private readonly ISharedJPOSService _sharedJposService;
        private readonly IUserAgreementHistoryService _userAgreementHistory;
        private readonly ApiResponse noSessionMatchExist = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true);
        private readonly ApiResponse NoProgramsExist = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoProgramsExist);
        private readonly ApiResponse someIssueInProcessing = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing);
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="programs"></param>
        /// <param name="mapper"></param>
        /// <param name="userRepository"></param>
        /// <param name="programType"></param>
        /// <param name="configuration"></param>
        /// <param name="emailService"></param>
        /// <param name="planService"></param>
        /// <param name="logger"></param>
        /// <param name="programAccountService"></param>
        /// <param name="programBrandingService"></param>
        /// <param name="organisation"></param>
        /// <param name="accMerchantRule"></param>
        /// <param name="usersProgram"></param>
        /// <param name="planProgramAccountLinkingService"></param>
        /// <param name="accountType"></param>
        /// <param name="userPlanService"></param>
        /// <param name="userTransactionService"></param>
        /// <param name="organisationProgramService"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="cardHolderAgreementService"></param>
        /// <param name="i2cAccountDetailRepository"></param> 
        /// <param name="sharedJposService"></param> 
        /// <param name="userAgreementHistory"></param>
        public ProgramController(IPrograms programs, IMapper mapper, IUserRepository userRepository, IProgramTypeService programType, IConfiguration configuration, IEmailService emailService, IPlanService planService,
            ILoggerManager logger, IProgramAccountService programAccountService, IProgramBrandingService programBrandingService, IOrganisation organisation,
            IAccountMerchantRulesService accMerchantRule,
              IUsersProgram usersProgram, IPlanProgramAccountLinkingService planProgramAccountLinkingService,
             IAccountTypeService accountType, IUserPlanService userPlanService, IUserTransactionInfoes userTransactionService,
             IOrganisationProgram organisationProgramService, IUserRoleRepository userRoleRepository, IRoleRepository roleRepository,
             ICardHolderAgreementService cardHolderAgreementService,
             II2CAccountDetailService i2cAccountDetailRepository, ISharedJPOSService sharedJposService,
             IUserAgreementHistoryService userAgreementHistory)
        {
            _programs = programs;
            _mapper = mapper;
            _userRepository = userRepository;
            _programType = programType;
            _configuration = configuration;
            _emailService = emailService;
            _planService = planService;
            _logger = logger;
            _programAccountService = programAccountService;
            _programBrandingService = programBrandingService;
            _organisation = organisation;
            _accMerchantRule = accMerchantRule;
            _usersProgram = usersProgram;
            _planProgramAccountLinkingService = planProgramAccountLinkingService;
            _accountType = accountType;
            _userPlanService = userPlanService;
            _userTransactionService = userTransactionService;
            _organisationProgramService = organisationProgramService;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _cardHolderAgreementService = cardHolderAgreementService;
            _i2cAccountDetailRepository = i2cAccountDetailRepository;
            _sharedJposService = sharedJposService;
            _userAgreementHistory = userAgreementHistory;
        }

        /// <summary>
        /// This Api is used to get all programs
        /// </summary>
        /// <returns></returns>
        [Route("Programs")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProgramCodes()
        {
            try
            {
                var programs = (await _programs.GetPrograms()).ToList();
                if (programs.Count <= 0)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, programs));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramCodes)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to check the program validity either is active or not.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("CheckProgramValidity")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckProgramValidity(int programId, int userId)
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programsId = await _programs.CheckProgramExpiration(programIdClaim, userIdClaim);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok(noSessionMatchExist);
                }
                if (programsId <= 0)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, programsId));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := CheckProgramValidity)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to check the linking of program with user.
        /// </summary>
        /// <returns></returns>
        [Route("CheckProgramLinkingWithUser")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckProgramLinkingWithUser()
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var UserUniqueId = Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "UserUniqueId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok(noSessionMatchExist);
                }
                var userPrograms = await _usersProgram.CheckUserLinkingWithProgram(userIdClaim, programIdClaim);
                var cardHolderAgreement = await _cardHolderAgreementService.GetCardHolderAgreementByIdNProgramNUser(programIdClaim, userIdClaim);
                if (cardHolderAgreement != null)
                {
                    cardHolderAgreement.CardholderAgreementURL = string.Concat(_configuration["WebURL"], "Account/ViewCardHolderAgreement?id=", Cryptography.EncryptPlainToCipher(programIdClaim.ToString()));
                }
                var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { AccountHolderUniqueId = UserUniqueId });
                var UserVirtualCardExists = false;
                if (userVirtualCardDetail?.Id > 0)
                {
                    UserVirtualCardExists = true;
                }
                ProgramLinkingNCardholderAgreement prg_cardholder = new ProgramLinkingNCardholderAgreement()
                {
                    ProgramLinking = new ProgramLinking()
                    {
                        IsProgramLinked = userPrograms.isLinkedProgram.Value
                    },
                    CardholderAgreement = cardHolderAgreement,
                    IsUserVirtualCardExists = UserVirtualCardExists
                };
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, prg_cardholder.ProgramLinking.IsProgramLinked ? "Program is associated with user." : "Program is not associated with user.", prg_cardholder));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := CheckProgramLinkingWithUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get programs by Id.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetProgramById")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProgramById(int programId)
        {
            try
            {
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var program = await _programs.GetProgramById(programIdClaim);
                if (program == null)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, program));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get the program detail by its Id.
        /// </summary>
        /// <returns></returns>
        [Route("AboutMealPlan")]
        [Authorize]
        [HttpGet]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetProgramDetail()
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok(noSessionMatchExist);
                }
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var program = _mapper.Map<ProgramDto>(await _programs.GetProgramDetailsById(programIdClaim));
                if (program == null)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, program));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get the master program types from the system.
        /// </summary>
        /// <returns></returns>
        [Route("ProgramTypes")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProgramTypes()
        {
            try
            {
                var programTypes = _mapper.Map<List<ProgramTypesDto>>((await _programType.GetProgramTypes()).ToList());
                if (programTypes.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoProgramTypesExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, programTypes));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramTypes)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get the program level admin list.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("ProgramLevelAdminList")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProgramLevelAdminList(int programId)
        {
            try
            {
                /* Get Admin Organisations. */
                var orgAdminDetails = await _programs.GetProgramAdminsList(programId);
                if (orgAdminDetails == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgAdminDetails, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := ProgramLevelAdminList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to invite program level admin.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("InviteProgramLevelAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> InviteProgramLevelAdmin(string email, int id)
        {
            try
            {
                var orgAdmin = (await _programs.GetOrganisationsAdminsList(id)).ToList();
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
                        var userId = (await _userRepository.GetUsersDetailByEmailIds(lst)).FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false).Id;
                        var userDetails = orgAdmin.FirstOrDefault(x => x.UserId == userId);
                        var obj = (await _userRepository.FindAsync(new { id = userId }));
                        if (userDetails.IsAdmin && userDetails.InvitationStatus == 0 && !userDetails.EmailConfirmed)
                        {
                            obj.InvitationStatus = 1;
                        }
                        else if (userDetails.IsAdmin && userDetails.InvitationStatus == 1 && !userDetails.EmailConfirmed)
                        {
                            obj.InvitationStatus = 2;
                        }
                        await _userRepository.UpdateAsync(obj, new { id = userId });
                        var URLRedirectLink = string.Concat(_configuration["WebURL"], "Account/Index?id=", Cryptography.EncryptPlainToCipher(item.EmailAddress) + "&uid=" + Cryptography.EncryptPlainToCipher(userId.ToString()) + "&pid=" + Cryptography.EncryptPlainToCipher("0") + "&ptype=" + Cryptography.EncryptPlainToCipher("1"));
                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.ProgramAdminIntimation);
                        var orgDetail = await _programs.GetDataByIdAsync(new { Id = id });
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png").ToString()).Replace("{Name}", userDetails.Name).Replace("{link}", URLRedirectLink).Replace("{roleName}", Roles.ProgramFull.Replace("Program ", "")).Replace("{orgName}", orgDetail != null ? orgDetail.name : "");
                        await _emailService.SendEmail(item.EmailAddress, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.RiseError(new Exception(string.Concat("API := (Accouont := InviteAdmin) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    }
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.InvitationSuccessfulSent));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Accouont := InviteAdmin) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
            }
            return Ok(someIssueInProcessing);
        }

        /// <summary>
        /// This Api is used to get the transactions of on program level.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        [Route("Transactions")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTransactions(int programId, string dateMonth)
        {
            try
            {
                DateTime? dateFilter = null;
                if (!string.IsNullOrEmpty(dateMonth))
                    dateFilter = Convert.ToDateTime(dateMonth);
                int orgType = OrganasationType.Merchant;
                List<TransactionViewDto> programs = (await _programs.GetTransaction(orgType, programId, dateFilter)).ToList();
                if (programs.Count <= 0)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, programs));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetTransactions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to add/update program info.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateProgramInfo")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateProgramInfo(ProgramInfoDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Adding/Updating Program Info. */
                model.OrganisationJPOSId = (await _organisation.GetDataByIdAsync(new { Id = model.organisationId })).JPOS_MerchantId;
                string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var updateStatus = await _programs.AddEditProgramInfo(model, clientIpAddress);
                if (updateStatus == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ProgramNotAddUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ProgramAddUpdatedSuccessfully, updateStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AddUpdateProgramInfo)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used to delete program info.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteProgramInfo")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteProgramInfo(ProgramInfoDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Delete Program Info. */
                var DeleteProgramDetail = await _programs.RemoveAsync(new { Id = model.id });

                if (DeleteProgramDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationNotDeletedSuccessfully));
                }
                var oIssuerJPOS = new IssuerJPOSDto()
                {
                    active = false
                };
                await _sharedJposService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.Issuers, oIssuerJPOS, model.JPOS_IssuerId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.Issuers);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationDeletedSuccessfully, DeleteProgramDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := DeleteProgramInfo)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get program info by Id.
        /// </summary>
        /// <param name="prgId"></param>
        /// <returns></returns>
        [Route("ProgramInfoById")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProgramInfoById(int prgId)
        {
            try
            {
                /* Get Program Info by Id. */
                var ProgramDetail = _mapper.Map<ProgramInfoDto>(await _programs.FindAsync(new { Id = prgId, IsActive = true, IsDeleted = false }));
                if (ProgramDetail == null && prgId > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                if (prgId <= 0)
                {
                    ProgramDetail = new ProgramInfoDto();
                    ProgramDetail.ProgramCodeId = string.Concat("P1000-", Convert.ToString(await _programs.GetPrimaryMaxAsync() + 1));
                    ProgramDetail.AccountHolderUniqueId = string.Concat("AC1000-", Convert.ToString(await _programs.GetPrimaryMaxAsync() + 1));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, ProgramDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramInfoById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get all programs list.
        /// </summary>
        /// <param name="isActive"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        [Route("GetAllProgramsList")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProgramsList(bool isActive, bool isDeleted)
        {
            try
            {
                var userIdClaimForAllProgramList = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var roleUserForAllProgramList = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim();

                /* Get All Programs List. */
                var ProgramDetailForAllProgramList = await _programs.GetAllPrograms(isActive, isDeleted, roleUserForAllProgramList, userIdClaimForAllProgramList);
                if (!ProgramDetailForAllProgramList.Any())
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, null, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, ProgramDetailForAllProgramList, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetAllProgramsList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This method is used to return all program with program admin role access.
        /// </summary>
        /// <param name="isActive"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        [Route("GetAllProgramsForPrgAdminList")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProgramsListForPrgAdmin(bool isActive, bool isDeleted)
        {
            try
            {
                var userIdClaimForPrgAdmin = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Get All Programs List. */
                var ProgramDetailForPrgAdmin = await _programs.GetAllProgramsofPrgAdmin(isActive, isDeleted, userIdClaimForPrgAdmin);
                if (!ProgramDetailForPrgAdmin.Any())
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, null, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, ProgramDetailForPrgAdmin, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetAllProgramsList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("GetPrimaryOrgNPrgDetailOfProgramAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPrimaryOrgNPrgDetailOfProgramAdmin(string userId)
        {
            try
            {
                var pId = Convert.ToInt32(userId);
                /* Get all the relations to return. */
                var organisation = await _programs.GetPrimaryOrgNPrgDetailOfProgramAdminQuery(pId);
                if (organisation == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisation, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }

        }
        /// <summary>
        /// This Api is used to get the custom fields of program by its Id.
        /// </summary>
        /// <param name="prgId"></param>
        /// <returns></returns>
        [Route("GetCustomFieldOfProgram")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCustomFieldOfProgramById(int prgId)
        {
            try
            {
                /* Get Program Info by Id. */
                var ProgramDetail = await _programs.FindAsync(new { Id = prgId, IsActive = true, IsDeleted = false });
                if (ProgramDetail == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, new ProgramInfoDto() { customErrorMessaging = ProgramDetail.customErrorMessaging, customInputMask = ProgramDetail.customInputMask, customName = ProgramDetail.customName, customInstructions = ProgramDetail.customInstructions }, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramInfoById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get max program info value.
        /// </summary>
        /// <returns></returns>
        [Route("MaxProgramInfoValue")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMaxProgramInfoValue()
        {
            try
            {
                var ProgramDetail = new ProgramInfoDto();
                ProgramDetail.ProgramCodeId = string.Concat("P1000-", Convert.ToString(await _programs.GetPrimaryMaxAsync() + 1));
                ProgramDetail.AccountHolderUniqueId = string.Concat("AC1000-", Convert.ToString(await _programs.GetPrimaryMaxAsync() + 1));

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, ProgramDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetMaxProgramInfoValue)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to delete program.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteProgram")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteProgram(ProgramListDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var DeleteProgramDetail = await _programs.RemoveAsync(new { Id = model.ProgramId });
                if (DeleteProgramDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ProgramNotDeletedSuccessfully));
                }
                var oIssuerJPOS = new IssuerJPOSDto()
                {
                    active = false
                };
                await _sharedJposService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.Issuers, oIssuerJPOS, model.JPOS_IssuerId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.Issuers);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ProgramDeletedSuccessfully, DeleteProgramDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := DeleteProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get user program.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("GetUserProgramByUserId")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GtUserProgram(int userId)
        {
            try
            {
                var program = await _usersProgram.GetDataByIdAsync(new { userId = userId, IsActive = true, IsDeleted = false, IsLinkedProgram = true });
                if (program == null)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, program.programId, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("ProgramListDropDown")]
        [HttpGet]
        // [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetProgramLstDrpDwn(string uId, string organisationId)
        {
            try
            {
                int userId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(uId));
                int orgId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(organisationId));
                int roleId = (await _userRoleRepository.GetDataByIdAsync(new { UserId = userId })).RoleId;
                string roleName = (await _roleRepository.GetDataByIdAsync(new { Id = roleId })).Name;
                /* Get Organisation Details. */
                var orgDrpDwnDetails = (await _programs.GetProgramListBasedOnUserRole(userId, roleName.Trim().ToLower(), orgId));

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDrpDwnDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationLstDrpDwn)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used get plan listing based on program.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("PlanListingForReports")]
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPlanListingForReport(int programId)
        {
            try
            {
                IEnumerable<PlanListingDto> planForReport = await _planService.GetPlanListing(programId);
                if (planForReport == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planForReport, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetPlanListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }
        /// <summary>
        /// This Api is used to get the notification and reward setting of the program i.e. Either it is shown or hidden.
        /// </summary>
        /// <returns></returns>
        [Route("ProgramNotificationNRewardsSetting")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNoitificationNRewardsSettingOfProgram()
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok(noSessionMatchExist);
                }
                var programsId = await _programs.CheckProgramExpiration(programIdClaim, userIdClaim);
                if (programsId <= 0)
                {
                    return Ok(NoProgramsExist);
                }
                var programDetail = await _programs.FindAsync(new { id = programIdClaim });
                ProgramNotificationNRewardSettingDto programSettingDetail = new ProgramNotificationNRewardSettingDto();
                programSettingDetail.ProgramId = programIdClaim;
                if (programDetail != null)
                {
                    programSettingDetail.IsNotificationToShow = programDetail.IsAllNotificationShow.Value;
                    programSettingDetail.IsRewardToShow = programDetail.IsRewardsShowInApp.Value;
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, programSettingDetail));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetNoitificationNRewardsSettingOfProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        #region Plan 
        /// <summary>
        /// This Api is used get plan listing based on program.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>        
        [Route("PlanListing")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPlanListing(int programId)
        {
            try
            {
                IEnumerable<PlanListingDto> planForlist = await _planService.GetPlanListing(programId);
                if (!planForlist.Any())
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planForlist, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetPlanListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to update plan status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdatePlanStatus")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdatePlanStatus(PlanListingDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get all the relations to return. */
                var updateStatus = await _planService.UpdatePlanStatus(model.Id, model.Status);
                if (updateStatus <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PlanStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PlanAddUpdatedSuccessfully, updateStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := UpdatePlanStatus)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used to delete plan.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeletePlan")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Deleteplan(PlanListingDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var DeletePlanDetail = await _planService.DeletePlanById(model.Id);
                if (DeletePlanDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PlanNotDeletedSuccessfully));
                }
                if (!string.IsNullOrEmpty(model.Jpos_PlanId))
                {
                    var oPlanJPOS = new PlanJposDto()
                    {
                        active = false
                    };
                    await _sharedJposService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.Plans, oPlanJPOS, model.Jpos_PlanId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.Plans);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PlanDeletedSuccessfully, DeletePlanDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := DeletePlan)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to clone plan.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ClonePlan")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlanClone(PlanListingDto model)
        {
            try
            {
                var result = 0;
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var planDetails = await _planService.GetDataByIdAsync(new { id = model.Id });
                result = await _planService.AddAsync(planDetails);
                if (result > 0)
                {
                    var planid = "P1000-" + result.ToString();
                    var chkExist = await _planService.FindAsync(new { id = result });
                    chkExist.planId = planid;
                    chkExist.name = chkExist.name + "_Copy";
                    await _planService.UpdateAsync(chkExist, new { id = result });
                    var planprogramaccountlinking = await _planProgramAccountLinkingService.GetDataAsync(new { planId = model.Id });
                    if (planprogramaccountlinking.Any())
                    {
                        foreach (var item in planprogramaccountlinking)
                        {
                            PlanProgramAccountsLinking objppal = new PlanProgramAccountsLinking();
                            objppal.planId = result;
                            objppal.programAccountId = item.programAccountId;
                            await _planProgramAccountLinkingService.AddAsync(objppal);
                        }
                    }

                }

                if (result <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PlanNotClonedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PlanClonedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := CreatePlanClone)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used to Add/Update plan 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreateModifyPlan")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdatePlan(PlanViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var result = await _planService.AddEditPlanDetails(model, clientIpAddress);
                if (result == "0" || result == "3000") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PlanStatusNotUpdatedSuccessfully)); }

                /* Data for the user entry in user transaction info for initial balance count.*/
                var organisationDetail = (await _organisationProgramService.GetMultipleDataByConditionAsync(new { programId = model.programId })).FirstOrDefault();
                var programAccount = await _planProgramAccountLinkingService.GetProgramAccountsDetailsByPlanIds(new List<int> { Convert.ToInt32(Cryptography.DecryptCipherToPlain(result)) });
                var userPlan = await _userPlanService.GetMultipleDataByConditionAsync(new { programPackageId = Cryptography.DecryptCipherToPlain(result) });
                var userTransactionAccountDetail = await _userTransactionService.GetProgramAccountsDetailsByAccountIds(model.PlanProgramAccount.Select(m => m.programAccountId).ToList());
                if (userTransactionAccountDetail.Count > 0)
                {
                    programAccount = programAccount.Where(x => !userTransactionAccountDetail.Select(m => m.ProgramAccountId).Contains(x.ProgramAccountId)).ToList();
                }
                List<UserTransactionInfo> userTransactionList = new List<UserTransactionInfo>();
                await AddUpdatePlanRefactor(model, organisationDetail, programAccount, userPlan, userTransactionList);

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PlanAddUpdatedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AddUpdatePlan)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        private async Task AddUpdatePlanRefactor(PlanViewModel model, Domain.DbModel.OrganisationProgram organisationDetail, List<ProgramAccountDetailDto> programAccount, IEnumerable<UserPlans> userPlan, List<UserTransactionInfo> userTransactionList)
        {
            if (userPlan.Any())
            {
                foreach (var user in userPlan)
                {
                    AddUpdatePlanRefactorInside(model, organisationDetail, programAccount, userTransactionList, user);
                }
                await _userTransactionService.AddAsync(userTransactionList);
            }
        }

        private static void AddUpdatePlanRefactorInside(PlanViewModel model, Domain.DbModel.OrganisationProgram organisationDetail, List<ProgramAccountDetailDto> programAccount, List<UserTransactionInfo> userTransactionList, UserPlans user)
        {
            foreach (var item in programAccount)
            {
                if (item.AccountTypeId != 3 && item.AccountTypeId != 4)
                {
                    var userTransaction = new UserTransactionInfo()
                    {
                        accountTypeId = item.AccountTypeId,
                        creditUserId = user.userId,
                        isActive = true,
                        isDeleted = false,
                        programId = item.ProgramId,
                        transactionAmount = item.InitialBalance,
                        transactionDate = DateTime.UtcNow,
                        CreditTransactionUserType = TransactionUserEnityType.User,
                        DebitTransactionUserType = TransactionUserEnityType.Admin,
                        programAccountId = item.ProgramAccountId,
                        createdBy = model.createdBy,
                        modifiedBy = model.modifiedBy,
                        createdDate = DateTime.UtcNow,
                        modifiedDate = DateTime.UtcNow,
                        organisationId = organisationDetail != null ? organisationDetail.id : 0,
                        planId = item.PlanId,
                        debitUserId = model.createdBy.Value,
                        transactionStatus = 0
                    };
                    userTransactionList.Add(userTransaction);
                }
            }
        }

        /// <summary>
        /// This Api is used to get the plan detail info with all accounts under the program
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetPlanInfoWithProgAcc")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PlanDetailInfoWithProgAcc(int planId, int programId)
        {
            try
            {
                /* Get plan by Id With ProgramAccount. */
                var planDetailsWithProgAcc = await _planService.GetPlanInfoWithProgramAccount(planId, programId);
                if (planId == 0)
                {
                    planDetailsWithProgAcc.planId = string.Concat("P1000-", Convert.ToString(await _planService.GetPrimaryMaxAsync() + 1));
                }
                if (planDetailsWithProgAcc == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planDetailsWithProgAcc));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetPlanInfoWithProgAcc)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }
        #endregion

        #region Program Account
        /// <summary>
        /// This Api is used to get account listing under program.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("AccountListing")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAccountListing(int programId)
        {
            try
            {
                IEnumerable<AccountListingDto> planForGetAccountListing = await _programAccountService.GetAccountListing(programId);
                if (planForGetAccountListing == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planForGetAccountListing, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AccountListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [Route("AccountListingOnPrgNPlan")]
        [HttpGet]
        public async Task<IActionResult> GetAccountListingBasedOnPlanNProgram(int programId, int planId)
        {
            try
            {
                IEnumerable<AccountListingDto> planNProgram = await _programAccountService.GetAccountBasedOnPlanNProgramSelection(programId, planId);
                if (planNProgram == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planNProgram, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AccountListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is used to get account listing under program.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("AccountListingForReports")]
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAccountListingForReports(int programId)
        {
            try
            {
                IEnumerable<AccountListingDto> planForReports = await _programAccountService.GetAccountListing(programId);
                if (planForReports == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planForReports, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AccountListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is update program account status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateProgramAccountStatus")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProgramAccountStatus(AccountListingDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get all the relations to return. */
                var updateStatus = await _programAccountService.UpdateProgramAccountStatus(model.Id, model.Status);
                if (updateStatus <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ProgramAccountStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ProgramAccountAddUpdatedSuccessfully, updateStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := UpdateProgramAccountStatus)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used delete program account.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteProgramAccount")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteProgramAccount(AccountListingDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var DeleteAccountDetail = await _programAccountService.DeleteAccountById(model.Id);
                if (DeleteAccountDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.AccountNotDeletedSuccessfully));
                }
                if (!string.IsNullOrEmpty(model.Jpos_ProgramAccountId))
                {
                    var oAccountJPOS = new AccountsJposDto()
                    {
                        active = false
                    };
                    await _sharedJposService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.Accounts, oAccountJPOS, model.Jpos_ProgramAccountId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.Accounts);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AccountDeletedSuccessfully, DeleteAccountDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := DeleteProgramAccount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to create program account clone.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CloneAccount")]
        [HttpPost]


        [Authorize]
        public async Task<IActionResult> CreateProgramAccountClone(AccountListingDto model)
        {
            try
            {
                var result = 0;
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var accDetails = await _programAccountService.GetDataByIdAsync(new { id = model.Id });
                result = await _programAccountService.AddAsync(accDetails);
                if (result > 0)
                {
                    var chkExist = await _programAccountService.FindAsync(new { id = result });
                    chkExist.ProgramAccountId = "PA1000-" + result.ToString();
                    chkExist.accountName = chkExist.accountName + "_Copy";
                    await _programAccountService.UpdateAsync(chkExist, new { id = result });
                    var planprogramaccountlinking = await _planProgramAccountLinkingService.GetDataAsync(new { programAccountId = model.Id });
                    if (planprogramaccountlinking.Any())
                    {
                        foreach (var item in planprogramaccountlinking)
                        {
                            PlanProgramAccountsLinking objppal = new PlanProgramAccountsLinking();
                            objppal.planId = item.planId;
                            objppal.programAccountId = result;
                            await _planProgramAccountLinkingService.AddAsync(objppal);
                        }
                    }
                }

                if (result <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AccountNotClonedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.AccountClonedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := CreateProgramAccountClone)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used to get program account detail info with all account type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetProgramAccountInfoWithAccountType")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ProgramAccountDetailInfoWithAccountType(int id)
        {
            try
            {
                var accDetails = await _programAccountService.GetProgramAccountInfoWithAccountType(id);
                if (accDetails == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                var weekDays = _mapper.Map<List<WeekDayDto>>((await _programAccountService.GetWeekDaysList()).ToList());
                if (weekDays.Count > 0)
                    accDetails.WeekDays = weekDays;
                var passType = _mapper.Map<List<PassTypeDto>>((await _programAccountService.GetPassTypeList()).ToList());
                if (passType.Count > 0)
                    accDetails.lstPassType = passType;
                var resetPeriod = _mapper.Map<List<ResetPeriodDto>>((await _programAccountService.GetResetPeriodList()).ToList());
                if (resetPeriod.Count > 0)
                    accDetails.ResetPeriod = resetPeriod;
                var exResetPeriod = _mapper.Map<List<ExchangeResetPeriodDto>>((await _programAccountService.ExchangeResetPeriodList()).ToList());
                if (exResetPeriod.Count > 0)
                    accDetails.ExchangeResetPeriod = exResetPeriod;
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, accDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := ProgramAccountDetailInfoWithAccountType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to add/update program account.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreateModifyProgramAccount")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateProgramAccount(ProgramAccountViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var result = await _programAccountService.AddEditProgramAccoutDetails(model, clientIpAddress);
                if (result == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PlanStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.PlanAddUpdatedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AddUpdateProgramAccount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }
        #endregion

        #region Branding
        /// <summary>
        /// This Api is used to get brandings list under the programs.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("BrandingListing")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBrandingListing(int programId)
        {
            try
            {
                IEnumerable<BrandingListingDto> planBrandingListing = await _programBrandingService.GetBrandingListing(programId);
                if (planBrandingListing == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planBrandingListing, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetBrandingListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to delete branding.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteBranding")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteBranding(BrandingListingDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var DeleteBrandDetail = await _programBrandingService.DeleteBrandById(model.Id);
                if (DeleteBrandDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.BrandNotDeletedSuccessfully));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.BrandDeletedSuccessfully, DeleteBrandDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := DeleteBranding)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get branding info with account type.
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetBrandingInfoWithAccountType")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BrandingDetailInfoWithAccountType(int brandId, int programId)
        {
            try
            {
                var planDetailsWithAccountType = await _programBrandingService.GetBrandingInfoWithAccountType(brandId, programId);
                if (planDetailsWithAccountType == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, planDetailsWithAccountType));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := BrandingDetailInfoWithAccountType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to add/update branding.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreateModifyBranding")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateBranding(ProgramBrandingViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var cardCheck = await _programBrandingService.CheckCardExistence(model.cardNumber);
                if (cardCheck != null)
                {
                    if (model.id > 0)
                    {
                        if (!cardCheck.cardNumber.Trim().Equals(model.cardNumber, StringComparison.OrdinalIgnoreCase))
                            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.CardExists, "0", 1)); // 404
                    }
                    else
                    {
                        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.CardExists, "0", 1)); // 404
                    }
                }

                var accountBrandingExistCheck = await _programAccountService.GetAccountDetailNCheckForBranding(model.programAccountID);
                if (accountBrandingExistCheck != null && accountBrandingExistCheck.BrandingCount > 0 && model.id <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.BrandingAlreadyExist, "0", 1)); // 404
                }
                var result = await _programBrandingService.AddEditBrandingDetails(model);
                if (result == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.BrandStatusNotUpdatedSuccessfully, "0", 1)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.BrandAddUpdatedSuccessfully, result, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AddUpdateBranding)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used to get program account type by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetProgramAccountTypeById")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProgramAccountTypeById(int id)
        {
            try
            {
                var accounttypeid = (await _programAccountService.GetDataByIdAsync(new { id })).accountTypeId;
                var accounttype = (await _accountType.GetDataByIdAsync(new { id = accounttypeid }));
                if (accounttype == null)
                {
                    return Ok(NoProgramsExist);
                }
                var branding = await _programBrandingService.GetBrandingInfoOnAccountSelection(id);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, branding));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramAccountTypeById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get program account type by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("CheckBrandingExistForAccount")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckBrandingExistForAccount(int id)
        {
            try
            {
                var accounttype = (await _programAccountService.GetAccountDetailNCheckForBranding(id));
                if (accounttype == null)
                {
                    return Ok(NoProgramsExist);
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, accounttype));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := CheckBrandingExistForAccount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }
        #endregion

        #region AccountMerchantRule

        /// <summary>
        /// This Api is get business type and merchant listing.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="businessTypeId"></param>
        /// <param name="accountTypeId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [Route("GetBusinessTypeAndMerchantListing")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> BusinessTypeAndMerchantListing(int programId, string businessTypeId, string accountTypeId, int accountId)
        {
            try
            {
                AccountMerchantRuleDto accountMerchantRule = await _accMerchantRule.GetAccountMerchantRule(programId, businessTypeId, accountTypeId, accountId);
                if (accountMerchantRule == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoPlanExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, accountMerchantRule, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetBusinessTypeAndMerchantListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to add/update account merchant rules.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreateModifyAccountMerchantRules")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateAccountMerchantRules(AccountMerchantRuleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var resultForAccountMerchantRules = await _accMerchantRule.AddEditAccountMerchantRules(model);
                if (resultForAccountMerchantRules == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.RuleStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.RuleAddUpdatedSuccessfully, resultForAccountMerchantRules));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AddUpdateAccountMerchantRules)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }

        /// <summary>
        /// This Api is used to update account merchant rules detail.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ModifyAccountMerchantRuleDetails")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateAccountMerchantRuleDeatils(List<AccountMerchantRuleAndDetailViewModel> model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var resultForMerchantRuleDeatils = await _accMerchantRule.EditAccountMerchantRuleDetails(model);
                if (resultForMerchantRuleDeatils == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.RuleStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.RuleAddUpdatedSuccessfully, resultForMerchantRuleDeatils));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := UpdateAccountMerchantRuleDeatils)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(someIssueInProcessing);
            }

        }
        #endregion

        #region Branding List for Mobile
        /// <summary>
        /// This Api is used to get branding listing for mobile.
        /// </summary>
        /// <returns></returns>
        [Route("MobBrandingListing")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBrandingListingMobile()
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok(noSessionMatchExist);
                }
                List<ProgramBrandingApiModel> prgBranding = _mapper.Map<List<ProgramBrandingApiModel>>(await _programBrandingService.GetBrandingsForMobile(programIdClaim, 4, userIdClaim));
                if (prgBranding.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoBrandingExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prgBranding));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetBrandingListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }
        #endregion

        #region Cardholder Agreement

        /// <summary>
        /// This Api is used to get all cardholder agreemens along with all previous history
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("CardholderAgreements")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreement(int programId)
        {
            try
            {
                var cardHolderAgreements = (await _cardHolderAgreementService.GetCardHolderAgreements(programId)).ToList();
                if (cardHolderAgreements.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new List<CardholderAgreementDto>()));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, cardHolderAgreements));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreement)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is used to get all cardholder agreemens along with all previous history
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="cardholderAgreementId"></param>
        /// <returns></returns>
        [Route("CardholderAgreementById")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementById(int programId, int cardholderAgreementId)
        {
            try
            {
                var cardHolderAgreementsById = (await _cardHolderAgreementService.GetCardHolderAgreementByIdNProgram(programId, cardholderAgreementId));
                if (cardHolderAgreementsById == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new CardholderAgreementDto()));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, cardHolderAgreementsById));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreement)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is used to get all cardholder agreemens along with all previous history
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="cardholderAgreementId"></param>
        /// <returns></returns>
        [Route("CardholderAgreementByIdUnauth")]
        [HttpGet]
        public async Task<IActionResult> CardholderAgreementByIdUnauth(int programId, int cardholderAgreementId)
        {
            try
            {
                var cardHolderAgreementsByIdUnauth = (await _cardHolderAgreementService.GetCardHolderAgreementByIdNProgram(programId, cardholderAgreementId));
                if (cardHolderAgreementsByIdUnauth == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new CardholderAgreementDto()));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, cardHolderAgreementsByIdUnauth));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreement)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This Api is used to check cardholder agreements existence
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("CardHolderAgreementsExistence")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementsExistence(int programId)
        {
            try
            {
                var cardHolderAgreementsExistence = (await _cardHolderAgreementService.GetCardHolderAgreementsExistence(programId));
                if (cardHolderAgreementsExistence == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new CardholderAgreementDto()));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, cardHolderAgreementsExistence));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreement)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This method is used to post Cardholder Agreement by Client.
        /// </summary>
        /// <param name="oCardHolderAgreement"></param>
        /// <returns></returns>
        [Route("AddCardHolderAgreement")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostCardHolderAgreement(CardholderAgreementDto oCardHolderAgreement)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var cardHolderAgreementModel = new CardHolderAgreement()
                {
                    id = oCardHolderAgreement.CardHolderAgreementId,
                    cardHoldrAgreementContent = oCardHolderAgreement.cardHoldrAgreementContent,
                    createdBy = userIdClaim,
                    createdDate = DateTime.UtcNow,
                    modifiedBy = userIdClaim,
                    modifiedDate = DateTime.UtcNow,
                    programID = oCardHolderAgreement.ProgramId,
                    versionNo = oCardHolderAgreement.versionNo

                };
                var cardHolderAgreements = (await _cardHolderAgreementService.AddUpdateCardHolderAgreement(cardHolderAgreementModel));
                if (cardHolderAgreements == null) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.CardHolderAgreementNotPostSuccessfully, "")); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.CardHolderAgreementPostSuccessfully, cardHolderAgreements));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreement)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }
        #endregion

        /// <summary>
        /// Gets the data
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("CardholderAgreementsUserHistory")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementHistory(int programId)
        {
            try
            {
                var cardHolderAgreementsHistory = (await _userAgreementHistory.GetCardHolderAgreementHistory(programId)).ToList();
                if (cardHolderAgreementsHistory.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new List<UserAgreementHistoryDto>()));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, cardHolderAgreementsHistory));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreementHistory)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// Gets the data
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="cardholderAgreementId"></param>
        /// <returns></returns>
        [Route("UserAgreementHistoryVersions")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementHistoryVersions(int programId, int cardholderAgreementId)
        {
            try
            {
                var userAgreementsHistoryVersions = (await _userAgreementHistory.GetCardHolderAgreementHistoryVersions(programId, cardholderAgreementId)).ToList();
                if (userAgreementsHistoryVersions.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new List<UserAgreementHistoryDto>()));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userAgreementsHistoryVersions));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetCardHolderAgreementHistory)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("ProgramAccountDropdownForUser")]
        [HttpGet]
        public async Task<IActionResult> GetProgramAccountDropdownForUser(int userId)
        {
            try
            {
                IEnumerable<AccountListingDto> programAccounts = await _programAccountService.GetProgramAccountsDropdownForUser(userId);
                if (programAccounts == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoProgramAccountExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, programAccounts, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := AccountListing)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }
    }
}

