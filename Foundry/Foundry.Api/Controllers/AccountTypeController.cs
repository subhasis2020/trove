using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include methods for account types.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountTypeController : ControllerBase
    {
        private readonly IAccountTypeService _accountType;
        private readonly ApiResponse NoProgramsExist = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoProgramsExist);
        private readonly ApiResponse SomeIssueInProcessing = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing);
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="accountType"></param>
        public AccountTypeController(IAccountTypeService accountType)
        {
            _accountType = accountType;
        }

        /// <summary>
        /// This Api is called to get all account types in the system.
        /// </summary>
        /// <returns></returns>
        [Route("GetAccountType")]
        [HttpGet]
        public async Task<IActionResult> GetAllAccountType()
        {
            try
            {
                var lstAccounttype = (await _accountType.GetAllAccountTypes()).ToList();
                if (lstAccounttype.Count <= 0)
                {
                    return Ok(NoProgramsExist); 
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, lstAccounttype));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (AccountType := GetAccountType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(SomeIssueInProcessing);
            }
        }

        /// <summary>
        /// This Api is called to get the account type detail based on its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetAccountTypeById")]
        [HttpGet]
        public async Task<IActionResult> GetAccountTypeById(int id)
        {
            try
            {
                var accounttype = (await _accountType.GetDataByIdAsync(new { id }));
                if (accounttype == null)
                {
                    return Ok(NoProgramsExist); 
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, accounttype));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Program := GetProgramCodes)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(SomeIssueInProcessing);
            }
        }
    }
}