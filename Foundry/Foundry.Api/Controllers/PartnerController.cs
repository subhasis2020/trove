using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using ElmahCore;
using foundry;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Foundry.Services.AcquirerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Foundry.Domain.Constants;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using IdentityModel.Client;
using System.Net.Http;
using Foundry.Identity;
using Foundry.Domain.ApiModel.PartnerApiModel;
using Foundry.Services.PartnerNotificationsLogs;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Data;

namespace Foundry.Api.Controllers
{
    
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PartnerController : ControllerBase
    {


        private readonly IConfiguration _configuration;
        private readonly II2CLogService _i2cLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganisation _organisation;
        private readonly IPrograms _program;
        private readonly ICardHolderAgreementService _cardHolderAgreementService;
        private readonly IBenefactorService _benefactor;
        private readonly IGeneralSettingService _setting;
        private readonly ISMSService _sMSService;
        private readonly IEmailService _emailService;
        private readonly IInvitationService _invite;
        private readonly IMapper _mapper;
        private readonly II2CAccountDetailService _i2cAccountDetailRepository;
        private readonly IAcquirerService _acquirerService;
        private readonly II2CCardBankAccountService _i2cCardBankAccountRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly IGeneralSettingService _generalRepository;
        private readonly II2CBank2CardTransferService _i2cBank2CardTransferRepository;
        private readonly IUserTransactionInfoes _userTransactionRepository;
      //  private readonly UserManager<User> _userManager;
        private readonly CustomUserManager _userManager;
        private readonly IPlanProgramAccountLinkingService _planProgramAccountLinkingService;
        private readonly IUsersProgram _usersProgram;
        private readonly IUserAgreementHistoryService _userAgreementHistoryService;
        private readonly IPlanService _planService;
        private readonly IUserPlanService _userPlanService;
        private readonly IUserTransactionInfoes _userTransactionInfoes;
        private readonly ITranlog _tranlog;
        private readonly IPartnerNotificationsLogServicer _partnerNotificationsLogRepository;
        private readonly ISharedJPOSService _sharedJPOSService;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IBinDataService _binDataService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="organisation"></param>
        /// <param name="program"></param>
        /// <param name="cardHolderAgreementService"></param>
        /// <param name="i2cAccountDetailRepository"></param>
        /// <param name="acquirerService"></param>
        /// <param name="mapper"></param>
        /// <param name="generalRepository"></param>
        /// <param name="i2cLogRepository"></param>
        /// <param name="configuration"></param>
        /// <param name="userTransactionRepository"></param>
        /// <param name="userAgreementHistoryService"></param>

        public PartnerController(IUserRepository userRepository, IOrganisation organisation, IPrograms program,
             ICardHolderAgreementService cardHolderAgreementService
            , IBenefactorService vbenefactor
            , IGeneralSettingService vsetting
            , IConfiguration vconfiguration
            , ISMSService vsMSService
            , IEmailService vemailService
            , IInvitationService vinvite
            , IMapper mapper
            , II2CAccountDetailService vi2cAccountDetailRepository
            , IAcquirerService vacquirerService
            , II2CCardBankAccountService vi2cCardBankAccountRepository
            , II2CLogService vi2cLogRepository
            , IGeneralSettingService vgeneralRepository
            , II2CBank2CardTransferService vi2cBank2CardTransferRepository
            , IUserTransactionInfoes vuserTransactionRepository
            , CustomUserManager  vuserManager
            , RoleManager<Role> roleManager
            , IPlanProgramAccountLinkingService planProgramAccountLinkingService
            , IUsersProgram usersProgram, IUserAgreementHistoryService userAgreementHistoryService
            , IPlanService planService
            , IUserTransactionInfoes userTransactionInfoes
            ,IUserPlanService userPlanService
            , ITranlog tranlog
           , IPartnerNotificationsLogServicer partnerNotificationsLogRepository,
             ISharedJPOSService sharedPJposService
            , IHostingEnvironment hostingEnvironment
            , IBinDataService binDataService
             )
        {
            _userRepository = userRepository;
            _organisation = organisation;
            _program = program;
            _cardHolderAgreementService = cardHolderAgreementService;
            _benefactor = vbenefactor;
            _setting = vsetting;
            _configuration = vconfiguration;
            _sMSService = vsMSService;
            _emailService = vemailService;
            _invite = vinvite;
            _mapper = mapper;
            _userManager = vuserManager;
            _userRepository = userRepository;
            _mapper = mapper;
            _i2cAccountDetailRepository = vi2cAccountDetailRepository;
            _acquirerService = vacquirerService;
            _i2cCardBankAccountRepository = vi2cCardBankAccountRepository;
            _i2cLogRepository = vi2cLogRepository;
            _generalRepository = vgeneralRepository;
            _i2cBank2CardTransferRepository = vi2cBank2CardTransferRepository;
            _userTransactionRepository = vuserTransactionRepository;
            _roleManager = roleManager;
            _planProgramAccountLinkingService = planProgramAccountLinkingService;
            _userAgreementHistoryService = userAgreementHistoryService;
            _planService= planService;
            _userTransactionInfoes = userTransactionInfoes;
            _usersProgram = usersProgram;
            _userPlanService = userPlanService;
            _tranlog = tranlog;
            _partnerNotificationsLogRepository = partnerNotificationsLogRepository;
            _sharedJPOSService = sharedPJposService;
            _hostingEnvironment= hostingEnvironment;
            _binDataService = binDataService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(template: "RegisterAccountHolder")]
        public async Task<IActionResult> RegisterAccountHolder(RegisterAccountHolderSodexoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse(StatusCodes.Status400BadRequest, true, MessagesConstants.InvalidInputData, ModelState.Values.SelectMany(x => x.Errors).ToList()));
                }
                var chkUserByEmail = await _userRepository.CheckUserByEmail(model.PrimaryEmail);
              //  var chkUserByPhone = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
               if (chkUserByEmail != null )
               {
                   return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, true, MessagesConstants.EmailExists));
               }

                var chkUserBypartneid = await _userRepository.CheckUserByPartnerId(model.PartnerUserId,model.PartnerId);
                //  var chkUserByPhone = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
                if (chkUserBypartneid != null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, true, MessagesConstants.PartnerUserIdExists));
                }


                string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var issuerId = (await _program.FindAsync(new { ProgramCodeId = model.ProgramIssuerId }));
                if(issuerId ==null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, true, MessagesConstants.InvalidProgramIssuerId));
                }

                var plan = (await _planService.GetPlanListing(issuerId.id)).ToList();
                
                var userModel = new User
                {

                    dateOfBirth = model.DateOfBirth,
                    FirstName = model.FirstName,
                    genderId = model.GenderId,
                    LastName = model.LastName,
                    PasswordHash = _configuration["BiteUserDefultPassword"], //model.PasswordHash,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.PrimaryEmail,
                    ProgramId = issuerId.id,
                    OrganisationId = Convert.ToInt32( _configuration["SodexhoOrgId"]),
                    UserDeviceId = model.DeviceId,
                    UserDeviceType = model.DeviceType,
                    PartnerUserId = model.PartnerUserId,
                    PartnerId = model.PartnerId,
                    UserCode= string.Concat("AHD1000-", Convert.ToString(await _userRepository.GetPrimaryMaxAsync() + 1))

            };

                
                List<UserPlans> planIdsList = new List<UserPlans>();
                foreach (var item in plan)
                {
                    planIdsList.Add(new UserPlans() { programPackageId = item.Id });
                }

                var result = await _userRepository.RegisterPartnerUser(userModel);
                if (result > 0 )
                {
                    if (planIdsList.Count > 0)
                    {
                        planIdsList.ToList().ForEach(x => x.userId = result);
                        await _userPlanService.AddUpdateUserPlans(planIdsList);
                    }

                    await RegisterAccountHolderRefactor(model, userModel, planIdsList, result);
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AccountholderAddedSuccessfully, Cryptography.EncryptPlainToCipher(result.ToString()), 1));
                }
               // return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AccountholderAddedSuccessfully, Cryptography.EncryptPlainToCipher(result.ToString()), 1));


                return StatusCode(StatusCodes.Status200OK, MessagesConstants.UserSuccessfulRegistration);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Partner := RegisterAccountHolder)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(StatusCodes.Status500InternalServerError, false, MessagesConstants.SomeIssueInProcessing));
            }

        }


        private async Task RegisterAccountHolderRefactor(RegisterAccountHolderSodexoModel model, User userModel, List<UserPlans> planIdsList, int result)
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
                        // createdBy = model.CreatedBy,
                        // modifiedBy = model.ModifiedBy,
                        createdDate = DateTime.UtcNow,
                        modifiedDate = DateTime.UtcNow,
                        organisationId = Convert.ToInt32( _configuration["BiteUserDefultPassword"]),
                        planId = item.PlanId,
                        //debitUserId = model.CreatedBy,
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

            await _usersProgram.AddUserInProgram(result, (int)userModel.ProgramId);

 
        }






        #region CardHolder Agreement
        /// <summary>
        /// This Api is used to check cardholder agreement existence
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route(template: "CardHolderAgreement")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementExistence()
        {
            try
            {
                var programIdClaimUpdateUser = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var cardHolderAgreementsExistence = (await _cardHolderAgreementService.GetCardHolderAgreementsExistence(programIdClaimUpdateUser));
                if (cardHolderAgreementsExistence == null)
                {
                    return StatusCode(StatusCodes.Status204NoContent, new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoCardholderAgreementExist, new CardholderAgreementDto()));
                }
                return StatusCode(StatusCodes.Status200OK, new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, cardHolderAgreementsExistence));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Sodexo := GetCardHolderAgreementsExistence)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(StatusCodes.Status500InternalServerError, false, MessagesConstants.SomeIssueInProcessing));
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
                    return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse(StatusCodes.Status400BadRequest, true, MessagesConstants.InvalidInputData, ModelState.Values.SelectMany(x => x.Errors).ToList()));

                }
              
                var identityUpdateUser = User.Identity as ClaimsIdentity;
                var userIdClaimUpdateUser = Convert.ToInt32(identityUpdateUser.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                // var sessionIdClaimUpdateUser = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimUpdateUser = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Checks the session of the user against its Id. */
                //if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimUpdateUser, sessionIdClaimUpdateUser)))
                //{
                //    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                //}
                model.Id = userIdClaimUpdateUser;
                var user = new User()
                {
                    Id = model.Id,
                    AgreementVersionNo = model.AgreementVersionNo,
                    IsAgreementRead = model.IsAgreementRead,
                    
                   
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
                //  await Test(cardmodel, programIdClaimUpdateUser);
                //start
                var detail = await _userRepository.GetUserInfoById(model.Id);
                var cardmodel = new I2CAddCardModel()
                {
                    FirstName = detail.FirstName,
                    LastName = detail.LastName,
                    Email = detail.Email,
                    AccountHolderUniqueId=detail.UserCode,
                    InitialAmount=model.InitialAmount,
                    IPAddress=model.IPAddress
                };
                List<string> userCodeList = new List<string> { cardmodel.AccountHolderUniqueId };

                var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { AccountHolderUniqueId = cardmodel.AccountHolderUniqueId });
                if (userVirtualCardDetail != null && userVirtualCardDetail.Id > 0)
                {
                    await _userAgreementHistoryService.AddAsync(userAgreementHistory);
                    return StatusCode(StatusCodes.Status200OK, new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserCardholderAgreementSettingsUpdated, userAgreement));

                }

                var UsersAccountDetail = await _userRepository.GetUsersDetailByUserCode(userCodeList);
              if (UsersAccountDetail.Any())
                {
                    List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();

                    AddCardType addCardType = new AddCardType()
                    {
                        Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                        Card = new NewCardType1()
                        {
                            StartingNumbers = i2cSettings[3].Value,
                            IPAddress = cardmodel.IPAddress,
                            CardProgramID = i2cSettings[4].Value
                        },
                        Profile = new ProfileType()
                        {
                            FirstName = cardmodel.FirstName,
                            LastName = cardmodel.LastName,
                            Email = cardmodel.Email,
                            NameOnCard = cardmodel.FirstName + " " + cardmodel.LastName,
                            Address = cardmodel.Address,
                            City = cardmodel.City,
                            StateCode = cardmodel.StateCode,
                            PostalCode = cardmodel.PostalCode,
                            Country = cardmodel.Country
                        },
                        InitialAmountSpecified = true,
                        IsNewCardPreActive = YN.N,
                        InitialLoadFlag = InitialLoadFlagType.G,
                        InitialAmount = cardmodel.InitialAmount,
                    };
                    var userObj = UsersAccountDetail.FirstOrDefault();
                    XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(addCardType), "Request");
                    I2CLog objRequestLog = await i2cLogRequest(userObj.Id, cardmodel.AccountHolderUniqueId, "AddCard", _configuration["ServiceAPIURL"] + "I2CAccount/AddCard", cardmodel.IPAddress, request);
                    if (objRequestLog.Id > 0)
                    {
                        await _userAgreementHistoryService.AddAsync(userAgreementHistory);
                        return await AddCardRefactor(cardmodel, programIdClaimUpdateUser, i2cSettings, addCardType, userObj, objRequestLog);
                    }
                }


                    //ends


                    await _userAgreementHistoryService.AddAsync(userAgreementHistory);
                return StatusCode(StatusCodes.Status200OK, new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserCardholderAgreementSettingsUpdated, userAgreement));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Sodexo := UpdateUserCardholderAgreementInfo)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
               return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(StatusCodes.Status500InternalServerError, false, MessagesConstants.SomeIssueInProcessing));

            }
        }
        #endregion

      



        #region  Add Card

        /// <summary>
        /// This Api is called to Create Card on I2C.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model">I2CAddCardModel</param>
        /// <returns>Token</returns>
         [Route("AddCard")]
          [HttpPost]
         [Authorize]
        public async Task<IActionResult> AddCard(I2CAddCardModel model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var programIdClaim = Convert.ToInt32(identity.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "programId".ToLower().Trim()).Value);

                if (!ModelState.IsValid)
                {
                    return Ok();
                }
                List<string> userCodeList = new List<string> { model.AccountHolderUniqueId };

                var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { AccountHolderUniqueId = model.AccountHolderUniqueId });
                if (userVirtualCardDetail != null && userVirtualCardDetail.Id > 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.VirtualCardAlreadyExists));
                }
                var UsersAccountDetail = await _userRepository.GetUsersDetailByUserCode(userCodeList);
                if (UsersAccountDetail.Any())
                {
                    List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();

                    AddCardType addCardType = new AddCardType()
                    {
                        Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                        Card = new NewCardType1()
                        {
                            StartingNumbers = i2cSettings[3].Value,
                            IPAddress = model.IPAddress,
                            CardProgramID = i2cSettings[4].Value
                        },
                        Profile = new ProfileType()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            NameOnCard = model.FirstName + " " + model.LastName,
                            Address = model.Address,
                            City = model.City,
                            StateCode = model.StateCode,
                            PostalCode = model.PostalCode,
                            Country = model.Country
                        },
                        InitialAmountSpecified = true,
                        IsNewCardPreActive = YN.N,
                        InitialLoadFlag = InitialLoadFlagType.G,
                        InitialAmount = model.InitialAmount,
                    };
                    var userObj = UsersAccountDetail.FirstOrDefault();
                    XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(addCardType), "Request");
                    I2CLog objRequestLog = await i2cLogRequest(userObj.Id, model.AccountHolderUniqueId, "AddCard", _configuration["ServiceAPIURL"] + "I2CAccount/AddCard", model.IPAddress, request);
                    if (objRequestLog.Id > 0)
                    {
                        return await AddCardRefactor(model, programIdClaim, i2cSettings, addCardType, userObj, objRequestLog);
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := AddCard)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        private async Task<IActionResult> AddCardRefactor(I2CAddCardModel model, int programIdClaim, List<GeneralSetting> i2cSettings, AddCardType addCardType, User userObj, I2CLog objRequestLog)
        {
            try
            {
                var cardResponse = await _acquirerService.AddCard(addCardType);
                XmlDocument response;
                if (cardResponse.AddCardResponse1 != null && cardResponse.AddCardResponse1.ResponseCode == "00")
                {
                    if (model.InitialAmount > 0)
                    {
                        /* Add initial amount in user transaction */
                        await AddUserTransaction(userObj.Id, model.InitialAmount, programIdClaim, userObj.Id);
                    }
                    response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(cardResponse), "Response");
                    objRequestLog.Status = "In-Progress";
                    int accountDetailId = await AddI2CAccountDetail(userObj, cardResponse, model.AccountHolderUniqueId);
                    /* Sandbox case */
                    if (model.IsSandBoxAccount)
                    {
                        var objActivateCard = await ActivateCard(i2cSettings, cardResponse);
                        if (objActivateCard.ActivateCardResponse1.ResponseCode == "00")
                        {
                            var obj = await _i2cAccountDetailRepository.GetDataByIdAsync(new { id = accountDetailId });
                            obj.CardStatus = "1";
                            await _i2cAccountDetailRepository.UpdateAsync(obj, new { id = accountDetailId });
                        }
                    }
                    /* Sandbox case */
                    objRequestLog.Status = "Success";
                    /* Update Log table */
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                    jposCardCollection ojposcard = new jposCardCollection()
                    {
                        bin = "494157",
                        cardProduct = "bitepay",
                        cardHolder = userObj.PartnerUserId,
                        token = cardResponse.AddCardResponse1.ReferenceID
                    };

                    int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.CardsCollection, ojposcard, "","", JPOSAPIConstants.AccountHolder);
                    await sendCardIssuedNotification(userObj , SodexoBiteNotification.SodexoBiteBaseUrl);

                    List<GeneralSetting> i2cSettings1 = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                    if (i2cSettings1[0].Value == "1")
                    {
                        await sendCardIssuedNotification(userObj, SodexoBiteNotification.SecondSodexoBiteBaseUrl);

                    }

                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.CardCreatedSuccessfully));
                }
                else
                {
                    objRequestLog.Status = "Failed";
                    /* Update Log table */
                    response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(cardResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, string.IsNullOrEmpty(cardResponse.AddCardResponse1.ResponseCode) ? MessagesConstants.CardIsNotCreated : cardResponse.AddCardResponse1.ResponseCode));
                }
            }
            catch (Exception ex)
            {
                await i2cLogResponse(objRequestLog, ex.Message);
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
            }
        }

        private async Task sendCardIssuedNotification(User user,string key)
        {
            PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
            try
            {
                using (var client = new HttpClient())
                {
                    object obj = new
                    {
                        UserObjectId = user.PartnerUserId 
        
                    };
                    List<GeneralSetting> i2cSettings = await GetGeneralSettings(key);

                    var url = i2cSettings[0].Value;
                    var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.BitePayCardIssued);

                    string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("BitePayCardIssued", hostURL.ToString(), myJSON, user.Id);
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

        private async Task<List<GeneralSetting>> GetGeneralSettings(string Key)
        {
            var settings = await _generalRepository.GetDataAsync(new { KeyName = Key });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }

       

        private async Task<activateCardResponse> ActivateCard(List<GeneralSetting> i2cSettings, addCardResponse cardResponse)
        {
            var activateCard = new ActivateCardType()
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeAddWithRefIdReqd()
                {
                    ReferenceID = cardResponse.AddCardResponse1.ReferenceID
                }
            };
            return await _acquirerService.ActivateCard(activateCard);
        }

        private async Task<int> AddI2CAccountDetail(User userObj, addCardResponse cardResponse, string accountHolderUniqueId)
        {
            I2CAccountDetail objAccounntDetail = new I2CAccountDetail
            {
                UserId = userObj.Id,
                NameOnCard = userObj.FirstName + " " + userObj.LastName,
                CustomerId = cardResponse.AddCardResponse1.CustomerId,
                //  CardNumber = (cardResponse.AddCardResponse1.NewCardNumber != null ? cardResponse.AddCardResponse1.NewCardNumber.Number : cardResponse.AddCardResponse1.BatchReferenceID),
                CardNumber = cardResponse.AddCardResponse1.ReferenceID,
                ExpiryMonth = cardResponse.AddCardResponse1.NewCardNumber.ExpiryDate.Substring(0, 2),
                ExpiryYear = cardResponse.AddCardResponse1.NewCardNumber.ExpiryDate.Substring(2, 4),
                CVV2 = !string.IsNullOrEmpty(cardResponse.AddCardResponse1.CVV2) ? Cryptography.EncryptPlainToCipher(cardResponse.AddCardResponse1.CVV2) : string.Empty,
                AccountHolderUniqueId = accountHolderUniqueId,
                Balance = Convert.ToDecimal(cardResponse.AddCardResponse1.Balance),
                CardStatus = cardResponse.AddCardResponse1.Status != null ? cardResponse.AddCardResponse1.Status.Code : string.Empty,
                ReferenceId = cardResponse.AddCardResponse1.ReferenceID
            };
            return await _i2cAccountDetailRepository.AddAsync(objAccounntDetail);
        }

        private async Task AddUserTransaction(int debitUserId, decimal amount, int programId, int creditUserId)
        {
            UserTransactionInfo userTransactionInfo = new UserTransactionInfo
            {
                debitUserId = debitUserId,
                creditUserId = creditUserId,
                accountTypeId = 3,
                transactionAmount = amount,
                periodRemark = "Initial Amount Load",
                transactionDate = DateTime.UtcNow,
                programId = programId,
                isActive = true,
                createdBy = debitUserId,
                createdDate = DateTime.UtcNow,
                modifiedBy = debitUserId,
                modifiedDate = DateTime.UtcNow,
                isDeleted = false,
                CreditTransactionUserType = debitUserId == creditUserId ? Constants.TransactionUserEnityType.BankAccountCredit
                                                                        : Constants.TransactionUserEnityType.Benefactor
            };
            await _userTransactionRepository.AddAsync(userTransactionInfo);
        }

        private async Task<I2CLog> i2cLogRequest(int userId, string accountHolderUniqueId, string apiName, string apiUrl, string ipAddress, XmlDocument request)
        {
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = userId,
                AccountHolderUniqueId = accountHolderUniqueId,
                ApiName = apiName,
                ApiUrl = apiUrl,
                IpAddress = ipAddress,
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };

            //log request in log table for i2c create bank account
            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
            return objRequestLog;
        }


        private async Task<List<GeneralSetting>> Geti2cGeneralSettings()
        {
            var settings = await _generalRepository.GetDataAsync(new { keyGroup = MessagesConstants.i2cKeyName });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }
        #endregion



        /// <summary>
        /// This Api is called to invite benefactor from mobile app.
        /// </summary>
        /// <param name="model">BenefactorRegisterModel</param>
        /// <returns></returns>
        [Route("InviteBenefactor")]
        [HttpPost]
        //[Authorize]
        // [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> InviteBenefactor(PartnerBenefactorRegisterModel inputmodel)
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

                BenefactorRegisterModel model = new BenefactorRegisterModel();

                model.FirstName = inputmodel.FirstName;
                model.LastName = inputmodel.LastName;
                model.EmailAddress = inputmodel.EmailAddress;
                model.MobileNumber = inputmodel.MobileNumber;
                model.RelationshipId = inputmodel.RelationshipId;

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
                    //    template.Body = template.Body.Replace("Trove", "Sodexo");
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




        #region Add Bank Account
        /// <summary>
        /// This Api is called to Create Card on I2C.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model">I2CCardBankAccountModel</param>
        /// <returns>Token</returns>
        [Route("AddBankAccount")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBankAccount(I2CCardBankAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                var objAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { UserId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.idRequesteeUserEnc)) });
                if (objAccountDetail != null)
                {
                    model.I2CAccountDetailId = objAccountDetail.Id;
                    CreateBankAccountType createBankAccountType = new CreateBankAccountType()
                    {
                        Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                        Account = new foundry.AccountType()
                        {
                            AccountNickname = model.AccountNickName,
                            AccountNumber = model.AccountNumber,
                            AccountTitle = model.AccountTitle,
                            ACHType = ACH_ACT.Item1,
                            BankName = model.BankName,
                            Comments = model.Comments,
                            RoutingNumber = model.RoutingNumber,
                            AccountType1 = ACT.Item11
                        },
                        Card = new CardTypeWithOptionalStatus()
                        {
                            ReferenceID = objAccountDetail.ReferenceId
                        }
                    };

                    XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(createBankAccountType), "Request");
                    I2CLog objRequestLog = await i2cLogRequest(model.Id, model.AccountHolderUniqueId, "AddBankAccount", _configuration["ServiceAPIURL"] + "I2CAccount/AddBankAccount", model.IpAddress, request);

                    if (objRequestLog.Id > 0)
                    {
                        return await AddBankAccountRefactor(model, i2cSettings, objAccountDetail, createBankAccountType, objRequestLog);
                    }
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := AddBankAccount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }




        private async Task<IActionResult> AddBankAccountRefactor(I2CCardBankAccountModel model, List<GeneralSetting> i2cSettings, I2CAccountDetail objAccountDetail, CreateBankAccountType createBankAccountType, I2CLog objRequestLog)
        {
            try
            {
                var cardResponse = await _acquirerService.CreateBankAccount(createBankAccountType);
                XmlDocument response;
                if (cardResponse.CreateBankAccountResponse1.ResponseCode == "00")
                {
                    response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(cardResponse), "Response");
                    objRequestLog.Status = "In-Progress";
                    model.Id = await Addi2cCardBankAccount(model, cardResponse);

                    /* Sandbox case */
                    if (model.IsSandBoxAccount)
                    {
                        var obj = await VerifyBankAccount(i2cSettings, objAccountDetail, cardResponse);
                        if (obj.VerifyBankAccountResponse1.ResponseCode == "00")
                        {
                            var i2cCardBankAccountObj = await _i2cCardBankAccountRepository.GetDataByIdAsync(new { id = model.Id });
                            i2cCardBankAccountObj.Status = true;
                            await _i2cCardBankAccountRepository.UpdateAsync(i2cCardBankAccountObj, new { id = model.Id });
                        }
                    }
                    /* Sandbox case */
                    objRequestLog.Status = "Success";
                    /* Update Log table */
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.BankAccountCreatedSuccessfully, 1));
                }
                else
                {
                    objRequestLog.Status = "Failed";
                    /* Update Log table */
                    response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(cardResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, string.IsNullOrEmpty(cardResponse.CreateBankAccountResponse1.ResponseCode) ? "Bank Account is not created" : cardResponse.CreateBankAccountResponse1.ResponseDesc, 0));
                }
            }
            catch (Exception ex)
            {
                objRequestLog.Response = ex.Message;
                await i2cLogResponse(objRequestLog, ex.Message);
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
            }
        }

        private async Task i2cLogResponse(I2CLog objRequestLog, string xml)
        {
            objRequestLog.Response = xml;
            objRequestLog.UpdatedDate = DateTime.UtcNow;
            await _i2cLogRepository.UpdateAsync(objRequestLog, new { id = objRequestLog.Id });
        }

        private async Task<int> Addi2cCardBankAccount(I2CCardBankAccountModel model, createBankAccountResponse cardResponse)
        {
            i2cCardBankAccount i2ccardBankAccount = new i2cCardBankAccount
            {
                UserId = model.Id,
                I2cAccountDetailId = model.I2CAccountDetailId,
                AccountNickName = model.AccountNickName,
                AccountNumber = model.AccountNumber,
                AccountTitle = model.AccountTitle,
                AccountType = model.AccountType,
                ACHType = model.ACHType,
                BankName = model.BankName,
                RoutingNumber = model.RoutingNumber,
                Comments = model.Comments,
                Status = model.Status,
                AccountSrNo = cardResponse.CreateBankAccountResponse1.AccountSrNo,
                CreatedDate = DateTime.UtcNow
            };
            return await _i2cCardBankAccountRepository.AddAsync(i2ccardBankAccount);
        }

        #endregion



        #region Verify Account

        /// <summary>
        /// This Api is called to Get i2c card bank accounts.
        /// </summary>
        /// <param name="accountSrNo"></param>
        /// <param name="bankName"></param>
        /// <param name="amountOne"></param>
        /// <param name="amountTwo"></param>
        /// <returns></returns>
        [Route("VerifyAccount")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> VerifyBankAccounts(string accountSrNo, string bankName, decimal amountOne, decimal amountTwo)
        {
            try
            {
                /* Get accountDetail for card number */
                var bankAccount = await _i2cCardBankAccountRepository.GetDataByIdAsync(new { AccountSrNo = accountSrNo });
                if (bankAccount == null || bankAccount.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoBankAccountExists));
                }
                var accountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { Id = bankAccount.I2cAccountDetailId });
                if (accountDetail == null || accountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                VerifyBankAccountType verifyBankAccountType = new VerifyBankAccountType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Card = new CardTypeWithOptionalStatus
                    {
                        ReferenceID = accountDetail.ReferenceId
                    },
                    Account = new AccountTypeWithVerifyAmounts()
                    {
                        AccountSrNo = accountSrNo,
                        VerifyAmountOne = amountOne,
                        VerifyAmountTwo = amountTwo
                    },
                    IsForcedPost = YN.N
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(verifyBankAccountType), "Request");
                I2CLog objRequestLog = await i2cLogRequest(accountDetail.UserId.Value, string.Empty, "VerifyAccount", _configuration["ServiceAPIURL"] + "I2CAccount/VerifyAccount", string.Empty, request);

                /* Call i2c api verify Account */
                var verifyBankAccount = await _acquirerService.VerifyBankAccount(verifyBankAccountType);
                if (verifyBankAccount.VerifyBankAccountResponse1.ResponseCode == "00")
                {
                    /* Update bank account status */
                    var bankAccounts = await _i2cCardBankAccountRepository.GetDataByIdAsync(new { Bankname = bankName, AccountSrNo = accountSrNo, Status = false });
                    if (bankAccounts == null || bankAccounts.Id <= 0)
                    {
                        return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoBankAccountExists));
                    }
                    bankAccounts.Status = true;
                    await _i2cCardBankAccountRepository.UpdateAsync(bankAccounts, new { Id = bankAccounts.Id });


                    objRequestLog.Status = "Success";
                    /* Update Log table */
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(verifyBankAccount.VerifyBankAccountResponse1), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.i2cAccountVerified, null, 1));
                }
                else
                {
                    objRequestLog.Status = "Failed";
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(verifyBankAccount.VerifyBankAccountResponse1), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, verifyBankAccount.VerifyBankAccountResponse1.ResponseDesc));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := VerifyBankAccounts)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }



        /// <summary>
        /// This method is used to verify bank account details from I2C.
        /// </summary>
        /// <param name="i2cSettings"></param>
        /// <param name="objAccountDetail"></param>
        /// <param name="cardResponse"></param>
        /// <returns></returns>
        private async Task<verifyBankAccountResponse> VerifyBankAccount(List<GeneralSetting> i2cSettings, I2CAccountDetail objAccountDetail, createBankAccountResponse cardResponse)
        {
            VerifyBankAccountType verifyBankAccountType = new VerifyBankAccountType()
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeWithOptionalStatus()
                {
                    ReferenceID = objAccountDetail.ReferenceId
                },
                Account = new AccountTypeWithVerifyAmounts()
                {
                    AccountSrNo = cardResponse.CreateBankAccountResponse1.AccountSrNo,
                    VerifyAmountOne = 0.5m,
                    VerifyAmountTwo = 0.5m
                }
            };
            return await _acquirerService.VerifyBankAccount(verifyBankAccountType);
        }

        #endregion




        #region Bank To Card Transfer

        /// <summary>
        /// This Api is called to Transefer amount from bank to card
        /// </summary>
        /// <param name="creditUserId"></param>
        /// <param name="debitUserId"></param>
        /// <param name="accountSrNum"></param>
        /// <param name="amount"></param>
        /// <param name="programId"></param>
        /// <param name="transferEndDate"></param>
        /// <returns></returns>
        [Route("Bank2CardTransfer")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Bank2CardTransfer(int creditUserId, int debitUserId, string accountSrNum, decimal amount, int programId, DateTime? transferEndDate = null)
        {
            try
            {
                var i2cCardBankAccount = await _i2cCardBankAccountRepository.GetDataByIdAsync(new { AccountSrNo = accountSrNum });
                if (i2cCardBankAccount.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoBankAccountExists));
                }
                var i2cAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { id = i2cCardBankAccount.I2cAccountDetailId });
                if (i2cAccountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                B2CTransferType b2CTransferType = new B2CTransferType
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Card = new CardTypeWithOptionalStatus
                    {
                        Number = i2cAccountDetail.CardNumber
                    },
                    TransferDetail = new TransferDetailType
                    {
                        AccountSrNo = i2cCardBankAccount.AccountSrNo,
                        Amount = amount,
                        TransferFrequency = TransferFrequencyType.O,
                        AmountSpecified = true
                    },
                    BankAccountNumber = i2cCardBankAccount.AccountNumber,
                    AccountTitle = i2cCardBankAccount.AccountTitle,
                    BankAccountType = i2cCardBankAccount.AccountType,
                    BankName = i2cCardBankAccount.BankName,
                    BankRountingNumber = i2cCardBankAccount.RoutingNumber
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2CTransferType), "Request");
                I2CLog objRequestLog = await i2cLogRequest(i2cAccountDetail.UserId.Value, i2cAccountDetail.AccountHolderUniqueId, "BankToCardTransfer", _configuration["ServiceAPIURL"] + "I2CAccount/BankToCardTransfer", string.Empty, request);

                if (objRequestLog.Id > 0)
                {
                    /* i2c api call */
                    var b2cResponse = await _acquirerService.BankToCardTransfer(b2CTransferType);
                    XmlDocument response;
                    if (b2cResponse.B2CTransferResponse1.ResponseCode == "00")
                    {
                        /*Add  User Transaction */
                        await AddUserTransaction(debitUserId, amount, programId, creditUserId);
                        /* Update balance in card */
                        i2cAccountDetail.Balance += amount;
                        await _i2cAccountDetailRepository.UpdateAsync(i2cAccountDetail, new { Id = i2cAccountDetail.Id });
                        /* payment initiation request in db */
                        await AddBank2CardTransferRecord(amount, i2cCardBankAccount, i2cAccountDetail, b2cResponse);
                        response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2cResponse), "Response");
                        objRequestLog.Status = "Success";
                        await i2cLogResponse(objRequestLog, response.InnerXml);
                        return Ok(new ApiResponse(StatusCodes.Status200OK, true, b2cResponse.B2CTransferResponse1.ResponseDesc, null, 1));
                    }
                    else
                    {
                        response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2cResponse), "Response");
                        objRequestLog.Status = "Failed";
                        await i2cLogResponse(objRequestLog, response.InnerXml);
                        return Ok(new ApiResponse(StatusCodes.Status200OK, false, b2cResponse.B2CTransferResponse1.ResponseDesc));
                    }
                }
                else
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoErrorMessagesExist));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := BankToCardTransfer)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        private async Task AddBank2CardTransferRecord(decimal amount, i2cCardBankAccount i2cCardBankAccount, I2CAccountDetail i2cAccountDetail, b2CTransferResponse b2cResponse)
        {
            i2cBank2CardTransfer i2cB2CTransfer = new i2cBank2CardTransfer
            {
                UserId = i2cAccountDetail.UserId,
                BankAccountId = i2cCardBankAccount.Id,
                CardNumber = i2cAccountDetail.CardNumber,
                TransferId = b2cResponse.B2CTransferResponse1.TransferId,
                Amount = amount,
                TransId = b2cResponse.B2CTransferResponse1.TransId,
                Status = Convert.ToInt16(Domain.Enums.i2cEnum.Initiated),
                TransferDate = DateTime.UtcNow,
                TransferEndDate = DateTime.UtcNow,
                CreatedBy = i2cAccountDetail.UserId.Value,
                UpdatedBy = i2cAccountDetail.UserId.Value,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow

            };
            await _i2cBank2CardTransferRepository.AddAsync(i2cB2CTransfer);
        }
        #endregion

        //[HttpGet("EnryptString/{strEncrypted}")]
        //public async Task<string> EnryptString(string strEncrypted)
        //{
        //    //byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
        //    string encrypted = Cryptography.EncryptPlainToCipher(strEncrypted);
        //    return encrypted;

        //}


        [HttpGet("Authorization")]
        public async Task<IActionResult> Authorisation(string PartnerUserId, string PartnerId)
        {
            // string decrypted = Cryptography.DecryptCipherToPlain(PartnerUserId);
            //byte[] b = Convert.FromBase64String(id);
            //string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            try
            {
                var User = await _userRepository.GetUserbyBiteUserId(PartnerUserId, int.Parse(PartnerId));

                //  var User = await _userManager.FindByIdAsync(decrypted.ToString());
                if (User == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, true, MessagesConstants.BiteUserNotExist));
                }
                var client = new HttpClient();
                TokenResponse tokenResponse = await GetAuthToken(User.UserName, client);

                if (tokenResponse.IsError)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, true, MessagesConstants.UnabletoAuthorize));
                }

                return Ok(new
                {
                    access_token = tokenResponse.AccessToken,
                    expires_in = tokenResponse.ExpiresIn,
                    token_type = tokenResponse.TokenType,
                    refresh_token = tokenResponse.RefreshToken

                });
            }
            catch(Exception ex)
            {
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, true, ex.Message));
            }
        }

        private async Task<TokenResponse> GetAuthToken(string emailAddress, HttpClient client)
        {
            return await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = _configuration["ServiceAPIURL"] + ApiConstants.GenerateUserToken,

                ClientId = "ro.angular",
                ClientSecret = "secret",
                Scope = "openid offline_access",
                GrantType = "password",
                UserName = emailAddress.Trim(),
                Password = _configuration["BiteUserDefultPassword"]
            });
        }










        /// <summary>
        /// This Api is called to get all the connections(benefactor) of the user irrespective of added and invited.
        /// </summary>
        /// <param name="model">SessionCheckModel</param>
        /// <returns></returns>
        [Route("UserBenefactor")]
        [HttpGet]
       // [Authorize]
        public async Task<IActionResult> GetUserConnections()
        {
            try
            {
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
                var connection = new List<DisplayBenefactorModel>();
                foreach(var itam in connectionsForUserConnections)
                {
                    DisplayBenefactorModel ben = new DisplayBenefactorModel();
                    ben.BenefactorUserId = itam.BenefactorUserId;
                    ben.FirstName = itam.FirstName;
                    ben.LastName = itam.LastName;
                    ben.RelationshipName = itam.RelationshipName;
                    ben.MobileNumber = itam.MobileNumber;
                    ben.EmailAddress = itam.EmailAddress;
                    connection.Add(ben);

                }


                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, connection));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := GetUserConnections) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
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
        public async Task<IActionResult> ReloadRequest(RequestBalanceReloadModel model)
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

                var benefactorForReloadRequest = await _invite.GetUserInfoById(model.BenefactorId);
                if(benefactorForReloadRequest.IsRequestAccepted !=true)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.BenefactorInvitationPending));
                }
               
                    var Userbenefactor = await _userRepository.CheckUserByEmail(benefactorForReloadRequest.Email);

                var reloadRequest = await _benefactor.PartnerReloadBalanceRequest(userIdClaimForReloadRequest, Userbenefactor.Id, programIdClaimForReloadRequest, model.Message , model.Amount);
                if (reloadRequest < 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ProgramExpired));
                }
                if (reloadRequest > 0)
                {
                    try
                    {
                        var usersForReloadRequest = await _userRepository.GetUsersDetailByIds(new List<int> { userIdClaimForReloadRequest, Userbenefactor.Id });
                        var userDetailForReloadRequest = usersForReloadRequest.FirstOrDefault(x => x.Id == userIdClaimForReloadRequest);
                        //  var benefactorForReloadRequest = usersForReloadRequest.FirstOrDefault(x => x.Id == model.BenefactorId);
                       // var benefactorForReloadRequest = await _invite.GetUserInfoById(model.BenefactorId);
                        var programDetailForReloadRequest = await _program.GetProgramById(programIdClaimForReloadRequest);
                        /* Email Sent */
                        var URLRedirectLink = string.Concat(_configuration["WebURL"], "Benefactor/ReloadRequest?id=", userDetailForReloadRequest.Id, "&reloadRequestId=", reloadRequest, "&programId=", programIdClaimForReloadRequest);
                        var template = await _emailService.GetEmailTemplateByName(EmailTemplates.PartnerReloadBalanceRequest);
                        template.Subject = template.Subject.Replace("{SenderName}", string.Concat(userDetailForReloadRequest?.FirstName, " ", userDetailForReloadRequest?.LastName));
                      //  template.Body = template.Body.Replace("Trove", "Sodexo");
                        template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(benefactorForReloadRequest?.FirstName, " ", benefactorForReloadRequest?.LastName)).Replace("{SenderName}", string.Concat(userDetailForReloadRequest?.FirstName, " ", userDetailForReloadRequest?.LastName)).Replace("{link}", URLRedirectLink).Replace("{ProgramName}", programDetailForReloadRequest?.Name).Replace("{AccountName}", "Discretionary").Replace("{Amount}", model.Amount.ToString()).Replace("{Message}", model.Message);
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
        /// This Api called to get the tranlog data.
        /// </summary>
        /// <param name="id"></param>

        /// <returns></returns>
        [Route("GettranlogData")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GettranlogData(int id)
        {
            try
            {
                var userdetail =await  _userRepository.GetUserInfoById(id);               
                var getdata =  _tranlog.gettranslog(userdetail?.PartnerUserId).ToList();
                if (getdata.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, getdata, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Loyality := GetOrgLoyalityGlobalSettings)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }




        [Route("UploadBinfile")]
        [HttpPost]

        public async Task<IActionResult> UploadBinfile()
        {
            try
            {

                var txtfile = Request.Form.Files.First();
                if (txtfile == null)
                {
                    //  throw new UserFriendlyException(L("File_Empty_Error"));
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, "File Empty Error", 1));

                }

                string extension = System.IO.Path.GetExtension(txtfile.FileName).ToLower();
                string[] validFileTypes = { ".txt", ".TXT" };
                if (!validFileTypes.Contains(extension))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, "Please Upload Files in txt format", 1));

                    //    throw new UserFriendlyException("Please Upload Files in .xls, .xlsx or .csv format");
                }


                //var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fullPath="";
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (txtfile.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(txtfile.ContentDisposition).FileName.Trim('"');
                     fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        txtfile.CopyTo(stream);
                    }
                }

                DataTable table = new DataTable("BinFile");


                DataColumn dtColumn = new DataColumn();
                dtColumn = new DataColumn();
                dtColumn.DataType = typeof(Int64);
                dtColumn.ColumnName = "BinStart";
                table.Columns.Add(dtColumn);


                dtColumn = new DataColumn();
                dtColumn.DataType = typeof(Int64);
                dtColumn.ColumnName = "BinEnd";
                table.Columns.Add(dtColumn);

                dtColumn = new DataColumn();
                dtColumn.DataType = Type.GetType("System.String");
                dtColumn.ColumnName = "code";
                table.Columns.Add(dtColumn);

                List<BinFileModel> varBinFile = new List<BinFileModel>();

               // var filePath = Path.GetTempFileName();
                using (StreamReader file1 = new StreamReader(fullPath))

                {

                    string ln;

                    while ((ln = file1.ReadLine()) != null)
                    {
                        string a = ln.Substring(97, 4);
                        if (ln.Substring(97, 4) == "USAV" || ln.Substring(97, 4) == "USAM" || ln.Substring(97, 4) == "USAA" || ln.Substring(97, 4) == "USAD")
                        {
                            string indicator = "CHJKLMNNORSX";
                            if (indicator.Contains(ln.Substring(101, 1)))
                            {
                                BinFileModel bin = new BinFileModel();

                                bin.BinStart = Convert.ToInt64(ln.Substring(1, 15).Trim());
                                bin.BinEnd = Convert.ToInt64(ln.Substring(16, 15).Trim());
                                bin.code = ln.Substring(97, 6).Trim();
                                DataRow dr = table.NewRow();
                                dr[0] = Convert.ToInt64(ln.Substring(1, 15).Trim());
                                dr[1] = Convert.ToInt64(ln.Substring(16, 15).Trim());
                                dr[2] = ln.Substring(97, 6).Trim();
                                table.Rows.Add(dr);


                                varBinFile.Add(bin);

                            }
                        }
                    }
                    file1.Close();
                }
                await _binDataService.InsertUpdateBinData(table);

                FileInfo file = new FileInfo(fullPath);
                if (file.Exists)//check file exsit or not
                {
                    file.Delete();
                }


                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, varBinFile, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Partner :=  UploadBinfile())", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }
        /// <summary>
        /// This Api is called to get the User loyalty reward transactions.
        /// </summary>
        /// <returns></returns>
        [Route("GetUserLoyaltyRewardTransactions")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserLoyaltyRewardTransactions(int userId,int pagenumber, int pagelength)
        {
            try
            {

                var userdetail = await _userRepository.GetUserInfoById(userId);
                var Usertransactions =  _tranlog.getUserLoyaltyRewardTransactions(userdetail?.PartnerUserId,pagenumber,pagelength);
                if (Usertransactions == null && userId > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, Usertransactions, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, Usertransactions, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Partner := GetUserLoyaltyRewardTransactions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }

        /// <summary>
        /// This Api is called to get the User bite pay transactions.
        /// </summary>
        /// <returns></returns>
        [Route("GetUserBitePayTransactions")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserBitePayTransactions(int userId,int pagenumber,int pagelength)
        {
            try
            {

                var userdetail = await _userRepository.GetUserInfoById(userId);
                var Usertransactions = _tranlog.getUserBitePayTransactions(userdetail?.PartnerUserId,pagenumber,pagelength);
                if (Usertransactions == null && userId > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, Usertransactions, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, Usertransactions, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Partner := GetUserLoyaltyRewardTransactions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                throw ex;
            }
        }

    }
}