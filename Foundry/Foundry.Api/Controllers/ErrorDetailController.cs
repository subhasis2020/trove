using System.Threading.Tasks;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Services.Errors;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Foundry.Domain.Dto;
using System.Collections.Generic;
using System;
using ElmahCore;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class is used to include methods for error mesaages retrieving from table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorDetailController : ControllerBase
    {
        private readonly IGeneralErrorDetail _errors;
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="mapper"></param>
        public ErrorDetailController(IGeneralErrorDetail errors, IMapper mapper)
        {
            _errors = errors;
            _mapper = mapper;
        }

        /// <summary>
        /// This Api is called to get the errors from database for mobile app.
        /// </summary>
        /// <returns></returns>
        [Route("GetAllErrors")]
        [HttpPost]
        public async Task<IActionResult> GetErrors()
        {
            try
            {
                var errors = _mapper.Map<List<GeneralErrorsDto>>(await _errors.GetGeneralErrors());
                if (errors == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoErrorMessagesExist)); // 404
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, errors));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Error := GetErrors)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }
    }
}