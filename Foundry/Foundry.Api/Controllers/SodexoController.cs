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

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SodexoController : ControllerBase
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

        private readonly IGeneralSettingService _generalRepository;
        private readonly II2CBank2CardTransferService _i2cBank2CardTransferRepository;
        private readonly IUserTransactionInfoes _userTransactionRepository;
        private readonly UserManager<User> _userManager;


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
        public SodexoController(IUserRepository userRepository, IOrganisation organisation, IPrograms program,
             ICardHolderAgreementService cardHolderAgreementService
            , IBenefactorService vbenefactor
            , IGeneralSettingService vsetting
            ,IConfiguration vconfiguration
            ,ISMSService vsMSService
            ,IEmailService vemailService
            ,IInvitationService  vinvite
            , IMapper mapper
            ,II2CAccountDetailService vi2cAccountDetailRepository
            , IAcquirerService vacquirerService
            , II2CCardBankAccountService vi2cCardBankAccountRepository
            , II2CLogService vi2cLogRepository
            , IGeneralSettingService vgeneralRepository
            , II2CBank2CardTransferService vi2cBank2CardTransferRepository
            , IUserTransactionInfoes vuserTransactionRepository
            , UserManager<User> vuserManager
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
        }


        [HttpGet]
        [Route(template: "getstr")]
        public string PostSodexoAccountHolder()
        {
            return "sdhsu"; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route(template: "RegisterAccountHolder")]
        //public async Task<IActionResult> PostSodexoAccountHolder(RegisterAccountHolderSodexoModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse(StatusCodes.Status400BadRequest, true, MessagesConstants.InvalidInputData, ModelState.Values.SelectMany(x => x.Errors).ToList()));
        //        }
        //        var chkUserByEmail = await _userRepository.CheckUserByEmail(model.PrimaryEmail);
        //        var chkUserByPhone = await _userRepository.CheckPhoneNumberExistence(model.PhoneNumber);
        //        if (chkUserByEmail != null && model.Id < 0 && model.Id != chkUserByEmail.Id)
        //        {
        //            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.EmailExists));
        //        }
        //        if (chkUserByPhone != null && model.Id < 0 && model.Id != chkUserByPhone.Id)
        //        {
        //            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.PhoneNumberExists));
        //        }
        //        var organisationContent = await _organisation.FindAsync(new { OrganisationSubTitle = "Sodexo", IsActive = true });
        //        var userModel = new User
        //        {
        //            UserCode = model.AccountHolderEnrollmentNo,
        //            dateOfBirth = model.DateOfBirth,
        //            FirstName = model.FirstName,
        //            genderId = model.GenderId,
        //            Id = model.Id,
        //            LastName = model.LastName,
        //            PasswordHash = _configuration["BiteUserDefultPassword"] , //model.PasswordHash,
        //            PhoneNumber = model.PhoneNumber,
        //            Email = model.PrimaryEmail,
        //            ProgramId = 0,
        //            secondaryEmail = model.SecondaryEmail,
        //            OrganisationId = organisationContent != null ? organisationContent?.id : 0,
        //            UserDeviceId = model.DeviceId,
        //            UserDeviceType = model.DeviceType,
        //            BiteUserId = model.BiteUserId
        //        };
        //        var usrforRegisterUser = await _userRepository.RegisterSodexoUser(userModel);
        //        return StatusCode(StatusCodes.Status200OK, MessagesConstants.UserSuccessfulRegistration);
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.RiseError(new Exception(string.Concat("API := (Sodexo := GetCardHolderAgreementsExistence)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(StatusCodes.Status500InternalServerError, false, MessagesConstants.SomeIssueInProcessing));
        //    }

        //}

        #region CardHolder Agreement
        /// <summary>
        /// This Api is used to check cardholder agreement existence
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route(template: "CardHolderAgreement")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementExistence(int programId)
        {
            try
            {
                var cardHolderAgreementsExistence = (await _cardHolderAgreementService.GetCardHolderAgreementsExistence(programId));
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
                    IsAgreementRead = model.IsAgreementRead
                };
                var userAgreement = await _userRepository.UpdateUserCardHolderAgreementReadDetail(user);
                if (userAgreement <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.UserCardholderAgreementSettingsNotUpdated));
                }
                //var userAgreementHistory = new UserAgreementHistory()
                //{
                //    cardHolderAgreementVersionNo = model.AgreementVersionNo,
                //    createdBy = userIdClaimUpdateUser,
                //    modifiedBy = userIdClaimUpdateUser,
                //    programId = programIdClaimUpdateUser,
                //    userId = userIdClaimUpdateUser,
                //    dateAccepted = DateTime.UtcNow,
                //    createdOn = DateTime.UtcNow,
                //    modifiedOn = DateTime.UtcNow
                //};
                //await _userAgreementHistoryService.AddAsync(userAgreementHistory);
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
                    /*Email*/
                    // Get email template by name.
                    var template = await _emailService.GetEmailTemplateByName(EmailTemplates.ResetPassword);
                    template.Body = template.Body.Replace("{LogoImage}", "").Replace("{Name}", string.Concat(userObj.FirstName, " ", userObj.LastName));
                    // Calling a method to send an email.
                    await _emailService.SendEmail(userObj.Email, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
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
                CardNumber = (cardResponse.AddCardResponse1.NewCardNumber != null ? cardResponse.AddCardResponse1.NewCardNumber.Number : cardResponse.AddCardResponse1.BatchReferenceID),
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
                        Number = accountDetail.CardNumber
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

        [HttpGet("EnryptString/{strEncrypted}")]
        public async Task<string> EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
           
        }


        //[HttpGet("Authorisation/{id}")]
        //public async Task<IActionResult> Authorisation(string id )
        //{
        //    byte[]  b = Convert.FromBase64String(id);
        //    string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);

        //    var User = await _userRepository.GetUserbyBiteUserId(long.Parse(decrypted));

        //  //  var User = await _userManager.FindByIdAsync(decrypted.ToString());
        //    if(User ==null )
        //    {
        //        return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, true, MessagesConstants.BiteUserNotExist));
        //    }
        //    var client = new HttpClient();
        //    TokenResponse tokenResponse = await GetAuthToken(User.UserName, client);

        //    if (tokenResponse.IsError)
        //    {
        //     return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, true, MessagesConstants.UnabletoAuthorize));
        //    }

        //    return Ok(new
        //    {
        //        access_token = tokenResponse.AccessToken,
        //        expires_in= tokenResponse.ExpiresIn,
        //        token_type =tokenResponse.TokenType,
        //        refresh_token = tokenResponse.RefreshToken

        //    });
        //}

        //private async Task<TokenResponse> GetAuthToken(string emailAddress, HttpClient client)
        //{
        //    return await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        //    {
        //        Address = _configuration["ServiceAPIURL"] + ApiConstants.GenerateUserToken,

        //        ClientId = "ro.angular",
        //        ClientSecret = "secret",
        //        Scope = "openid offline_access",
        //        GrantType = "password",
        //        UserName = emailAddress.Trim(),
        //        Password = _configuration["BiteUserDefultPassword"]
        //    });
        //}

        //    [HttpPost("loginas/{id}")]
        //    //[Authorize(Roles = "admin")]
        //    public async Task<IActionResult> LoginAs(int id, [FromServices] ITokenService TS,
        //[FromServices] IUserClaimsPrincipalFactory<User> principalFactory,
        //[FromServices] IdentityServerOptions options)
        //    {
        //        var Request = new TokenCreationRequest();
        //        var User = await _userManager.FindByIdAsync(id.ToString());
        //        var IdentityPricipal = await principalFactory.CreateAsync(User);
        //        var IdServerPrincipal = IdentityServerPrincipal.Create(User.Id.ToString(), User.UserName, IdentityPricipal.Claims.ToArray());

        //        Request.Subject = IdServerPrincipal;
        //        Request.IncludeAllIdentityClaims = true;
        //        Request.ValidatedRequest = new ValidatedRequest();
        //        Request.ValidatedRequest.Subject = Request.Subject;
        //        //Request.ValidatedRequest.SetClient(Config.GetClients().First());
        //        //Request.Resources = new Resources(Config.GetIdentityResources(), Config.GetApiResources());
        //        Request.ValidatedRequest.Options = options;
        //        Request.ValidatedRequest.ClientClaims = IdServerPrincipal.Claims.ToArray();

        //        var Token = await TS.CreateAccessTokenAsync(Request);
        //        Token.Issuer = "http://" + HttpContext.Request.Host.Value;

        //        var TokenValue = await TS.CreateSecurityTokenAsync(Token);
        //        return Ok(TokenValue);
        //    }


    }
}