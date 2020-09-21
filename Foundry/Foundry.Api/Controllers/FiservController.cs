using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundry.LogService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using Foundry.Services;
using ElmahCore;
using Foundry.Domain.ApiModel;
using Foundry.Domain;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// Fiserv
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FiservController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IGatewayRequestResponseLogService _gatewayRequestRespLog;
        private readonly IGatewayCardWebHookTokenService _gatewayCardWebhookToken;
        private readonly IFiservPaymentTransactionLogService _paymentTransactionLog;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="gatewayRequestRespLog"></param>
        /// <param name="gatewayCardWebhookToken"></param>
        /// <param name="mapper"></param>
        /// <param name="paymentTransactionLog"></param>
        public FiservController(ILoggerManager logger, IGatewayRequestResponseLogService gatewayRequestRespLog,
            IGatewayCardWebHookTokenService gatewayCardWebhookToken, IMapper mapper, IFiservPaymentTransactionLogService paymentTransactionLog)
        {
            _logger = logger;
            _gatewayRequestRespLog = gatewayRequestRespLog;
            _gatewayCardWebhookToken = gatewayCardWebhookToken;
            _mapper = mapper;
            _paymentTransactionLog = paymentTransactionLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment")]
        public async Task<IActionResult> PaymentHook()
        {
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8))
                {
                    var responseNonce = Request.Headers["Nonce"];

                    var responseClientToken = Request.Headers["Client-Token"];
                    var content = await reader.ReadToEndAsync();
                    _logger.LogInfo("Webhook from Fiserv is as: ");
                    _logger.LogInfo("Webhook from Fiserv for NONCE is as: " + responseNonce);
                    _logger.LogInfo("Webhook from Fiserv for ClientToken is as: " + responseClientToken);
                    _logger.LogInfo(content);

                    dynamic response1 = JsonConvert.DeserializeObject(content);
                    PaymentCardWebhookDto objCard = response1.ToObject<PaymentCardWebhookDto>();



                    if (objCard != null)
                    {
                        bool isvalid =true;
                        var bindata = await _gatewayCardWebhookToken.CheckBinNumberIsValid((long)Convert.ToDouble(objCard.card.bin));
                        if (bindata!= null)
                        {
                            isvalid = true;

                        }
                        else
                        {
                            isvalid = false;
                        }
                        
                        var gatewayCardWebhook = new GatewayCardWebHookToken()
                        {
                            cardBrand = objCard.card.brand,
                            expiryMonthYear = string.Concat(objCard.card.exp.month, "/", objCard.card.exp.year),
                            last4digits = objCard.card.last4,
                            maskedLastDigitCard = objCard.card.masked,
                            nameOnCard = objCard.card.name,
                            Token = objCard.card.token,
                            TokenReceivedDate = DateTime.UtcNow,
                            ClientToken = responseClientToken,
                            Nonce = responseNonce,
                            Bin = (long)Convert.ToDouble(objCard.card.bin),
                            IsCardValid = isvalid
                        };
                        var gatewayResponse = new GatewayRequestResponseLog()
                        {
                            webhookReceivedDate = DateTime.UtcNow,
                            WebhookResponse = content,
                            ClientToken = responseClientToken,
                            Nonce = responseNonce
                        };
                        var chkForNonceLog = await _gatewayRequestRespLog.FindAsync(new { Nonce = responseNonce, ClientToken = responseClientToken });
                        if (chkForNonceLog != null)
                        {
                            chkForNonceLog.webhookReceivedDate = DateTime.UtcNow;
                            chkForNonceLog.WebhookResponse = content;
                            await _gatewayRequestRespLog.UpdateAsync(chkForNonceLog, new { Id = chkForNonceLog.id } );
                        }
                        else 
                        { await _gatewayRequestRespLog.AddAsync(gatewayResponse); }
                       // var existChkToken = await _gatewayCardWebhookToken.FindAsync(new { gatewayCardWebhook.cardBrand, gatewayCardWebhook.last4digits, gatewayCardWebhook.expiryMonthYear });

                        //if (existChkToken == null)
                        //{
                            var chkForNonce = await _gatewayCardWebhookToken.FindAsync(new { Nonce = responseNonce, ClientToken = responseClientToken });
                            if (chkForNonce != null)
                            {
                                chkForNonce.cardBrand = objCard.card.brand;
                                chkForNonce.expiryMonthYear = string.Concat(objCard.card.exp.month, "/", objCard.card.exp.year);
                                chkForNonce.last4digits = objCard.card.last4;
                                chkForNonce.maskedLastDigitCard = objCard.card.masked;
                                chkForNonce.nameOnCard = objCard.card.name;
                                chkForNonce.Token = objCard.card.token;
                                chkForNonce.TokenReceivedDate = DateTime.UtcNow;
                            chkForNonce.Bin = (long)Convert.ToDouble(objCard.card.bin);
                            chkForNonce.IsCardValid = isvalid;
                                await _gatewayCardWebhookToken.UpdateAsync(chkForNonce, new { Id = chkForNonce.id });
                            }
                       // }
                    }
                    return Ok(content);
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := PaymentHook)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debitUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UsersCards")]
        [Authorize]
        public async Task<IActionResult> GetUsersCards(int debitUserId,int creditUserId)
        {
            try
            {
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, _mapper.Map<List<GatewayCardWebhookTokenDto>>(await _gatewayCardWebhookToken.GetMultipleDataByConditionAsync(new { debitUserId, IsCardToSave = true,creditUserId })), 0));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersCards)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debitUserId"></param>
        /// <param name="creditUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UsersWebToken")]
        [Authorize]
        public async Task<IActionResult> GetUsersWebToken(int debitUserId, int creditUserId)
        {
            try
            {
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, await _gatewayCardWebhookToken.GetLatestWebhookToken(creditUserId, debitUserId)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UsersWebTokenByClientToken")]
      //  [Authorize]
        public async Task<IActionResult> GetUsersWebTokenByClientToken(string clientToken)
        {
            try
            {
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, await _gatewayCardWebhookToken.GetLatestWebhookTokenByClientToken(clientToken)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGatewayWebToken")]
        [Authorize]
        public async Task<IActionResult> AddWebToken(GatewayCardWebHookTokenModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var objToken = _mapper.Map<GatewayCardWebHookToken>(model);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, await _gatewayCardWebhookToken.AddAsync(objToken)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateWebToken")]
        [Authorize]
        public async Task<IActionResult> UpdateWebToken(GatewayCardWebHookTokenModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var objToken = await _gatewayCardWebhookToken.FindAsync(new { clientToken = model.ClientToken });
                if (objToken != null)
                {
                    objToken.ipgFirstTransactionId = model.ipgFirstTransactionId;
                    objToken.schemetransactionID = model.schemetransactionID;
                    await _gatewayCardWebhookToken.UpdateAsync(objToken, new { Id = objToken.id });
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, true));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGatewayWebTokenNLog")]
        [Authorize]
        public async Task<IActionResult> AddWebTokenNLog(GatewayCardWebhookNLogModel model)
        {
            try
            {
                 
                var objToken = _mapper.Map<GatewayCardWebHookToken>(model.GatewayCardWebHookTokenModel);
                await _gatewayCardWebhookToken.AddAsync(objToken).ConfigureAwait(true);
                var objTokenLog = _mapper.Map<GatewayRequestResponseLog>(model.GatewayRequestResponseLogModel);
                await _gatewayRequestRespLog.AddAsync(objTokenLog);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteGatewayWebToken")]
        [Authorize]
        public async Task<IActionResult> DeleteWebToken(GatewayCardWebHookTokenModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, await _gatewayCardWebhookToken.DeleteEntityAsync(new { clientToken = model.ClientToken })));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := DeleteWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGatewayWebLogToken")]
        [Authorize]
        public async Task<IActionResult> AddWebLogToken(GatewayRequestResponseLogModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var objToken = _mapper.Map<GatewayRequestResponseLog>(model);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, await _gatewayRequestRespLog.AddAsync(objToken)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddPaymentTransactionLog")]
        [Authorize]
        public async Task<IActionResult> AddPaymentTransactionLog(FiservPaymentTransactionLogModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var objToken = _mapper.Map<FiservPaymentTransactionLog>(model);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, await _paymentTransactionLog.AddAsync(objToken)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Fiserv := GetUsersWebToken)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

    }
}