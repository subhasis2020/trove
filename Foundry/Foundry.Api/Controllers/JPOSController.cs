using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This class API is used to contain all the methods with JPOS call.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JposController : ControllerBase
    {
        #region Private Variables
        private readonly IUserRepository _userRepository;
        private readonly IPrograms _programRepository;
        private readonly IGeneralSettingService _setting;
        private string consumerId = "";
        private string version = "";
        private string url = "";
        private string n = "";
        #endregion

        #region Constructor
        /// <summary>
        /// JposController
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="setting"></param>
        /// <param name="programReository"></param>
        public JposController(IUserRepository userRepository, IGeneralSettingService setting, IPrograms programReository)
        {
            _userRepository = userRepository;
            _setting = setting;
            _programRepository = programReository;
        }
        #endregion

        #region API Methods
        /// <summary>
        /// This Api is used to get user QRCode from JPOS.
        /// </summary>
        /// <returns></returns>
        [Route("QRCode")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetQRCode()
        {
            try
            {
                var userIdClaimForQRCode = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForQRCode = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var jposAccountIdForQRCode = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "jposAccountHolderId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForQRCode, sessionIdClaimForQRCode)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                using (var client = new HttpClient())
                {
                    var jposSettingsForQR = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsKeyGroup.JPOS_Version0)).ToList();
                    if (jposSettingsForQR.Count > 0)
                    {
                        foreach (var singleitem in jposSettingsForQR)
                        {
                            switch (singleitem.KeyName)
                            {
                                case Constants.JPOSConstants.JPOS_Version:
                                    version = singleitem.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_ConsumerId:
                                    consumerId = singleitem.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_HostURL:
                                    url = singleitem.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_N:
                                    n = singleitem.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);

                        var hostURLForQRCode = new Uri($"" + url + "wallets/" + jposAccountIdForQRCode + "/token");
                        var response = await client.GetAsync(hostURLForQRCode);
                        string jsonForQRCode;
                        using (var content = response.Content)
                        {
                            jsonForQRCode = await content.ReadAsStringAsync();
                            dynamic response1 = JsonConvert.DeserializeObject(jsonForQRCode.ToString());
                            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.JPOSQRCodeCreatedSuccessfully, response1));
                        }
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.JPOSQRCodeUnsuccessfulCreation, null));
                }

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (JPOS := GetQRCode)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get user balance from JPOS.
        /// </summary>
        /// <returns></returns>
        [Route("JPOSUserBalance")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetJPOSUserBalance()
        {
            try
            {
                var userIdClaimForUserBalance = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForUserBalance = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var jposAccountIdForUserBalance = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "jposAccountHolderId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForUserBalance, sessionIdClaimForUserBalance)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                using (var client = new HttpClient())
                {
                    var jposSettingsForUserBalance = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsKeyGroup.JPOS_Version0)).ToList();
                    if (jposSettingsForUserBalance.Count > 0)
                    {
                        foreach (var itemForJPOS in jposSettingsForUserBalance)
                        {
                            switch (itemForJPOS.KeyName)
                            {
                                case Constants.JPOSConstants.JPOS_Version:
                                    version = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_ConsumerId:
                                    consumerId = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_HostURL:
                                    url = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_N:
                                    n = itemForJPOS.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);

                        var hostURLForUserBalance = new Uri($"" + url + "wallets/" + jposAccountIdForUserBalance + "/balance?layers=840,1840");
                        var response = await client.GetAsync(hostURLForUserBalance);
                        string jsonForUserBalance;
                        using (var content = response.Content)
                        {
                            jsonForUserBalance = await content.ReadAsStringAsync();
                            dynamic response1 = JsonConvert.DeserializeObject(jsonForUserBalance.ToString());
                            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.JPOSBalanceReturnedSuccessfully, response1));
                        }
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.JPOSBalanceUnsuccessfuleturn, null));
                }

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (JPOS := JPOSUserBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get user transactions from JPOS.
        /// </summary>
        /// <returns></returns>
        [Route("JPOSUserTransactions")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetJPOSUserTransactions()
        {
            try
            {
                var userIdClaimForUserTransactions = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForUserTransactions = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var jposAccountIdForUserTransactions = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "jposAccountHolderId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForUserTransactions, sessionIdClaimForUserTransactions)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                using (var client = new HttpClient())
                {
                    var jposSettings = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsKeyGroup.JPOS_Version0)).ToList();
                    if (jposSettings.Count > 0)
                    {
                        foreach (var itemJPOStrans in jposSettings)
                        {
                            switch (itemJPOStrans.KeyName)
                            {
                                case Constants.JPOSConstants.JPOS_Version:
                                    version = itemJPOStrans.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_ConsumerId:
                                    consumerId = itemJPOStrans.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_HostURL:
                                    url = itemJPOStrans.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_N:
                                    n = itemJPOStrans.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);

                        var hostURLForUserTransactions = new Uri($"" + url + "wallets/" + jposAccountIdForUserTransactions + "/detail?layers=840,1840");
                        var responseForUserTransactions = await client.GetAsync(hostURLForUserTransactions);
                        string jsonForUserTransactions;
                        using (var content = responseForUserTransactions.Content)
                        {
                            jsonForUserTransactions = await content.ReadAsStringAsync();
                            dynamic responseOfUserTransactions = JsonConvert.DeserializeObject(jsonForUserTransactions.ToString());
                            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.JPOSTransactionReturnedSuccessfully, responseOfUserTransactions));
                        }
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.JPOSTransactionUnsuccessfuleturn, null));
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (JPOS := GetJPOSUserTransactions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }
        /// <summary>
        /// This Api is used to get user balance from JPOS.
        /// </summary>
        /// <returns></returns>
        [Route("GetJPOSBiteUserBalance")]
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetJPOSBiteUserBalance(int UserId)
        {
            try
            {
                var userdetail = await _userRepository.GetUserInfoById(UserId);

                using (var client = new HttpClient())
                {
                    var jposSettingsForUserBalance = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsKeyGroup.JPOS_Staging)).ToList();
                    if (jposSettingsForUserBalance.Count > 0)
                    {
                        foreach (var itemForJPOS in jposSettingsForUserBalance)
                        {
                            switch (itemForJPOS.KeyName)
                            {
                                case Constants.JPOSConstants.JPOS_Version:
                                    version = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_ConsumerId:
                                    consumerId = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_HostURL:
                                    url = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_N:
                                    n = itemForJPOS.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    var usrBitePayBalance = new JPOSBiteBalanceApiModel();

                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);

                        var hostURLForUserBalance = new Uri($"" + url + "wallets/" + userdetail?.PartnerUserId + "/balance");
                        var response = await client.GetAsync(hostURLForUserBalance);
                        string jsonForUserBalance;
                        using (var content = response.Content)
                        {
                            jsonForUserBalance = await content.ReadAsStringAsync();
                            dynamic response1 = JsonConvert.DeserializeObject(jsonForUserBalance.ToString());
                            usrBitePayBalance = response1.ToObject<JPOSBiteBalanceApiModel>();
                            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.JPOSBalanceReturnedSuccessfully, response1));
                        }
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.JPOSBalanceUnsuccessfuleturn, null));
                }

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (JPOS := JPOSUserBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProgramId"></param>
        /// <returns></returns>
        [Route("GetIssuerProperties")]
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetIssuerProperties(int ProgramId)
        {
            try
            {
                // var userdetail = await _userRepository.GetUserInfoById(ProgramId);

                using (var client = new HttpClient())
                {
                    var jposSettingsForUserBalance = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsKeyGroup.JPOS_Staging)).ToList();
                    if (jposSettingsForUserBalance.Count > 0)
                    {
                        foreach (var itemForJPOS in jposSettingsForUserBalance)
                        {
                            switch (itemForJPOS.KeyName)
                            {
                                case Constants.JPOSConstants.JPOS_Version:
                                    version = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_ConsumerId:
                                    consumerId = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_HostURL:
                                    url = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_N:
                                    n = itemForJPOS.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);

                        var hostURLIssuerProperties = new Uri($"" + url + "issuers/" + ProgramId + "?detail=true");
                        var response = await client.GetAsync(hostURLIssuerProperties);
                        string jsonIssuerProperties;
                        using (var content = response.Content)
                        {
                            jsonIssuerProperties = await content.ReadAsStringAsync();
                            dynamic serviceresponse = JsonConvert.DeserializeObject(jsonIssuerProperties.ToString());
                            var issuerDetails = serviceresponse.ToObject<IssuerDetails>();
                            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.JPOSIssuredPropertiesReturnedSuccessfully, serviceresponse));
                        }
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.JPOSIssuredPropertiesReturnedUnsuccessfully, null));
                }

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (JPOS := JPOSUserBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// 
        /// </summary>       
        /// <returns></returns>
        [Route("UpdateIssuer")]
        // [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateIssuer()
        {
            try
            {
                // var userdetail = await _userRepository.GetUserInfoById(ProgramId);

                using (var client = new HttpClient())
                {
                    var jposSettingsForUserBalance = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.GeneralSettingsKeyGroup.JPOS_Staging)).ToList();
                    if (jposSettingsForUserBalance.Count > 0)
                    {
                        foreach (var itemForJPOS in jposSettingsForUserBalance)
                        {
                            switch (itemForJPOS.KeyName)
                            {
                                case Constants.JPOSConstants.JPOS_Version:
                                    version = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_ConsumerId:
                                    consumerId = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_HostURL:
                                    url = itemForJPOS.Value;
                                    break;
                                case Constants.JPOSConstants.JPOS_N:
                                    n = itemForJPOS.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);

                        var hostURLIssuerProperties = new Uri($"" + url + "issuers");
                         var response = await client.GetAsync(hostURLIssuerProperties);
                        string jsonIssuerProperties;
                        using (var content = response.Content)
                        {
                            jsonIssuerProperties = await content.ReadAsStringAsync();
                            dynamic serviceresponse = JsonConvert.DeserializeObject(jsonIssuerProperties.ToString());

                            Issuers issuerDetails = serviceresponse.ToObject<Issuers>();
                            foreach (var issuer in issuerDetails.issuer)
                            {
                                await _programRepository.RefreshPrograms(issuer.organizationName, int.Parse(issuer.id), 
                                    int.Parse(issuer.id), int.Parse(issuer.id), 1, issuer.name, issuer.startDate, issuer.endDate);
                            }


                            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.JPOSIssuredPropertiesReturnedSuccessfully, serviceresponse));
                        }
                    }
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.JPOSIssuredPropertiesReturnedUnsuccessfully, null));
                }

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (JPOS := JPOSUserBalance)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        #endregion
    }
}