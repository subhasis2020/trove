using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ElmahCore;
using Foundry.Api.Attributes;
using Foundry.Api.Models;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Foundry.Domain.Constants;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include methods to get user transactions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserTransactionsController : ControllerBase
    {
        private readonly IUserTransactionInfoes _userTransactionService;
        private readonly IUserRepository _userRepository;
        private readonly IProgramAccountService _programAccount;
        private readonly IGeneralSettingService _setting;
        private readonly IUserNotificationSettingsService _userNotificationSettingsService;
        private readonly IUserPushedNotificationService _userPushedNotificationService;
        private readonly II2CAccountDetailService _i2cAccountDetail;
        private readonly IPrograms _program;
       

        /// <summary>
        ///  Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="userTransactionService"></param>
        /// <param name="userRepository"></param>
        /// <param name="programAccount"></param>
        /// <param name="setting"></param>
        /// <param name="userNotificationSettingsService"></param>
        /// <param name="userPushedNotificationService"></param>
        /// <param name="userPushedNotificationsStatusService"></param>
        /// <param name="program"></param>
        /// <param name="i2cAccountDetail"></param>
        public UserTransactionsController(IUserTransactionInfoes userTransactionService, IUserRepository userRepository,
            IProgramAccountService programAccount, IGeneralSettingService setting, IUserNotificationSettingsService userNotificationSettingsService,
            IUserPushedNotificationService userPushedNotificationService, IUserPushedNotificationsStatusService userPushedNotificationsStatusService,
            IPrograms program, II2CAccountDetailService i2cAccountDetail)
        {
            _userTransactionService = userTransactionService;
            _userRepository = userRepository;
            _programAccount = programAccount;
            _setting = setting;
            _userNotificationSettingsService = userNotificationSettingsService;
            _userPushedNotificationService = userPushedNotificationService;
            _program = program;
            _i2cAccountDetail = i2cAccountDetail;
        }
        /// <summary>
        /// This Api is called to get user available in descretionary.
        /// </summary>
        /// <returns></returns>
        [Route("GetUserAvailableBalance")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserAvailableBalance()
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "userId".ToLower().Trim()).Value);
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "programId".ToLower().Trim()).Value);
                var userBalance = await _userTransactionService.GetUserAvailableBalance(userIdClaim, programIdClaim);

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userBalance.ToList()));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (UserTransactions := GetUserAvailableBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        /// <summary>
        /// This Api is called to get user available in all account types.
        /// </summary>
        /// <returns></returns>
        [Route("GetUserAvailableBalanceByUserId")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserAvailableBalanceByUserId(int userId, int programId)
        {
            try
            {
                var userBalance = await _userTransactionService.GetUserAvailableBalance(userId, programId);
                var userBalanceSkipLastReload = userBalance.Where(x=>!x.DataKey.Contains("Last Reload Date"));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userBalanceSkipLastReload));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (UserTransactions := GetUserAvailableBalanceByUserId)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get user available balance for VPL.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetUserAvailableBalanceForVPL")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserAvailableBalanceForVPL(int userId, int programId)
        {
            try
            {
                var userBalance = await _userTransactionService.GetUserAvailableBalanceForVPL(userId, programId);

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, userBalance.DataValue, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (UserTransactions := GetUserAvailableBalanceForVPL)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is called to get respective user transaction with date filter.
        /// </summary>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        [Route("GetRespectiveUserTransactions")]
        [Authorize]
        [HttpGet]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetRespectiveUserTransactions(string dateMonth = null)
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "userId".ToLower().Trim()).Value);
                var sessionIdClaim = User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sessionMobileId".ToLower().Trim()).Value;
                var programIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "programId".ToLower().Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaim, sessionIdClaim)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                DateTime? dateFilter = null;
                if (!string.IsNullOrEmpty(dateMonth))
                    dateFilter = Convert.ToDateTime(dateMonth);

                var transactions = await _userTransactionService.GetRespectiveUsersTransactions(userIdClaim, dateFilter, programIdClaim);
                if (transactions.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoTransactionsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, transactions));

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (UserTransactions := GetRespectiveUserTransactions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUserTransaction")]
        [Authorize]
        public async Task<IActionResult> AddUserTransaction(FiservMainTransactionModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                if (model.ProgramAccountIdSelected > 0)
                {
                    var planProgram = await _programAccount.GetUserProgramPlanNOrgByProgramAccountId(model.ProgramAccountIdSelected,model.CreditUserId);
                    model.PlanId = planProgram?.planId;
                    model.ProgramId = planProgram?.programId;
                    model.OrganizationId = planProgram?.OrganizationId;
                    model.AccountTypeId = planProgram?.AccountTypeId;
                    model.OrganizationId = planProgram.OrganizationId;
                }
                var objUserTransaction = new UserTransactionInfo()
                {
                    accountTypeId = 3,
                    //accountTypeId = model.AccountTypeId,
                    createdBy = userIdClaim,
                    createdDate = DateTime.UtcNow,
                    CreditTransactionUserType = TransactionUserEnityType.User,
                    creditUserId = model.CreditUserId,
                    debitUserId = userIdClaim,
                    DebitTransactionUserType = TransactionUserEnityType.Benefactor,
                    isActive = true,
                    isDeleted = false,
                    modifiedBy = userIdClaim,
                    modifiedDate = DateTime.UtcNow,
                    organisationId = model.OrganizationId,
                    transactionAmount = model.approvedAmount.total,
                    transactionDate = model.TransactionDateTime,
                    TransactionId = model.ipgTransactionId,
                    transactionStatus = 1,
                    TransactionPaymentGateway = model.TransactionPaymentGateway,
                    //   programAccountId = model.ProgramAccountIdSelected,
                    programAccountId = model.ProgramId.Value,
                    programId = model.ProgramId.Value,
                    planId = model.PlanId,

                };
                var transactionUserAdd = await _userTransactionService.AddAsync(objUserTransaction);
                if (model.AccountTypeId == 3)
                {
                    var userCardAccountBalance = await _i2cAccountDetail.FindAsync(new { UserId = model.CreditUserId });
                    if (userCardAccountBalance != null)
                    {// That will work only for discretionary for i2c later
                        userCardAccountBalance.Balance = userCardAccountBalance.Balance + objUserTransaction.transactionAmount;
                        await _i2cAccountDetail.UpdateAsync(userCardAccountBalance, new { Id = userCardAccountBalance.Id });
                    }
                   
                }
                var creditUserDetail = await _userRepository.FindAsync(new { Id = model.CreditUserId });
                var debitUserDetail = await _userRepository.FindAsync(new { Id = userIdClaim });
                /*  Notification  */
                var balanceContent = string.Empty;
                if (model.AccountTypeId == 1)
                    balanceContent = string.Concat(objUserTransaction.transactionAmount, " meal passes");
                else if (model.AccountTypeId == 2)
                    balanceContent = string.Concat(objUserTransaction.transactionAmount, " flex points");
                else if (model.AccountTypeId == 3)
                    balanceContent = string.Concat("$", objUserTransaction.transactionAmount);
                string notificationMessage = MessagesConstants.BalanceAddedNotificationMessage.Replace("{Benefactor}", string.Concat(debitUserDetail?.FirstName, " ", debitUserDetail?.LastName)).Replace("{Balance}", balanceContent), notificationTitle = MessagesConstants.BalanceAddedNotifyTitle;
                var programNotificationSetCheck = await _program.FindAsync(new { id = creditUserDetail?.ProgramId });
                if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
                {
                    var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                    PushNotifications push = new PushNotifications();
                    var usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(new List<int> { creditUserDetail.Id }, (int)NotificationSettingsEnum.Transaction);
                    if (usrNotify.Count > 0)
                    {
                        await push.SendPushBulk(new List<string> { creditUserDetail?.UserDeviceId }, notificationTitle, notificationMessage, "", userIdClaim.ToString(), "transaction", "icon", "transaction", 1, (serverApiKey != null ? serverApiKey.Value : ""), false, "AddBalance", 0);
                    }
                }
                await _userPushedNotificationService.AddAsync(new UserPushedNotifications()
                {
                    notificationMessage = notificationMessage,
                    notificationTitle = notificationTitle,
                    notificationType = (int)NotificationSettingsEnum.Transaction,
                    referenceId = 1,
                    createdBy = userIdClaim,
                    modifiedBy = userIdClaim,
                    ProgramId = creditUserDetail?.ProgramId,
                    userId = creditUserDetail.Id,
                    IsRedirect = true,
                    NotificationSubType = "AddBalance",
                    CustomReferenceId = 0
                });
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, transactionUserAdd));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (UserTransactions := AddUserTransaction)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }
    }
}