using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Foundry.Services.PartnerNotificationsLogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static Foundry.Domain.Constants;
using System.Net.Http.Headers;
using System.Globalization;
using Foundry.Api.Models;
using System.Net;
using System.Security.Cryptography;
using foundry;
using Foundry.Services.AcquirerService;

namespace Foundry.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class I2CPushNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly II2CLogService _i2cLogRepository;
        private readonly IPartnerNotificationsLogServicer _partnerNotificationsLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly II2CAccountDetailService _i2cAccountDetailRepository;
        private readonly IGeneralSettingService _generalRepository;
        private readonly IMapper _mapper;
        private readonly ISharedJPOSService _sharedJPOSService;
        private readonly IBenefactorService _benefactor;
        private readonly IGatewayCardWebHookTokenService _gatewayCardWebhookToken;
        private readonly IAcquirerService _acquirerService;
        private readonly II2CCardBankAccountService _i2cCardBankAccountRepository;
        private readonly II2CBank2CardTransferService _i2cBank2CardTransferRepository;
        private readonly IUserTransactionInfoes _userTransactionRepository;
        private readonly IEmailService _emailService;
        private readonly IFiservMethods _fiservMethods;
        protected string NotificationEventId { get; set; }
        protected long HealthCheckId { get; set; }

        public I2CPushNotificationController(IConfiguration configuration, II2CLogService i2cLogRepository
            , II2CAccountDetailService i2cAccountDetailRepository
            , IPartnerNotificationsLogServicer partnerNotificationsLogRepository
            , IUserRepository userRepository
            , IGeneralSettingService generalRepository
            , IMapper mapper
            , ISharedJPOSService sharedJPOSService, IBenefactorService benefactor,
            IGatewayCardWebHookTokenService gatewayCardWebhookToken, IAcquirerService acquirerService,
            II2CCardBankAccountService i2CCardBankAccountService, II2CBank2CardTransferService i2CBank2CardTransferService,
            IUserTransactionInfoes userTransactionInfoes, IEmailService emailService,
            IFiservMethods fiservMethods
            )
        {
            _configuration = configuration;
            _i2cLogRepository = i2cLogRepository;
            _i2cAccountDetailRepository = i2cAccountDetailRepository;
            _partnerNotificationsLogRepository = partnerNotificationsLogRepository;
            _userRepository = userRepository;
            _generalRepository = generalRepository;
            _mapper = mapper;
            _sharedJPOSService = sharedJPOSService;
            _benefactor = benefactor;
            _gatewayCardWebhookToken = gatewayCardWebhookToken;
            _acquirerService = acquirerService;
            _i2cCardBankAccountRepository = i2CCardBankAccountService;
            _i2cBank2CardTransferRepository = i2CBank2CardTransferService;
            _userTransactionRepository = userTransactionInfoes;
            _emailService = emailService;
            _fiservMethods = fiservMethods;
        }

        [HttpPost]
        [Route(template: "Echo")]
        public async Task<IActionResult> Healthcheck()
        {
            I2CLog i2cLogReq = new I2CLog();
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8))
                {
                    var content = await reader.ReadToEndAsync();
                    dynamic response1 = JsonConvert.DeserializeObject(content);

                    HealthCheckmModel notification = response1.ToObject<HealthCheckmModel>();
                    string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                    i2cLogReq = await i2cLogRequest("Healthcheck", Request.Path, clientIpAddress, content);
                    HealthCheckId = notification.HealthCheckId;
                    if (notification.Header.Id == _configuration["I2CAcquirerIDforPushNotification"])
                    {
                        if (notification.Header.Id == _configuration["I2CAcquirerIDforPushNotification"] &&
                        notification.Header.UserId == _configuration["I2CAcquirerUserIdforPushNotification"] &&
                        notification.Header.Password == _configuration["I2CAcquirerPasswordforPushNotification"]
                        )
                        {

                            i2cLogReq.Status = "success";
                            object ret = new
                            {
                                ResponseCode = "00",
                                HealthCheckId = notification.HealthCheckId
                            };
                            string myJSON = JsonConvert.SerializeObject(ret);

                            await i2cLogResponse(i2cLogReq, myJSON);

                            return Ok(ret);


                        }
                        else
                        {
                            object ret = new
                            {
                                ResponseCode = "06",
                                HealthCheckId = notification.HealthCheckId
                            };
                            string myJSON = JsonConvert.SerializeObject(ret);


                            i2cLogReq.Status = "Failed";
                            await i2cLogResponse(i2cLogReq, myJSON);

                            return Ok(ret);
                        }
                    }
                    else
                    {
                        object ret = new
                        {
                            ResponseCode = "05",
                            HealthCheckId = notification.HealthCheckId
                        };
                        string myJSON = JsonConvert.SerializeObject(ret);


                        i2cLogReq.Status = "Failed";
                        await i2cLogResponse(i2cLogReq, myJSON);

                        return Ok(ret);
                    }

                }
            }
            catch (Exception ex)
            {

                object ret = new
                {
                    ResponseCode = "01",
                    HealthCheckId = HealthCheckId
                };
                string myJSON = JsonConvert.SerializeObject(ret);
                i2cLogReq.Status = "Failed";
                await i2cLogResponse(i2cLogReq, myJSON + "   " + ex.Message);
                return Ok(ret);
                //i2cLogReq.Status = "Failed";
                //await i2cLogResponse(i2cLogReq, ex.Message);
                //return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, true, ex.Message));
            }
        }

        [HttpPost]
        [Route(template: "Event")]
        public async Task<IActionResult> EventNotification()
        {
            I2CLog i2cLogReq = new I2CLog();
            try
            {

                using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8))
                {
                    string content = await reader.ReadToEndAsync();
                    dynamic response1 = JsonConvert.DeserializeObject(content);
                    I2CNotifyEventModel notification = response1.ToObject<I2CNotifyEventModel>();
                    string clientIpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                    i2cLogReq = await i2cLogRequest("EventNotification", Request.Path, clientIpAddress, content);
                    NotificationEventId = notification.Transaction.NotificationEventId;



                    List<GeneralSetting> ForwardSettings = await GetGeneralSettings(I2CPush.IsActive);

                    if (ForwardSettings[0].Value == "1")
                    {
                        await ForwardI2cPush(content);
                    }


                        if (!(notification.Header.Id == _configuration["I2CAcquirerIDforPushNotification"]))
                    {
                        object ret = new
                        {
                            ResponseCode = "05",
                            NotificationEventId = notification.Transaction.NotificationEventId
                        };
                        string myJSON = JsonConvert.SerializeObject(ret);
                        i2cLogReq.Status = "Failed";
                        await i2cLogResponse(i2cLogReq, myJSON);
                        return Ok(ret);
                    }
                    else
                    {
                        if (!(notification.Header.Id == _configuration["I2CAcquirerIDforPushNotification"] &&
                        notification.Header.UserId == _configuration["I2CAcquirerUserIdforPushNotification"] &&
                        notification.Header.Password == _configuration["I2CAcquirerPasswordforPushNotification"]
                        ))
                        {
                            object ret = new
                            {
                                ResponseCode = "06",
                                NotificationEventId = notification.Transaction.NotificationEventId
                            };
                            string myJSON = JsonConvert.SerializeObject(ret);
                            i2cLogReq.Status = "Failed";
                            await i2cLogResponse(i2cLogReq, myJSON);
                            return Ok(ret);
                        }




                        //When a user makes a purchase at a POS
                        if (notification.Transaction.TransactionType == "00")
                        {
                            await PostToTranlog(notification);

                            //var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { ReferenceId = notification.Card.CardReferenceID });
                            //var user = await _userRepository.GetUserById((int)userVirtualCardDetail.UserId);

                            //await sendInPersonSaleNotification(user, notification.Transaction.TransactionAmount, notification.Card.AvailableBalance);
                            await AutoReloadTrigger(notification.Card.CardReferenceID, notification.Card.AvailableBalance);
                            // await PostToTranlog(notification);
                        }
                        // When a user makes a purchase at a POS ,When a user makes a purchase in the Bite App Ex.Ordering a coffee to pickup Debit Funds API
                        else if (notification.Transaction.TransactionType == "SD")
                        {
                            ////var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { ReferenceId = notification.Card.CardReferenceID });
                            ////var user = await _userRepository.GetUserById((int)userVirtualCardDetail.UserId);

                            ////await sendOrderHeadNotification(user, notification.Transaction.TransactionAmount, notification.Card.AvailableBalance);
                            //  await PostToTranlog(notification);
                            await AutoReloadTrigger(notification.Card.CardReferenceID, notification.Card.AvailableBalance);
                        }

                        // When a User’s account balance Decrease by Admin
                        else if (notification.Transaction.TransactionType == "22")
                        {
                            await PostToTranlog(notification);
                        }

                        // When a User’s account balance increases by FISERV transaction
                        else if (notification.Transaction.TransactionType == "21")
                        {
                            await PostToTranlog(notification);
                            //var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { ReferenceId = notification.Card.CardReferenceID });
                            //var user = await _userRepository.GetUserById((int)userVirtualCardDetail.UserId);

                            //await sendAddValueNotification(user, notification.Transaction.TransactionAmount, notification.Card.AvailableBalance, SodexoBiteNotification.SodexoBiteBaseUrl);

                            //List<GeneralSetting> i2cSettings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                            //if (i2cSettings[0].Value == "1")
                            //{
                            //    await sendAddValueNotification(user, notification.Transaction.TransactionAmount, notification.Card.AvailableBalance, SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                            //}
                        }
                        //  When a User’s account balance increases by ACH Transaction
                        else if (notification.Transaction.TransactionType == "46")
                        {
                            await PostToTranlog(notification);
                            //var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { ReferenceId = notification.Card.CardReferenceID });
                            //var user = await _userRepository.GetUserById((int)userVirtualCardDetail.UserId);

                            //await sendAddValueNotification(user, notification.Transaction.TransactionAmount, notification.Card.AvailableBalance, SodexoBiteNotification.SodexoBiteBaseUrl);

                            //List<GeneralSetting> i2cSettings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                            //if (i2cSettings[0].Value == "1")
                            //{
                            //    await sendAddValueNotification(user, notification.Transaction.TransactionAmount, notification.Card.AvailableBalance, SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                            //}
                        }
                        //  When a card account is issued to a user
                        //	Transaction Type code: NC or RC or 57
                        else if (notification.Transaction.TransactionType == "NC" || notification.Transaction.TransactionType == "RC" || notification.Transaction.TransactionType == "57")
                        {

                        }
                        //When a card account is activated by a user
                        //	Transaction Type Code: D2 or S2 or AL
                        else if (notification.Transaction.TransactionType == "D2" || notification.Transaction.TransactionType == "S2" || notification.Transaction.TransactionType == "AL")
                        {
                            var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { ReferenceId = notification.Card.CardReferenceID });
                            var user = await _userRepository.GetUserById((int)userVirtualCardDetail.UserId);

                            if (notification.Card.CardStatus == "B")
                            {
                                if (userVirtualCardDetail != null && userVirtualCardDetail.Id > 0)
                                {
                                    userVirtualCardDetail.CardStatus = "1";
                                    await _i2cAccountDetailRepository.UpdateAsync(userVirtualCardDetail, new { id = userVirtualCardDetail.Id });

                                    await sendActivatedNotification(user, SodexoBiteNotification.SodexoBiteBaseUrl);
                                    List<GeneralSetting> i2cSettings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                                    if (i2cSettings[0].Value == "1")
                                    {

                                        await sendActivatedNotification(user, SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                                    }
                                }

                            }

                        }
                        // When a bank account is added for add value use by a user or benefactor
                        // Transaction Type Code: A6 or A7 or A0 or I2
                        else if (notification.Transaction.TransactionType == "A6" || notification.Transaction.TransactionType == "A7" || notification.Transaction.TransactionType == "A0" || notification.Transaction.TransactionType == "I2")
                        {

                        }


                        i2cLogReq.Status = "success";
                        object obj = new
                        {
                            ResponseCode = "00",
                            NotificationEventId = notification.Transaction.NotificationEventId
                        };
                        string myJSON1 = JsonConvert.SerializeObject(obj);

                        await i2cLogResponse(i2cLogReq, myJSON1);

                        return Ok(obj);


                    }
                }

            }
            catch (Exception ex)
            {
                object ret = new
                {
                    ResponseCode = "01",
                    NotificationEventId = NotificationEventId
                };
                string myJSON = JsonConvert.SerializeObject(ret);
                i2cLogReq.Status = "Failed";
                await i2cLogResponse(i2cLogReq, myJSON + "   " + ex.Message);
                return Ok(ret);


                //   return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, true, ex.Message));
            }

        }


        private async Task<I2CLog> i2cLogRequest(string apiName, string apiUrl, string ipAddress, string request)
        {
            I2CLog objRequestLog = new I2CLog()
            {
                UserId = null,
                AccountHolderUniqueId = null,
                ApiName = apiName,
                ApiUrl = apiUrl,
                IpAddress = ipAddress,
                Request = request,
                CreatedDate = DateTime.UtcNow
            };

            objRequestLog.Id = await _i2cLogRepository.AddAsync(objRequestLog);
            return objRequestLog;
        }

        private async Task i2cLogResponse(I2CLog objRequestLog, string xml)
        {
            objRequestLog.Response = xml;
            objRequestLog.UpdatedDate = DateTime.UtcNow;
            await _i2cLogRepository.UpdateAsync(objRequestLog, new { id = objRequestLog.Id });
        }


        //private async Task<PartnerNotificationsLog> PartnerNotificationsLogRequest(string apiName, string apiUrl, string request,int userid)
        //{
        //    PartnerNotificationsLog objRequestLog = new PartnerNotificationsLog()
        //    {
        //        UserId = null,
        //        ApiName = apiName,
        //        ApiUrl = apiUrl,
        //        Request = request,
        //        CreatedDate = DateTime.UtcNow
        //    };

        //    objRequestLog.Id = await _partnerNotificationsLogRepository.AddAsync(objRequestLog);
        //    return objRequestLog;
        //}

        //private async Task PartnerNotificationsLogResponse(PartnerNotificationsLog objRequestLog, string xml)
        //{
        //    objRequestLog.Response = xml;
        //    objRequestLog.UpdatedDate = DateTime.UtcNow;
        //    await _partnerNotificationsLogRepository.UpdateAsync(objRequestLog, new { id = objRequestLog.Id });
        //}

        private async Task sendActivatedNotification(UserDto user, string key)
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
                    var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.BitePayCardActivated);

                    string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("card_activated", hostURL.ToString(), myJSON, user.Id);
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
        //private async Task sendInPersonSaleNotification(UserDto user,string transactionamount,string availabalance)
        //{
        //    PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            object obj = new
        //            {

        //                UserObjectId = user.PartnerUserId,
        //                Date = DateTime.Now.ToString("YYYY-MM-DD"),
        //                Time = DateTime.Now.ToString("HH-mm-ss"),
        //                TransactionAmount = transactionamount,
        //                AvailableBalance = availabalance
        //            };
        //            List<GeneralSetting> i2cSettings = await GetGeneralSettings();

        //            var url = i2cSettings[0].Value;
        //            var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.InPersonSaleUrl);

        //            string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        //            PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("in_personsale", hostURL.ToString(), myJSON, user.Id);
        //            HttpContent stringContent = new StringContent(myJSON);
        //            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


        //            // var content = new StringContent(myJSON.ToString(), Encoding.UTF8, "application/json");
        //            var result = await client.PostAsync(hostURL, stringContent);
        //             PartnerNotificationsLogReq.Status = result.StatusCode.ToString();
        //            var content = await result.Content.ReadAsStringAsync();
        //            await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, content);

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        PartnerNotificationsLogReq.Status = "Error";
        //        await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, ex.Message);
        //    }



        //}

        //private async Task sendOrderHeadNotification(UserDto user, string transactionamount, string availabalance)
        //{
        //    PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            object obj = new
        //            {

        //                UserObjectId = user.PartnerUserId,
        //                Date = DateTime.Now.ToString("YYYY-MM-DD"),
        //                Time = DateTime.Now.ToString("HH-mm-ss"),
        //                TransactionAmount = transactionamount,
        //                AvailableBalance = availabalance
        //            };
        //            List<GeneralSetting> i2cSettings = await GetGeneralSettings();

        //            var url = i2cSettings[0].Value;
        //            var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.OrderAheadUrl);

        //            string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        //            PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("orderhead", hostURL.ToString(), myJSON, user.Id);
        //            HttpContent stringContent = new StringContent(myJSON);
        //            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


        //            // var content = new StringContent(myJSON.ToString(), Encoding.UTF8, "application/json");
        //            var result = await client.PostAsync(hostURL, stringContent);

        //            PartnerNotificationsLogReq.Status = result.StatusCode.ToString();
        //            var content = await result.Content.ReadAsStringAsync();
        //            await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, content);

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        PartnerNotificationsLogReq.Status = "Error";
        //        await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, ex.Message);
        //    }



        //}


        private async Task ForwardI2cPush(string contain)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    List<GeneralSetting> i2cSettings = await GetGeneralSettings(I2CPush.ForwardI2CPushUrl);

                    var url = i2cSettings[0].Value;
                    var hostURL = new Uri($"" + url );

                    HttpContent stringContent = new StringContent(contain);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // var content = new StringContent(myJSON.ToString(), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(hostURL, stringContent);
        
                }
            }
            catch (Exception ex)
            {
            }
        }

