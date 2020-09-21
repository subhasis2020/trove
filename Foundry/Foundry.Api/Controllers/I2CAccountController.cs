using AutoMapper;
using ElmahCore;
using foundry;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Foundry.Services.AcquirerService;
using Foundry.Services.PartnerNotificationsLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using static Foundry.Domain.Constants;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include all methods related to I2CAccounts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class I2CAccountController : ControllerBase
    {
        #region Private Variable

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly II2CLogService _i2cLogRepository;
        private readonly II2CAccountDetailService _i2cAccountDetailRepository;
        private readonly II2CCardBankAccountService _i2cCardBankAccountRepository;
        private readonly II2CBank2CardTransferService _i2cBank2CardTransferRepository;
        private readonly IUserTransactionInfoes _userTransactionRepository;
        private readonly IAcquirerService _acquirerService;
        private readonly IMapper _mapper;
        private readonly IGeneralSettingService _generalRepository;
        private readonly IFiservMethods _fiservMethods;
        private readonly IPartnerNotificationsLogServicer _partnerNotificationsLogRepository;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="i2cLogRepository"></param>
        /// <param name="configuration"></param>
        /// <param name="i2cAccountDetailRepository"></param>
        /// <param name="acquirerService"></param>
        /// <param name="mapper"></param>
        /// <param name="generalRepository"></param>
        /// <param name="i2cCardBankAccountRepository"></param>
        /// <param name="i2cBank2CardTransferRepository"></param>
        /// <param name="userTransactionRepository"></param>
        public I2CAccountController(IUserRepository userRepository,
            II2CLogService i2cLogRepository, IConfiguration configuration,
            II2CAccountDetailService i2cAccountDetailRepository, IAcquirerService acquirerService,
            IMapper mapper, IGeneralSettingService generalRepository,
            II2CCardBankAccountService i2cCardBankAccountRepository,
            II2CBank2CardTransferService i2cBank2CardTransferRepository,
            IUserTransactionInfoes userTransactionRepository,
            IPartnerNotificationsLogServicer partnerNotificationsLogRepository,
            IFiservMethods fiservMethods)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _i2cLogRepository = i2cLogRepository;
            _acquirerService = acquirerService;
            _i2cAccountDetailRepository = i2cAccountDetailRepository;
            _mapper = mapper;
            _generalRepository = generalRepository;
            _i2cCardBankAccountRepository = i2cCardBankAccountRepository;
            _i2cBank2CardTransferRepository = i2cBank2CardTransferRepository;
            _userTransactionRepository = userTransactionRepository;
            _fiservMethods = fiservMethods;
            _partnerNotificationsLogRepository = partnerNotificationsLogRepository;
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
                // CardNumber = (cardResponse.AddCardResponse1.NewCardNumber != null ? cardResponse.AddCardResponse1.NewCardNumber.Number : cardResponse.AddCardResponse1.BatchReferenceID),
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

        #endregion

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
                if (objAccountDetail!= null)
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

        private async Task i2cLogResponse(I2CLog objRequestLog, string xml)
        {
            objRequestLog.Response = xml;
            objRequestLog.UpdatedDate = DateTime.UtcNow;
            await _i2cLogRepository.UpdateAsync(objRequestLog, new { id = objRequestLog.Id });
        }

        private async Task<List<GeneralSetting>> Geti2cGeneralSettings()
        {
            var settings = await _generalRepository.GetDataAsync(new { keyGroup = MessagesConstants.i2cKeyName });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }

        #region Balance

        /// <summary>
        /// This Api is called to Get i2c account Balance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Route("GetBalance")]
        [HttpGet]
        // [Authorize]
        public async Task<IActionResult> Geti2cUserBalance(int id, decimal amount)
        {
            try
            {
                /* Get accountDetail for card number */
                var accountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { UserId = id });
                if (accountDetail == null || accountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }

                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                BalanceInquiryType balanceInquiryTypeObj = new BalanceInquiryType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Card = new CardTypeAdd() { ReferenceID = accountDetail.ReferenceId } //to do decode
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(balanceInquiryTypeObj), "Request");
                I2CLog objRequestLog = await i2cLogRequest(accountDetail.UserId.Value, string.Empty, "VerifyAccount", _configuration["ServiceAPIURL"] + "I2CAccount/VerifyAccount", string.Empty, request);

                /* Call i2c api verify Account */
                var balanceInquiryObj = await _acquirerService.BalanceInquiry(balanceInquiryTypeObj);
                if (balanceInquiryObj.BalanceInquiryResponse1.ResponseCode == "00")
                {
                    objRequestLog.Status = "Success";
                    /* Update Log table */
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(balanceInquiryObj.BalanceInquiryResponse1), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    decimal i2cUserCardBalance = Convert.ToDecimal(balanceInquiryObj.BalanceInquiryResponse1.Balance);
                    if (accountDetail.Balance != i2cUserCardBalance)
                    {
                        accountDetail.Balance = Convert.ToDecimal(balanceInquiryObj.BalanceInquiryResponse1.Balance);
                        await _i2cAccountDetailRepository.UpdateAsync(accountDetail, new { Id = accountDetail.Id });
                    }

                    if (i2cUserCardBalance < amount)
                        return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.InsufficientBalance, i2cUserCardBalance));
                    else
                        return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.BalanceReturnedSuccessfully));
                }
                else
                {
                    objRequestLog.Status = "Failed";
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(balanceInquiryObj.BalanceInquiryResponse1), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, balanceInquiryObj.BalanceInquiryResponse1.ResponseDesc));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := GetBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        #endregion




        /// <summary>
        /// This Api is called to Get i2c account detail.
        /// </summary>
        /// <param name="id">userId</param>
        /// <returns>Account Detail</returns>
        [Route("Get")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Geti2cUserAccountDetail(int id)
        {
            try
            {
                var userVirtualCardDetail = _mapper.Map<List<I2CAccountDetailModel>>(await _i2cAccountDetailRepository.GetDataAsync(new { Userid = id }));
                if (userVirtualCardDetail.Count <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userVirtualCardDetail.FirstOrDefault()));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := Geti2cAccountDetailById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to Get i2c card bank accounts.
        /// </summary>
        /// <param name="byUserId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [Route("Account")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBankAccounts(int byUserId, int toUserId)
        {
            try
            {
                var objBankAccounts = await _i2cCardBankAccountRepository.GetBankAccountListing(byUserId, toUserId);
                if (objBankAccounts.Count > 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, objBankAccounts));
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoBankAccountExists, objBankAccounts));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := GetBankAccounts)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        /// <summary>
        /// This Api is called to Get i2c card bank accounts with credit card.
        /// </summary>
        /// <param name="byUserId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [Route("GetUserCardwithBankAccount")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserCardwithBankAccount(int byUserId, int toUserId)
        {
            try
            {
                var objBankAccounts = await _userRepository.GetUserCardwithBankAccount(byUserId, toUserId);
                if (objBankAccounts.Count > 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, objBankAccounts));
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoBankAccountExists, objBankAccounts));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := GetBankAccounts)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }
        /// <summary>
        /// This Api is called to get the bank account details by serial no.
        /// </summary>
        /// <param name="accountSrNum"></param>
        /// <returns>i2CCardBankAccount</returns>
        [Route("AccountBySerialNo")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBankAccountBySerialNo(string accountSrNum)
        {
            try
            {
                var objBankAccountBySerialNo = _mapper.Map<I2CCardBankAccountModel>(await _i2cCardBankAccountRepository.GetSingleDataByConditionAsync(new { AccountSrNo = accountSrNum }));
                if (objBankAccountBySerialNo != null)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, objBankAccountBySerialNo));
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoBankAccountExists, objBankAccountBySerialNo));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := GetBankAccounts)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to Remove bank account.
        /// </summary>
        /// <param name="accountSrNo"></param>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        [Route("RemoveBankAccount")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoveBankAccount(string accountSrNo, string cardNo)
        {
            try
            {
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                RemoveBankAccountType removeBankAccountType = new RemoveBankAccountType
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Account = new AccountTypeWithActSrNoRequird
                    {
                        //AccountNumber = "123456789123456789"
                        AccountSrNo = accountSrNo
                    },
                    Card = new CardTypeWithOptionalStatus
                    {
                        Number = cardNo
                    }
                };
                var obj = await _acquirerService.RemoveBankAccount(removeBankAccountType);
                if (obj.RemoveBankAccountResponse1.ResponseCode == "00")
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.BankAccountRemovedSuccessfully));
                }
                else
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.NoErrorMessagesExist));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := RemoveBankAccount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to load funds in VPL card
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="id">i2cCardBankAccountId</param>
        /// <param name="amount">amount credit</param>
        /// <returns></returns>
        [Route("LoadFunds")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LoadFunds(int id, decimal amount)
        {
            try
            {
                var i2cCardBankAccount = await _i2cCardBankAccountRepository.GetDataByIdAsync(new { id = id });
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
                CreditFundsType creditFundsTypeObj = new CreditFundsType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Card = new CardTypeAdd()
                    {
                        ReferenceID = i2cAccountDetail.ReferenceId
                    },
                    FundingCard = new FundingCardType()
                    {
                        Number = i2cCardBankAccount.AccountNumber
                    },
                    Amount = amount,
                    AmountSpecified = true
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(creditFundsTypeObj), "Request");
                I2CLog objRequestLog = new I2CLog()
                {
                    UserId = i2cAccountDetail.UserId,
                    AccountHolderUniqueId = i2cAccountDetail.AccountHolderUniqueId,
                    ApiName = "LoadFunds",
                    ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/LoadFunds",
                    IpAddress = "",
                    Request = request.InnerXml,
                    CreatedDate = DateTime.UtcNow
                };

                //log request in log table for i2c create bank account
                objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
                if (objRequestLog.Id > 0)
                {
                    var loadFundResponse = await _acquirerService.CreditFunds(creditFundsTypeObj);

                    if (loadFundResponse != null && loadFundResponse.CreditFundsResponse1.ResponseCode == "00")
                    {
                        objRequestLog.Status = "In-Progress";
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := RemoveBankAccount)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        /// <summary>
        /// This Api is called to credit funds in i2c card after fiserv payment
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="creditUserId">userid</param>
        /// <param name="amount">amount credit</param>
        /// <returns></returns>
        [Route("CreditFundsOnFiservPayment")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreditFundsOnFiservPayment(int creditUserId, decimal amount, string orderId)
        {
            try
            {

                var i2cAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { UserId = creditUserId });
                if (i2cAccountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                CreditFundsType creditFundsTypeObj = new CreditFundsType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Card = new CardTypeAdd()
                    {
                        ReferenceID = i2cAccountDetail.ReferenceId
                    },
                   
                    Amount = amount,
                    AmountSpecified = true
                };
                
                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(creditFundsTypeObj), "Request");
                I2CLog objRequestLog = new I2CLog()
                {
                    UserId = i2cAccountDetail.UserId,
                    AccountHolderUniqueId = i2cAccountDetail.AccountHolderUniqueId,
                    ApiName = "CreditFundsOnFiservPayment",
                    ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/CreditFundsOnFiservPayment",
                    IpAddress = "",
                    Request = request.InnerXml,
                    CreatedDate = DateTime.UtcNow
                };
                //log request in log table for i2c create bank account
                objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);

                var loadFundResponse = await _acquirerService.CreditFunds(creditFundsTypeObj);
              
                    if (loadFundResponse != null && loadFundResponse.CreditFundsResponse1.ResponseCode == "00")
                {
                  //  await AddUserTransaction(debitUserId, amount, programId, creditUserId);
                    /* Update balance in card */
                    i2cAccountDetail.Balance += amount;
                    var Balance = i2cAccountDetail.Balance;
                    await _i2cAccountDetailRepository.UpdateAsync(i2cAccountDetail, new { Id = i2cAccountDetail.Id });
                    objRequestLog.Status = "Success";


                    /* Update Log table */
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(loadFundResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                    var user = await _userRepository.GetUserById(creditUserId);


                    await sendAddValueNotification(user, amount.ToString(), Balance.ToString(), SodexoBiteNotification.SodexoBiteBaseUrl);

                    List<GeneralSetting> Settings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                    if (Settings[0].Value == "1")
                    {
                        await sendAddValueNotification(user, amount.ToString(), Balance.ToString(), SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                    }

                }
                    else
                {
                    objRequestLog.Status = "Failed";
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(loadFundResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                    await _fiservMethods.fiservReverseTransaction(amount, orderId);

                    return BadRequest();
                   // return Ok(new ApiResponse(StatusCodes.Status400BadRequest, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(loadFundResponse);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := CreditFundsOnFiservPayment)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        private async Task<List<GeneralSetting>> GetGeneralSettings(string Key)
        {
            var settings = await _generalRepository.GetDataAsync(new { KeyName = Key });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }
        private async Task sendAddValueNotification(UserDto user, string vAmountAdded, string vAvailableBalance, string key)
        {
            PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
            try
            {
                using (var client = new HttpClient())
                {
                    object obj = new
                    {

                        UserObjectId = user.PartnerUserId,
                        AmountAdded = vAmountAdded,
                        AvailableBalance = vAvailableBalance,
                        AddedBy = "",
                        AddedByBenefactor = "N"


                    };
                    List<GeneralSetting> i2cSettings = await GetGeneralSettings(key);

                    var url = i2cSettings[0].Value;
                    var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.BitePayAddValue);

                    string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("AddValue", hostURL.ToString(), myJSON, user.Id);
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






        /// <summary>
        /// This Api is called to credit funds in i2c card ByAdmin
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="creditUserId">userid</param>
        /// <param name="amount">amount credit</param>
        /// <returns></returns>


        [Route("CreditFundsByAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreditFundsByAdmin(int creditUserId, decimal amount)
        {
            try
            {

                var i2cAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { UserId = creditUserId });
                if (i2cAccountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                CreditFundsType creditFundsTypeObj = new CreditFundsType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    Card = new CardTypeAdd()
                    {
                        ReferenceID = i2cAccountDetail.ReferenceId
                    },

                    Amount = amount,
                    AmountSpecified = true
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(creditFundsTypeObj), "Request");
                I2CLog objRequestLog = new I2CLog()
                {
                    UserId = i2cAccountDetail.UserId,
                    AccountHolderUniqueId = i2cAccountDetail.AccountHolderUniqueId,
                    ApiName = "CreditFundsOnFiservPayment",
                    ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/CreditFundsOnFiservPayment",
                    IpAddress = "",
                    Request = request.InnerXml,
                    CreatedDate = DateTime.UtcNow
                };
                //log request in log table for i2c create bank account
                objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);

                var loadFundResponse = await _acquirerService.CreditFunds(creditFundsTypeObj);

                if (loadFundResponse != null && loadFundResponse.CreditFundsResponse1.ResponseCode == "00")
                {
                    //  await AddUserTransaction(debitUserId, amount, programId, creditUserId);
                    /* Update balance in card */
                    i2cAccountDetail.Balance += amount;
                    var Balance = i2cAccountDetail.Balance;
                    await _i2cAccountDetailRepository.UpdateAsync(i2cAccountDetail, new { Id = i2cAccountDetail.Id });
                    objRequestLog.Status = "Success";


                    /* Update Log table */
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(loadFundResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                    var user = await _userRepository.GetUserById(creditUserId);


                    await sendAddValueNotification(user, amount.ToString(), Balance.ToString(), SodexoBiteNotification.SodexoBiteBaseUrl);

                    List<GeneralSetting> Settings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                    if (Settings[0].Value == "1")
                    {
                        await sendAddValueNotification(user, amount.ToString(), Balance.ToString(), SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                    }

                }
                else
                {
                    objRequestLog.Status = "Failed";
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(loadFundResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    
                    return BadRequest();
                    // return Ok(new ApiResponse(StatusCodes.Status400BadRequest, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(loadFundResponse);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := CreditFundsOnFiservPayment)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        /// <summary>
        /// This Api is called to Debit Funds  in i2c card ByAdmin
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="creditUserId">userid</param>
        /// <param name="amount">amount credit</param>
        /// <returns></returns>
      
        [Route("DebitFundsByAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DebitFundsByAdmin(int creditUserId, decimal amount)
        {
            try
            {

                var i2cAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { UserId = creditUserId });
                if (i2cAccountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                DebitFundsType debitFundsTypeObj = new DebitFundsType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                    CardAcceptor = new CardAcceptorType()
                    {
                        Id = "22"
                    },
                    Card = new CardTypeAdd()
                    {
                        ReferenceID = i2cAccountDetail.ReferenceId
                    },

                    Amount = amount,
                    TransactionTypeId = "22"
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(debitFundsTypeObj), "Request");
                I2CLog objRequestLog = new I2CLog()
                {
                    UserId = i2cAccountDetail.UserId,
                    AccountHolderUniqueId = i2cAccountDetail.AccountHolderUniqueId,
                    ApiName = "DebitFundsByAdmin",
                    ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/DebitFundsByAdmin",
                    IpAddress = "",
                    Request = request.InnerXml,
                    CreatedDate = DateTime.UtcNow
                };
                //log request in log table for i2c create bank account
                objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);

                var DebitFundsResponse = await _acquirerService.DebitFunds(debitFundsTypeObj);

                if (DebitFundsResponse != null && DebitFundsResponse.DebitFundsResponse1.ResponseCode == "00")
                {
     
                    objRequestLog.Status = "Success";

                    /* Update Log table */
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(DebitFundsResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);
                    
                }
                else
                {
                    objRequestLog.Status = "Failed";
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(DebitFundsResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                    return BadRequest();
                    // return Ok(new ApiResponse(StatusCodes.Status400BadRequest, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(DebitFundsResponse);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := DebitFundsByAdmin)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }







        /// <summary>
        /// This Api is called to Debit Funds  in i2c card ByAdmin
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="creditUserId">userid</param>
        /// <param name="amount">amount credit</param>
        /// <returns></returns>

        [Route("ReversalTranByAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReversalTranByAdmin(decimal dsamount, int userId, string dstxnid, DateTime date)
        {
            try
            {

                var i2cAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { UserId = userId });
                if (i2cAccountDetail.Id <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoAccountDetailFound));
                }
                List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
                ReversalType reversalTypeObj = new ReversalType()
                {
                    Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),

                    Card = new CardTypeAdd()
                    {
                        ReferenceID = i2cAccountDetail.ReferenceId
                    },
                    Trans = new TransType()
                    {
                        ID = dstxnid,
                        Date = date.ToString("MM/dd/yyyy")
                        

                    },
                    Amount = dsamount
                };

                XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(reversalTypeObj), "Request");
                I2CLog objRequestLog = new I2CLog()
                {
                    UserId = i2cAccountDetail.UserId,
                    AccountHolderUniqueId = i2cAccountDetail.AccountHolderUniqueId,
                    ApiName = "Reversal",
                    ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/ReversalTranByAdmin",
                    IpAddress = "",
                    Request = request.InnerXml,
                    CreatedDate = DateTime.UtcNow
                };
                //log request in log table for i2c create bank account
                objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);

                var DebitFundsResponse = await _acquirerService.Reversal(reversalTypeObj);

                if (DebitFundsResponse != null && DebitFundsResponse.ReversalResponse1.ResponseCode == "00")
                {

                    objRequestLog.Status = "Success";

                    /* Update Log table */
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(DebitFundsResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                }
                else
                {
                    objRequestLog.Status = "Failed";
                    var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(DebitFundsResponse), "Response");
                    await i2cLogResponse(objRequestLog, response.InnerXml);

                    return BadRequest();
                    // return Ok(new ApiResponse(StatusCodes.Status400BadRequest, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(DebitFundsResponse);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := ReversalTranByAdmin)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }










        /// <summary>
        /// This Api is called for Bank to card transfer list
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="accountSrNo"></param>
        /// <returns></returns>
        [Route("Bank2CardTransferList")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Bank2CardTransferList(string cardNo, string accountSrNo)
        {
            List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();

            B2CTransferListType bankAccountsListType = new B2CTransferListType
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeWithOptionalStatus
                {
                    Number = cardNo
                },
                AccountSrNo = accountSrNo,
                ApplyFee = YN.Y
            };
            XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(bankAccountsListType), "Request");
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = 21,
                AccountHolderUniqueId = string.Empty,
                ApiName = "Bank2CardTransferLists",
                ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/Bank2CardTransferLists",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };
            //log request in log table for i2c create bank account
            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
            var res = await _acquirerService.Bank2CardTransferLists(bankAccountsListType);
            objRequestLog.Status = "Success";
            /* Update Log table */
            var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(res), "Response");
            await i2cLogResponse(objRequestLog, response.InnerXml);
            return Ok(new ApiResponse(StatusCodes.Status200OK, false, res.B2CTransferListResponse1.ResponseDesc));
        }

        /// <summary>
        /// This Api is called to initiate a request from bank to card.
        /// </summary>
        /// <param name="refId"></param>
        /// <param name="accountSrNo"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Route("B2CTransfer")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BankToCardTransferRequest(string refId, string accountSrNo, decimal amount)
        {
            List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();


            B2CTransferType b2CTransferType = new B2CTransferType
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeWithOptionalStatus
                {
                    ReferenceID = refId
                    //CustomerId = "110000000005612604"
                    //AccountNumber=
                },
                TransferDetail = new TransferDetailType
                {
                    AccountSrNo = accountSrNo,
                    Amount = amount,
                    TransferFrequency = TransferFrequencyType.O,
                    AmountSpecified = true,
                    TransferDate = DateTime.UtcNow.ToString("d"),
                    //TransferEndDate = DateTime.UtcNow.AddDays(-1).ToString("d")
                }
            };

            XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2CTransferType), "Request");
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = 6,
                AccountHolderUniqueId = string.Empty,
                ApiName = "B2CTransfer",
                ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/B2CTransfer",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };
            //log request in log table for i2c create bank account
            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
            var res = await _acquirerService.BankToCardTransfer(b2CTransferType);
            objRequestLog.Status = "Success";
            /* Update Log table */
            var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(res), "Response");
            await i2cLogResponse(objRequestLog, response.InnerXml);
            return Ok(new ApiResponse(StatusCodes.Status200OK, false, res.B2CTransferResponse1.ResponseDesc));
        }

        /// <summary>
        /// This Api is called to cancel bank to card transfer.
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        [Route("CancelB2CTransfer")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CancelBankToCardTransfer(string cardNo)
        {
            List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
            B2CTransferCancelType b2CTransferCancelType = new B2CTransferCancelType
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeWithOptionalStatus
                {
                    Number = cardNo
                },
                TransferDetail = new TransferDetailTypeWithTransferId
                {
                    TransferId = "8421"
                },
                ApplyFee = YN.Y
            };
            XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2CTransferCancelType), "Request");
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = 21,
                AccountHolderUniqueId = string.Empty,
                ApiName = "CancelB2CTransfer",
                ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/CancelB2CTransfer",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };
            //log request in log table for i2c create bank account
            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
            var res = await _acquirerService.CancelBankToCardTransfer(b2CTransferCancelType);
            objRequestLog.Status = "Success";
            /* Update Log table */
            var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(res), "Response");
            await i2cLogResponse(objRequestLog, response.InnerXml);
            return Ok(new ApiResponse(StatusCodes.Status200OK, false, res.B2CTransferCancelResponse1.ResponseDesc));
        }

        /// <summary>
        /// This Api is called to fetch bank account listing.
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="accountSrNo"></param>
        /// <returns></returns>
        [Route("BankAccountsList")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BankAccountsList(string cardNo, string accountSrNo)
        {
            List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
            BankAccountsListType bankAccountsListType = new BankAccountsListType
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeWithOptionalStatus
                {
                    Number = cardNo
                },
                Account = new AccountTypeWithActSrNoRequird
                {
                    AccountSrNo = accountSrNo
                }
            };

            XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(bankAccountsListType), "Request");
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = 21,
                AccountHolderUniqueId = string.Empty,
                ApiName = "BankAccountsList",
                ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/BankAccountsList",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };

            //log request in log table for i2c create bank account
            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);

            var res = await _acquirerService.BankAccountsList(bankAccountsListType);

            objRequestLog.Status = "Success";
            /* Update Log table */
            var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(res), "Response");
            await i2cLogResponse(objRequestLog, response.InnerXml);
            return Ok(new ApiResponse(StatusCodes.Status200OK, false, res.BankAccountsListResponse1.ResponseDesc));
        }

        /// <summary>
        /// This Api is called to get the transaction history for I2C.
        /// </summary>
        /// <param name="refId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [Route("TransactionHistory")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TransactionHistory(string refId, string dateFrom, string dateTo)
        {
            List<GeneralSetting> i2cSettings =   await Geti2cGeneralSettings();
            TransactionHistoryType transactionHistoryType = new TransactionHistoryType
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardType
                {
                    ReferenceID = "110557115737",
                    //CustomerId= "6019990205911025"

                },
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(transactionHistoryType), "Request");
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = 0,
                AccountHolderUniqueId = string.Empty,
                ApiName = "TransactionHistory",
                ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/TransactionHistory",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };
            //log request in log table for i2c create bank account
           // objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
            var res = await _acquirerService.TransactionHistory(transactionHistoryType);
            objRequestLog.Status = "Success";
            /* Update Log table */
            var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(res), "Response");
            await i2cLogResponse(objRequestLog, response.InnerXml);
            return Ok(new ApiResponse(StatusCodes.Status200OK, false, res.TransactionHistoryResponse1.ResponseDesc));
        }

        /// <summary>
        /// This Api is called to get card holder profile.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="refId"></param>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        [Route("GetCardHolderProfile")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCardHolderProfileAdvance(string customerId, string refId, string cardNo)
        {
            List<GeneralSetting> i2cSettings = await Geti2cGeneralSettings();
            GetCardholderProfileAdvanceType getCardholderProfileAdvanceType = new GetCardholderProfileAdvanceType
            {
                Acquirer = _acquirerService.GetAcquirerType(i2cSettings[0].Value, i2cSettings[1].Value, i2cSettings[2].Value),
                Card = new CardTypeWithOptionalStatus
                {
                    CustomerId = customerId,
                    ReferenceID = refId,
                    StatusCode = CSTAT.B,
                    Number = cardNo
                },
                ApplyFee = YN.Y
            };

            XmlDocument request = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(getCardholderProfileAdvanceType), "Request");
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = 0,
                AccountHolderUniqueId = string.Empty,
                ApiName = "GetCardHolderProfile",
                ApiUrl = _configuration["ServiceAPIURL"] + "I2CAccount/GetCardHolderProfile",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };

            //log request in log table for i2c create bank account
            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);

            var res = await _acquirerService.GetCardHolderProfileAdvance(getCardholderProfileAdvanceType);

            objRequestLog.Status = "Success";
            /* Update Log table */
            var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(res), "Response");
            await i2cLogResponse(objRequestLog, response.InnerXml);
            return Ok(new ApiResponse(StatusCodes.Status200OK, false, res.GetCardholderProfileAdvanceResponse1.ResponseDesc));
        }

    }
}