//-------------------------------------------------------------








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


        private async Task<List<GeneralSetting>> GetGeneralSettings(string Key)
        {
            var settings = await _generalRepository.GetDataAsync(new { KeyName = Key });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }

        private async Task PostToTranlog(I2CNotifyEventModel i2cNot)
        {
            TranLogDto tranlog = new TranLogDto()
            {
                Header = new Domain.Dto.Header(),

                Transaction = new Domain.Dto.Transaction()
                {
                    TransactionId = i2cNot.Transaction.TransactionId,
                    MessageType = i2cNot.Transaction.MessageType,
                    TransactionType = i2cNot.Transaction.TransactionType,
                    Date = i2cNot.Transaction.Date,
                    TransactionAmount = i2cNot.Transaction.TransactionAmount,
                    AuthorizationCode = i2cNot.Transaction.AuthorizationCode,
                    RetrievalReferenceNo = i2cNot.Transaction.RetrievalReferenceNo,
                    TransactionDescription = i2cNot.Transaction.TransactionDescription,
                    CardAcceptor = new Domain.Dto.CardAcceptor()
                    {
                        MerchantCode = i2cNot.Transaction.CardAcceptor.MerchantCode,
                        MCC = i2cNot.Transaction.CardAcceptor.MCC,
                        DeviceType = i2cNot.Transaction.CardAcceptor.DeviceType,
                        LocalDateTime = i2cNot.Transaction.CardAcceptor.LocalDateTime,
                    }

                },
                Card = new Domain.Dto.Card()
                {
                    CardReferenceID = i2cNot.Card.CardReferenceID,
                    AvailableBalance = i2cNot.Card.AvailableBalance
                }

            };

            int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.txnlog, tranlog, "", "", JPOSAPIConstants.Transactions);
        }

        private async Task AutoReloadTrigger(string CardReferenceID, string Balancestr)
        {
            //  CardReferenceID = "110625128851";
            decimal Balance = Convert.ToDecimal(Balancestr);
            var userVirtualCardDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { ReferenceId = CardReferenceID });
            var rules = await _benefactor.GetUserReloadRuleForTrigger((int)userVirtualCardDetail.UserId, Balance);
            ReloadRulesDto model = new ReloadRulesDto();
            if (rules != null)
            {


                model.ReloadAmount = rules.reloadAmount;
                model.CheckDroppedAmount = Convert.ToDecimal(rules.userDroppedAmount);
                model.CardId = rules.CardId;
                model.i2cBankAccountId = rules.i2cBankAccountId;
                model.programId = Convert.ToInt32(rules.programId);
                model.BenefactorUserId = Convert.ToInt32(rules.benefactorUserId);
                model.ReloadUserId = rules.userId;

                if (Balance <= Convert.ToDecimal(rules.userDroppedAmount))

                {
                    await ReloadAmountRequest1(userVirtualCardDetail, model);
                }

            }

            //        decimal threshholdamount = 0;
            //        decimal Balance = Convert.ToDecimal(Balancestr);
            //        var rules = rules.Where(x => Convert.ToDecimal(x.userDroppedAmount) >= Balance);
            //        if(aCount()>0)
            //        {

            //        }


            //ReloadRulesDto model = new ReloadRulesDto();
            //foreach (var item in rule)
            //{
            //    model.ReloadAmount = item.reloadAmount;
            //    model.CheckDroppedAmount = Convert.ToDecimal(item.userDroppedAmount);
            //    model.CardId = item.CardId;
            //    model.i2cBankAccountId = item.i2cBankAccountId;
            //    model.programId = Convert.ToInt32(item.programId);
            //    model.BenefactorUserId = Convert.ToInt32(item.benefactorUserId);
            //    model.ReloadUserId = item.userId;
            //    threshholdamount = Convert.ToDecimal(item.userDroppedAmount);
            //}



            //// var user = await _userRepository.GetUserById((int)userVirtualCardDetail.UserId);

            //if (Balance < threshholdamount)
            //await ReloadAmountRequest1(userVirtualCardDetail, model);


        }

        private async Task ReloadAmountRequest1(I2CAccountDetail i2CAccountDetail, ReloadRulesDto model)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    if (model.i2cBankAccountId != null)
                    {
                        //   string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataReload);

                        //   ForBankStringContentNClientRequestHeader(client, json);
                        //   var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.Bank2CardTransfer + "?creditUserId=" + model.ReloadUserId + "&debitUserId=" + model.BenefactorUserId + "&accountSrNum=" + model.i2cBankAccountId + "&amount=" + model.ReloadAmount + "&programId=" + model.programId).Result;
                        await Bank2CardTransfer(model);

                        //   await PostReloadAmountAfterTransaction(model);


                    }

                    else
                    {
                        string token = string.Empty;
                        string ipgTransactionId = string.Empty;
                        string expirydate = string.Empty;
                        string schemetransactionID = string.Empty;

                        // ForUserWebHookTokenClientRequestHeader(client1);

                        var gatewaymodel = await _gatewayCardWebhookToken.GetLatestWebhookTokenByClientToken(model.CardId);
                        GetCardWebHookToken(out token, out ipgTransactionId, out expirydate, out schemetransactionID, gatewaymodel);


                        long timestamp, nonce;
                        string apiKeyFD, apiSecretFD, clientRequestId;
                        SettingsGenerateForReloadBalance(out timestamp, out nonce, out apiKeyFD, out apiSecretFD, out clientRequestId);
                        string payloadJson, URLTransaction;
                        ModalGeneration(model, token, expirydate, ipgTransactionId, out payloadJson, out URLTransaction, schemetransactionID);

                        string messageSignature;

                        SetMessageHeadersSignature(timestamp, apiKeyFD, apiSecretFD, clientRequestId, payloadJson, out messageSignature);
                        HttpContent stringContent = StringContentSetting(payloadJson);
                        string FiservRequestCreate = string.Empty;
                        RequestHeaderSetting(client, timestamp, nonce, apiKeyFD, clientRequestId, messageSignature);

                        FiservRequestCreate = LogRequestCreateForTransaction(timestamp, nonce, apiKeyFD, clientRequestId, payloadJson, URLTransaction, messageSignature);
                        var result = await client.PostAsync(URLTransaction, stringContent).ConfigureAwait(true);
                        var transactionResult = await result.Content.ReadAsStringAsync().ConfigureAwait(true);
                        await LoggingTransaction(model, client, ipgTransactionId, FiservRequestCreate, result, transactionResult);


                        dynamic response2 = JsonConvert.DeserializeObject(transactionResult);
                        var transactionDto = response2.ToObject<FiservMainTransactionModel>();
                        string msgwithcode = "";
                        if (transactionDto.processor != null)
                        {
                            msgwithcode = "Error code - " + transactionDto.processor.responseCode + "," + transactionDto.processor.responseMessage;
                        }
                        else
                        {
                            msgwithcode = transactionDto.transactionStatus;
                        }


                        if (result.IsSuccessStatusCode)
                        {
                            //credit funds to i2c

                            // var resultcredit = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.FiservCreditFundsi2c + "?creditUserId=" + model.ReloadUserId + "&amount=" + model.ReloadAmount).Result;
                            await creditfunds(i2CAccountDetail, Convert.ToDecimal(model.ReloadAmount), transactionDto.orderId);

                            // var response = await result.Content.ReadAsAsync<ApiResponse>();
                            // if (response.StatusFlagNum == 1)
                            // {
                            //await PostReloadAmountAfterTransaction(dataReload);
                            // }
                            //  else { return ForUnsuccesfulFiservtoI2cTransfer(response); }
                        }
                        else
                        {
                            // return Json(new { Status = false, messageSignature = msgwithcode });
                        }

                    }
                }
                //   return Json(new { Status = true });
            }
            catch (Exception ex)
            {
                throw ex;
                // HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (ReloadRequest - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                //   return Json(new { Status = false });
            }

        }
        private async Task<List<GeneralSetting>> Geti2cGeneralSettings()
        {
            var settings = await _generalRepository.GetDataAsync(new { keyGroup = MessagesConstants.i2cKeyName });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }
        private async Task creditfunds(I2CAccountDetail i2cAccountDetail, decimal amount, string orderId)
        {
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
                ApiUrl = "AutoReload",
                IpAddress = "",
                Request = request.InnerXml,
                CreatedDate = DateTime.UtcNow
            };
            var loadFundResponse = await _acquirerService.CreditFunds(creditFundsTypeObj);

            if (loadFundResponse != null && loadFundResponse.CreditFundsResponse1.ResponseCode == "00")
            {
                //  await AddUserTransaction(debitUserId, amount, programId, creditUserId);
                /* Update balance in card */
                i2cAccountDetail.Balance += amount;
                await _i2cAccountDetailRepository.UpdateAsync(i2cAccountDetail, new { Id = i2cAccountDetail.Id });
                objRequestLog.Status = "Success";
                var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(loadFundResponse), "Response");
                await i2cLogResponse(objRequestLog, response.InnerXml);

                var user = await _userRepository.GetUserById((int)i2cAccountDetail.UserId);


                await sendAddValueNotification(user, amount.ToString(), amount.ToString(), SodexoBiteNotification.SodexoBiteBaseUrl);

                List<GeneralSetting> Settings = await GetGeneralSettings(SodexoBiteNotification.Is2ndUrlActiveForBite);

                if (Settings[0].Value == "1")
                {
                    await sendAddValueNotification(user, amount.ToString(), amount.ToString(), SodexoBiteNotification.SecondSodexoBiteBaseUrl);
                }

            }
            else
            {
                objRequestLog.Status = "Failed";
                var response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(loadFundResponse), "Response");
                await i2cLogResponse(objRequestLog, response.InnerXml);

                await _fiservMethods.fiservReverseTransaction(amount, orderId);

                //return BadRequest();
                // return Ok(new ApiResponse(StatusCodes.Status400BadRequest, false, MessagesConstants.SomeIssueInProcessing));
            }
            
        }
        private async Task CallMethodForLoggingNOtherDataInsertion(ReloadRulesDto model, HttpClient client, string ipgTransactionId, HttpContent stringContentLogTransaction)
        {
            await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddPaymentTransactionLog, stringContentLogTransaction);
            //   var objToken = _mapper.Map<FiservPaymentTransactionLog>(model);
            //  await _paymentTransactionLog.AddAsync(objToken);


            GatewayCardWebHookTokenModel objUpdCard = new GatewayCardWebHookTokenModel()
            {
                ipgFirstTransactionId = ipgTransactionId,
                ClientToken = model.CardId,
                schemetransactionID = model.schemetransactionID
            };
            string jsonUpdCard = Newtonsoft.Json.JsonConvert.SerializeObject(objUpdCard);
            HttpContent stringContentUpdCard = new StringContent(jsonUpdCard);
            stringContentUpdCard.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //   client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
            // await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UpdateWebToken, stringContentUpdCard);
            var objToken = await _gatewayCardWebhookToken.FindAsync(new { clientToken = model.CardId });
            if (objToken != null)
            {
                objToken.ipgFirstTransactionId = ipgTransactionId;
                objToken.schemetransactionID = model.schemetransactionID;
                await _gatewayCardWebhookToken.UpdateAsync(objToken, new { Id = objToken.id });
            }

        }
        private async Task<string> ForSuccessfulPayment(ReloadRulesDto model, HttpClient client, string ipgTransactionId, string transactionResult)
        {
            dynamic response1 = JsonConvert.DeserializeObject(transactionResult);
            var transactionDto = response1.ToObject<FiservMainTransactionModel>();
            if (transactionDto != null)
            {
                model.schemetransactionID = transactionDto.schemeTransactionId;
                ipgTransactionId = transactionDto.ipgTransactionId;
                DateTime transactionDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                // Add the timestamp (number of seconds since the Epoch) to be converted
                transactionDateTime = transactionDateTime.AddSeconds(transactionDto.transactionTime);
                transactionDto.CreditUserId = model.ReloadUserId;
                transactionDto.ProgramAccountIdSelected = 38;
                transactionDto.TransactionDateTime = transactionDateTime;
                transactionDto.TransactionPaymentGateway = "Fiserv";

            }
            /**/
            string jsonTransaction = Newtonsoft.Json.JsonConvert.SerializeObject(transactionDto);
            HttpContent stringContentTransaction = new StringContent(jsonTransaction.ToString());
            stringContentTransaction.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            var resultTransaction = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUserTransaction, stringContentTransaction);
            if (resultTransaction.IsSuccessStatusCode)
            {
                //  await PostReloadAmountAfterTransaction(model);
            }

            return ipgTransactionId;
        }


        private async Task Bank2CardTransfer(ReloadRulesDto model, DateTime? transferEndDate = null)
        {
            try
            {
                var i2cCardBankAccount = await _i2cCardBankAccountRepository.GetDataByIdAsync(new { AccountSrNo = model.i2cBankAccountId });
                var i2cAccountDetail = await _i2cAccountDetailRepository.GetDataByIdAsync(new { id = i2cCardBankAccount.I2cAccountDetailId });

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
                        Amount = Convert.ToDecimal(model.ReloadAmount),
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
                        await AddUserTransaction(model.BenefactorUserId, Convert.ToDecimal(model.ReloadAmount), model.programId, model.ReloadUserId);
                        /* Update balance in card */
                        i2cAccountDetail.Balance += model.ReloadAmount;
                        await _i2cAccountDetailRepository.UpdateAsync(i2cAccountDetail, new { Id = i2cAccountDetail.Id });
                        /* payment initiation request in db */
                        await AddBank2CardTransferRecord(Convert.ToDecimal(model.ReloadAmount), i2cCardBankAccount, i2cAccountDetail, b2cResponse);
                        response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2cResponse), "Response");
                        objRequestLog.Status = "Success";
                        await i2cLogResponse(objRequestLog, response.InnerXml);
                        // return Ok(new ApiResponse(StatusCodes.Status200OK, true, b2cResponse.B2CTransferResponse1.ResponseDesc, null, 1));
                    }
                    else
                    {
                        response = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(b2cResponse), "Response");
                        objRequestLog.Status = "Failed";
                        await i2cLogResponse(objRequestLog, response.InnerXml);
                        //  return Ok(new ApiResponse(StatusCodes.Status200OK, false, b2cResponse.B2CTransferResponse1.ResponseDesc));
                    }
                }
                else
                {
                    // return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoErrorMessagesExist));
                }
            }
            catch (Exception ex)
            {
                //  HttpContext.RiseError(new Exception(string.Concat("API := (I2CAccount := BankToCardTransfer)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                //  return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
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

        private HttpContent CheckForLogTransaction(ReloadRulesDto model, HttpClient client, string FiservRequestCreate, string transactionResult)
        {
            FiservPaymentTransactionLogModel oModelLog = new FiservPaymentTransactionLogModel()
            {
                creditUserId = model.ReloadUserId,
                debitUserId = model.BenefactorUserId,
                FiservRequestContent = FiservRequestCreate,
                FiservRequestDate = DateTime.UtcNow,
                FiservResponseContent = transactionResult,
                FiservResponseDate = DateTime.UtcNow
            };
            string jsonLogTransaction = Newtonsoft.Json.JsonConvert.SerializeObject(oModelLog);
            HttpContent stringContentLogTransaction = new StringContent(jsonLogTransaction);
            stringContentLogTransaction.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
            return stringContentLogTransaction;
        }
        private async Task<string> LoggingTransaction(ReloadRulesDto model, HttpClient client, string ipgTransactionId, string FiservRequestCreate, HttpResponseMessage result, string transactionResult)
        {
            //  var loggedId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                ipgTransactionId = await ForSuccessfulPayment(model, client, ipgTransactionId, transactionResult);
            }
            HttpContent stringContentLogTransaction = CheckForLogTransaction(model, client, FiservRequestCreate, transactionResult);
            // model.i = ipgTransactionId;
            await CallMethodForLoggingNOtherDataInsertion(model, client, ipgTransactionId, stringContentLogTransaction);
            return ipgTransactionId;
        }
        private static string LogRequestCreateForTransaction(long timestamp, long nonce, string apiKeyFD, string clientRequestId, string payloadJson, string URLTransaction, string messageSignature)
        {
            return string.Concat("Headers:{Api-key: ", apiKeyFD, ",Message-Signature: ", messageSignature, ",Nonce: ", nonce.ToString(), ",Timestamp: ", timestamp.ToString(), ",Client-Request-Id: ", clientRequestId, "},URL:{", URLTransaction, "}Body:{", payloadJson, "}");
        }
        private static HttpContent StringContentSetting(string payloadJson)
        {
            HttpContent stringContent = new StringContent(payloadJson);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            stringContent.Headers.ContentLength = payloadJson.Length;
            return stringContent;
        }

        private static void RequestHeaderSetting(HttpClient client, long timestamp, long nonce, string apiKeyFD, string clientRequestId, string messageSignature)
        {
            client.DefaultRequestHeaders.Add("Api-key", apiKeyFD);
            client.DefaultRequestHeaders.Add("Message-Signature", messageSignature);
            client.DefaultRequestHeaders.Add("Nonce", nonce.ToString());
            client.DefaultRequestHeaders.Add("Timestamp", timestamp.ToString());
            client.DefaultRequestHeaders.Add("Client-Request-Id", clientRequestId);
        }
        private void SetMessageHeadersSignature(long timestamp, string apiKeyFD, string apiSecretFD, string clientRequestId, string payloadJson, out string messageSignature)
        {
            var message = string.Concat(apiKeyFD, clientRequestId, timestamp, payloadJson);
            messageSignature = Sign(apiSecretFD, message);


        }
        #region private methods
        /// <summary>
        /// This method is used to generate signature for payment js
        /// </summary>
        /// <param name="apiSecret"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private string Sign(string apiSecret, String payload = "")
        {
            UTF8Encoding encoder = new UTF8Encoding();

            // Create signature message
            string message = payload;
            byte[] secretKeyBytes = encoder.GetBytes(apiSecret);
            byte[] messageBytes = encoder.GetBytes(message);

            // Perform hashing
            HMACSHA256 hmac = new HMACSHA256(secretKeyBytes);
            byte[] hmacBytes = hmac.ComputeHash(messageBytes);
            String hexHmac = ByteArrayToString(hmacBytes);

            // Convert to Base64
            byte[] hexBytes = encoder.GetBytes(hexHmac);
            String signature = Convert.ToBase64String(hexBytes);
            return signature;
        }
        /// <summary>
        /// This method is used to convert byte array to string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] input)
        {
            int i;
            StringBuilder output = new StringBuilder(input.Length);
            for (i = 0; i < input.Length; i++)
            {
                output.Append(input[i].ToString("x2"));
            }
            return output.ToString();
        }
        #endregion
        private void GenerateModelWIthTransactionId(ReloadRulesDto model, string token, string expirydate, string ipgTransactionId, out string payloadJson, out string URLTransaction, string schemeTransactionID)
        {
            string mon = expirydate.Split('/')[0];
            string str = expirydate.Split('/')[1];
            string year = str.Substring(str.Length - 2);
            URLTransaction = _configuration["SaleTransactionFirstData"];
            var paymentPayload = new PayloadPaymentFirst()
            {
                transactionAmount = new TransactionAmount()
                {
                    total = Convert.ToDecimal(model.ReloadAmount),
                    currency = "USD"
                },
                requestType = "PaymentTokenSaleTransaction",
                storeId = _configuration["IPGPaymentGateway:storeId"],
                paymentMethod = new PaymentMenthod()
                {
                    paymentToken = new PaymentToken()
                    {
                       // function = "DEBIT",
                        //  securityCode = model.CCCode,
                        value = token,
                        expiryDate = new ExpiryDate()
                        {
                            month = mon,
                            year = year
                        }

                    }

                },
                storedCredentials = new StoredCredentials()
                {
                    // scheduled = true,
                    scheduled = false,
                    sequence = "SUBSEQUENT",
                    referencedSchemeTransactionId = schemeTransactionID

                }
            };
            payloadJson = JsonConvert.SerializeObject(paymentPayload);
        }

        private void GenerateModalForNullTransactionId(ReloadRulesDto model, string token, string expirydate, out string payloadJson, out string URLTransaction)
        {

            string mon = expirydate.Split('/')[0];
            string str = expirydate.Split('/')[1];
            string year = str.Substring(str.Length - 2);
            URLTransaction = _configuration["SaleTransactionFirstData"];
            var paymentPayload = new PayloadPaymentFirst()
            {
                transactionAmount = new TransactionAmount()
                {
                    total = Convert.ToDecimal(model.ReloadAmount),
                    currency = "USD"
                },
                requestType = "PaymentTokenSaleTransaction",
                storeId = _configuration["IPGPaymentGateway:storeId"],
                paymentMethod = new PaymentMenthod()
                {
                    paymentToken = new PaymentToken()
                    {
                       // function = "DEBIT",
                        value = token,
                        expiryDate = new ExpiryDate()
                        {
                            month = mon,
                            year = year
                        }

                    }

                },
                storedCredentials = new StoredCredentials()
                {
                    // scheduled = true,
                    scheduled = false,
                    sequence = "FIRST",
                }
            };
            payloadJson = JsonConvert.SerializeObject(paymentPayload);
        }

        private void ModalGeneration(ReloadRulesDto model, string token, string expirydate, string ipgTransactionId, out string payloadJson, out string URLTransaction, string schemeTransactionID)
        {
            payloadJson = string.Empty;
            URLTransaction = string.Empty;

            if (!string.IsNullOrEmpty(ipgTransactionId))
            {
                GenerateModelWIthTransactionId(model, token, expirydate, ipgTransactionId, out payloadJson, out URLTransaction, schemeTransactionID);
            }
            else
            {
                GenerateModalForNullTransactionId(model, token, expirydate, out payloadJson, out URLTransaction);

            }
        }

        private void SettingsGenerateForReloadBalance(out long timestamp, out long nonce, out string apiKeyFD, out string apiSecretFD, out string clientRequestId)
        {
            Random generator = new Random();
            var num = generator.Next(0, 9999).ToString("D" + 4);
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            nonce = timestamp + Convert.ToInt16(num);
            apiKeyFD = _configuration["credentials:apiKey"];
            apiSecretFD = _configuration["credentials:apiSecret"];
            clientRequestId = Guid.NewGuid().ToString();
        }
        private static void GetCardWebHookToken(out string token, out string ipgTransactionId, out string expirydate, out string schemetransactionID, GatewayCardWebhookTokenDto CardDetail)
        {

            token = CardDetail?.Token.ToString();
            ipgTransactionId = CardDetail?.ipgFirstTransactionId?.ToString();
            expirydate = CardDetail?.expiryMonthYear?.ToString();
            schemetransactionID = CardDetail?.schemetransactionID?.ToString();

        }
        private async Task PostReloadAmountAfterTransaction(ReloadRulesDto dataReload)
        {
            using (var client1 = new HttpClient())
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(dataReload);

                HttpContent stringContent1 = new StringContent(json1.ToString());
                stringContent1.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                // client1.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                //  var result1 = await client1.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ReloadAmount, stringContent1);
                //  if (result1.IsSuccessStatusCode)
                //// {
                //  var response1 = await result1.Content.ReadAsAsync<ApiResponse>();
                //   return Json(new { Status = true, Message = response1.Message });
                //  }
                // return "false";
                // return Json(new { Status = false, Message = "" });
                //   await ReloadBalanceUser(dataReload);
            }
        }

        //public async Task ReloadBalanceUser(ReloadRulesDto model)
        //{
        //    try
        //    {
        //        model.BenefactorUserId = model.BenefactorUserId;
        //        var reloadRequest = await _benefactor.ReloadUserBalance(model);
        //        if (reloadRequest > 0)
        //        {
        //            try
        //            {
        //                var users = await _userRepository.GetUsersDetailByIds(new List<int> { model.ReloadUserId, model.BenefactorUserId });
        //                var userDetail = users.FirstOrDefault(x => x.Id == model.ReloadUserId);
        //                var benefactor = users.FirstOrDefault(x => x.Id == model.BenefactorUserId);
        //                /* Email Sent */

        //                var template = await _emailService.GetEmailTemplateByName(EmailTemplates.BalanceReload);
        //                template.Subject = template.Subject.Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName));
        //                template.Body = template.Body.Replace("Trove", "Sodexo");
        //                template.Body = template.Body.Replace("background:linear-gradient(208.63deg, #3952d5 0%, rgba(20,4,185,0.8) 51.1%, #3a55d7 100%)", "");
        //                template.Body = template.Body.Replace("{LogoImage}", string.Concat(_configuration["ServiceAPIURL"], "Images", "/email-logo.png")).Replace("{Name}", string.Concat(userDetail?.FirstName, " ", userDetail?.LastName)).Replace("{SenderName}", string.Concat(benefactor?.FirstName, " ", benefactor?.LastName)).Replace("{Amount}", model.ReloadAmount.ToString());
        //                await _emailService.SendEmail(benefactor?.Email, template.Subject, template.Body, template.CCEmail, template.BCCEmail);
        //            }
        //            catch (Exception ex)
        //            {
        //                //HttpContext.RiseError(new Exception(string.Concat("API := (Account := ReloadBalanceUser)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
        //            }

        //           // return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ReloadBalanceSuccessfully, model.ReloadUserId, 1));
        //        }
        //       // return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
        //    }
        //    catch (Exception ex)
        //    {
        //      //  HttpContext.RiseError(new Exception(string.Concat("API := (Benefactor := ReloadBalanceUser) : ", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
        //       // return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
        //    }
        //}


        //public async Task<int> ReloadUserBalance(ReloadRulesDto model)
        //{
        //    try
        //    {
        //        ReloadRequestModel mo = new ReloadRequestModel();
        //        mo.re
        //        if (model.ReloadRequestId.HasValue && model.ReloadRequestId > 0)
        //        {
        //            var reloadRequest = await _reloadRequest.GetSingleDataByConditionAsync(new { id = model.ReloadRequestId, isRequestAccepted = false, isDeleted = false });
        //            if (reloadRequest != null)
        //            {
        //                reloadRequest.isRequestAccepted = true;
        //                reloadRequest.programId = model.ProgramId;
        //                reloadRequest.modifiedBy = model.BenefactorUserId;
        //                reloadRequest.modifiedDate = DateTime.UtcNow;
        //                await _reloadRequest.UpdateAsync(reloadRequest, new { Id = reloadRequest.id });
        //            }
        //        }
        //        var checkAutoReload = await GetReloadRuleOfUser(model.ReloadUserId, model.BenefactorUserId).ConfigureAwait(false);
        //        int reloadruleId = 0;
        //        if (model.IsAutoReload)
        //        {
        //            if (checkAutoReload == null) { checkAutoReload = new ReloadRules(); }
        //            else { reloadruleId = checkAutoReload.id; }
        //            checkAutoReload.userId = model.ReloadUserId;
        //            checkAutoReload.benefactorUserId = model.BenefactorUserId;
        //            checkAutoReload.isAutoReloadAmount = model.IsAutoReload;
        //            checkAutoReload.modifiedDate = DateTime.UtcNow;
        //            checkAutoReload.programId = model.ProgramId;
        //            checkAutoReload.reloadAmount = model.AutoReloadAmount;
        //            checkAutoReload.userDroppedAmount = model.CheckDroppedAmount;
        //            checkAutoReload.isActive = true;
        //            checkAutoReload.isDeleted = false;

        //            if (reloadruleId > 0) { await _reloadRule.UpdateAsync(checkAutoReload, new { Id = reloadruleId }); } else { await _reloadRule.AddAsync(checkAutoReload); }

        //        }

        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //private IActionResult ForUnsuccesfuleBankTransfer(ApiResponse response)
        //{
        //    // return Json(new { Status = false, Message = response.Message });
        //    return response.Message.ToString();
        //}
        //private void ForUserWebHookTokenClientRequestHeader(HttpClient client1)
        //{
        //    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    client1.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
        //}

        //private void ForBankStringContentNClientRequestHeader(HttpClient client, string json)
        //{
        //    HttpContent stringContent = new StringContent(json);
        //    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
        //}


    }
}