using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Foundry.Domain.Constants;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class is used to get all the action methods for benefactor.
    /// </summary>
    [ServiceFilter(typeof(CustomActionAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class BenefactorController : Controller
    {

        private readonly IConfiguration _configuration;
        /// <summary>
        /// This constructor is used to inject the services.
        /// </summary>
        /// <param name="configuration"></param>
        public BenefactorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// This method is used to get the notifications to the user in popup window in user interface.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Benefactor, Super Admin")]
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            List<BenefactorNotificationsModel> linkedNotifications = new List<BenefactorNotificationsModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.BenefactorNotifications + "?benefactorId=" + Convert.ToInt32(Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value))).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            linkedNotifications = response1.ToObject<List<BenefactorNotificationsModel>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetNotifications - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = linkedNotifications });
        }
        /// <summary>
        /// This method is used to show the default action method of the controller.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Benefactor")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method is used to show the dashboard of the user.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Benefactor")]
        public IActionResult Dashboard()
        {
            try
            {
                ViewBag.Linked = !string.IsNullOrEmpty(Request.Query["linked"].ToString()) ? Request.Query["linked"].ToString() : "1";
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (Dashboard - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }
        /// <summary>
        /// This method is used to show the transaction page of the benefactor.
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [Authorize(Roles = "Benefactor")]
        public async Task<IActionResult> Transactions()
        {
            List<LinkedUsersModel> linkedUsers = new List<LinkedUsersModel>();
            try
            {
                linkedUsers = await GetLinkedUserOfBenefactor(true).ConfigureAwait(false);
                ViewBag.CurrentPeriod = DateTime.Now.ToString("MMMM yyyy");
                var monthsBetweenDates = Enumerable.Range(0, 12)
                                           .Select(i => DateTime.Now.AddMonths(-i))
                                           .OrderByDescending(e => e.Year).Skip(1)
                                           .AsEnumerable();

                ViewBag.MonthYear = monthsBetweenDates.Select(e => e.ToString("MMMM yyyy")).ToArray();
                ViewBag.UserId = linkedUsers.Select(x => x.linkedUserId).FirstOrDefault();
                ViewBag.CanViewTransaction = linkedUsers.Select(x => x.CanViewTransaction).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (Transactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View(linkedUsers);
        }

        /// <summary>
        /// This method is used to show the linked accounts page for the user.
        /// </summary>
        /// <param name="islink"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [Authorize(Roles = "Benefactor")]
        public async Task<IActionResult> LinkedAccounts(bool islink, int id)
        {
            List<LinkedUsersModel> linkedUsers = new List<LinkedUsersModel>();
            try
            {
                var benefactorId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.LinkedUsersInformation + "?benefactorId=" + Convert.ToInt32(benefactorId) + "&islink=" + islink + "&id=" + id).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            linkedUsers = response1.ToObject<List<LinkedUsersModel>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (LinkedAccounts - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View(linkedUsers);
        }

        /// <summary>
        /// This method is used to show the page to mobile users for reload money.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reloadRequestId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Basic User")]
        public async Task<IActionResult> UserReloadAmout(int id, int reloadRequestId, int programId)
        {
            try
            {
                List<TranlogViewModel> model = new List<TranlogViewModel>();
                if (TempData["Message"] != null && TempData["Status"] != null && TempData["ReloadedUserId"] != null)
                {
                    ViewBag.StatusResponse = TempData["Status"];
                    ViewBag.MessageResponse = TempData["Message"];
                    id = Convert.ToInt32(TempData["ReloadedUserId"]);
                    TempData["ReloadedUserId"] = null;
                    TempData["Message"] = null;
                    TempData["Status"] = null;
                }
                List<LinkedUsersModel> linkedUsers = new List<LinkedUsersModel>();
                var loggedId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                if (User.IsInRole(Roles.Benefactor))
                {

                    linkedUsers = await GetLinkedUserOfBenefactor(false).ConfigureAwait(false);
                    ViewBag.UserId = id == 0 ? linkedUsers.Select(x => x.linkedUserId).FirstOrDefault() : id;
                }
                else
                {
                    ViewBag.UserId = Convert.ToInt32(loggedId);
                }
                ViewBag.LoggedUserId = Convert.ToInt32(loggedId);
                ViewBag.ReloadRequestId = reloadRequestId;
                programId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                if (programId <= 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);


                        var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserProgramByUserId + "?userId=" + loggedId).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            if (response.Status)
                                programId = Convert.ToInt32(response.Result.ToString());

                        }
                    }


                   
                }
                //max balance check
                int uid = Convert.ToInt32(loggedId);
                using (var client1 = new HttpClient())
                {
                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client1.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    decimal maxaccessiblebal = Convert.ToDecimal(_configuration["MaxAccessibleFunds"]);
                    //decimal startingbalance = 0;
                    string Amounttoadd = "0";
                    var usertranlogdata = client1.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GettranlogData + "?id=" + uid).Result;
                    //DateTime currentdate = DateTime.Now;

                    if (usertranlogdata.IsSuccessStatusCode)
                    {
                        var response = await usertranlogdata.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum != 0)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            model = response1.ToObject<List<TranlogViewModel>>();
                            foreach (var obj in model)
                            {

                                Amounttoadd = obj.total;
                                ViewBag.Amounttoadd = Amounttoadd;
                            }
                        }
                        else
                        {
                            ViewBag.Amounttoadd = maxaccessiblebal;
                        }
                    }
                }
                ViewBag.ProgramId = programId;
                ViewBag.UserType = User.Claims.FirstOrDefault(x => x.Type.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()).Value.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim().ToLower();
                ViewBag.FirstDatajsDocURL = _configuration["FirstDatajsDocURL"].ToString();
                ViewData["LinkedUsers"] = linkedUsers;
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (ReloadRequest - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }
        /// <summary>
        /// This method is used to show the reload request money page to the benefactor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reloadRequestId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [Authorize(Roles = "Benefactor")]
        public async Task<IActionResult> ReloadRequest(int id, int reloadRequestId, int programId)
        {
            try
            {
                List<TranlogViewModel> model = new List<TranlogViewModel>();
                var usrAvailableBalance = new List<UserAvailableBalanceDto>();
                if (TempData["Message"] != null && TempData["Status"] != null && TempData["ReloadedUserId"] != null)
                {
                    ViewBag.StatusResponse = TempData["Status"];
                    ViewBag.MessageResponse = TempData["Message"];
                    id = Convert.ToInt32(TempData["ReloadedUserId"]);
                    TempData["ReloadedUserId"] = null;
                    TempData["Message"] = null;
                    TempData["Status"] = null;
                }

                var loggedId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;

                var linkedUsers = await GetLinkedUserOfBenefactor(false);
                ViewBag.UserId = id == 0 ? linkedUsers.Select(x => x.linkedUserId).FirstOrDefault() : id;
                ViewBag.LoggedUserId = Convert.ToInt32(loggedId);
                ViewBag.ReloadRequestId = reloadRequestId;
                // if (programId <= 0)  //commneted check for issue occur notification reload balance click
                // {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserProgramByUserId + "?userId=" + (id == 0 ? linkedUsers.Select(x => x.linkedUserId).FirstOrDefault() : id)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                            programId = Convert.ToInt32(response.Result.ToString());
                    }
                }
                //max balance check
                int uid = ViewBag.UserId;
                using (var client1 = new HttpClient())
                {
                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client1.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    decimal maxaccessiblebal = Convert.ToDecimal(_configuration["MaxAccessibleFunds"]);
                    //decimal startingbalance = 0;
                    string Amounttoadd = "0";
                    var usertranlogdata = client1.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GettranlogData + "?id=" + uid).Result;
                    //DateTime currentdate = DateTime.Now;

                    if (usertranlogdata.IsSuccessStatusCode)
                    {
                        var response = await usertranlogdata.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum != 0)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            model = response1.ToObject<List<TranlogViewModel>>();
                            foreach (var obj in model)
                            {
                                //if (currentdate > obj.date)
                                //{
                                //    startingbalance = obj.dsbalance;
                                //}
                                //else
                                //{
                                //    startingbalance = obj.dsbalance;
                                //}
                                // startingbalance = obj.dsbalance;
                                Amounttoadd = obj.total;
                                ViewBag.Amounttoadd = Amounttoadd;
                            }
                        }
                        else
                        {
                            ViewBag.Amounttoadd = maxaccessiblebal;
                        }
                    }
                        }
                    ViewBag.UserType = User.Claims.FirstOrDefault(x => x.Type.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()).Value.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim().ToLower();
                   ViewBag.FirstDatajsDocURL = _configuration["FirstDatajsDocURL"].ToString();

                    ViewBag.ProgramId = programId;
                    ViewData["LinkedUsers"] = linkedUsers;

                List<SelectListItem> ddlUsers = linkedUsers.AsEnumerable().Select(c => new SelectListItem { Text = string.Concat(c.UserFirstName, " ", c.UserLastName), Value = c.linkedUserId.ToString() }).ToList();
                ViewBag.LinkedUsers = ddlUsers;
            }
          //  }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (ReloadRequest - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }

        [Authorize(Roles = "Benefactor")]
        public async Task<IActionResult> GetLinkedUserDetails(int id)
        {
            try
            {
                int programId = 0;
                ReloadRuleDto rule = new ReloadRuleDto();
                List<TranlogViewModel> model = new List<TranlogViewModel>();
                var usrAvailableBalance = new List<UserAvailableBalanceDto>();
                var userBankDetails1 = new List<BankAccountModel>();
                var userBankDetails = new List<I2CCardBankAccountModel>();
                var cardToken = new List<GatewayCardWebhookTokenDto>();
                var programAccountsUser = new List<AccountListingDto>();
                List<LinkedUsersModel> linkedUsers = new List<LinkedUsersModel>();
                var loggedId = Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value;
                //max balance check
            
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    decimal maxaccessiblebal = Convert.ToDecimal(_configuration["MaxAccessibleFunds"]);
                    //decimal startingbalance = 0;
                    string Amounttoadd = "0";
                    var usertranlogdata = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GettranlogData + "?id=" + id).Result;
                    //DateTime currentdate = DateTime.Now;

                    if (usertranlogdata.IsSuccessStatusCode)
                    {
                        var response = await usertranlogdata.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum != 0)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            model = response1.ToObject<List<TranlogViewModel>>();
                            foreach (var obj in model)
                            {

                                Amounttoadd = obj.total;
                                ViewBag.Amounttoadd = Amounttoadd;
                            }
                        }
                        else
                        {
                            ViewBag.Amounttoadd = maxaccessiblebal;
                        }
                    }


                    var result3 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserProgramByUserId + "?userId=" + id).Result;
                    if (result3.IsSuccessStatusCode)
                    {
                        var response = await result3.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                            programId = Convert.ToInt32(response.Result.ToString());
                    }

                    var result2 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.i2cGetBalance + "?id=" + Convert.ToInt32(id)).Result;
                    var result1 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserAvailableBalanceByUserId + "?userId=" + Convert.ToInt32(id) + "&programId=" + programId).Result;
                    //   

                    if (result1.IsSuccessStatusCode)
                    {
                        var response = await result1.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        usrAvailableBalance = response1.ToObject<List<UserAvailableBalanceDto>>();
                        ViewBag.userCurrentBalance = usrAvailableBalance[2].DataValue;
                    }

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ReloadRuleUser + "?userId=" + Convert.ToInt32(id) + "&benefactorId=" + Convert.ToInt32(loggedId)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            rule = response1.ToObject<ReloadRuleDto>();

                        }

                    }
                }
           
                if (User.IsInRole(Roles.Benefactor))
                {
                    // ViewBag.UserId = id == 0 ? linkedUsers.Select(x => x.linkedUserId).FirstOrDefault() : id;
                }
                else
                {
                    ViewBag.UserId = Convert.ToInt32(loggedId);
                }
                ViewBag.ReloadRequestId = loggedId;
                ViewBag.ProgramId = programId;
                ViewBag.LoggedUserId = Convert.ToInt32(loggedId);
                List<SelectListItem> ddlProgramAccounts = programAccountsUser.AsEnumerable().Select(c => new SelectListItem { Text = string.Concat(c.AccountName, " (", c.AccountType, ")"), Value = c.Id.ToString() }).ToList();
                ViewBag.ProgramAccountsUser = ddlProgramAccounts;

                //bind bankdropdownlist
                List<SelectListItem> ddlBanklist = new List<SelectListItem>();


                if (userBankDetails.Any())
                {
                    for (var i = 0; i < userBankDetails.Count; i++)
                    {
                        var addBank = new SelectListItem();                     
                        addBank.Value = userBankDetails[i].IdValue;
                        addBank.Text = userBankDetails[i].CardBankName;
                        ddlBanklist.Add(addBank);
                    }


                }
                var text = new SelectListItem();
                text.Text = "+ add payment method";
                text.Value = "seladd";
                ddlBanklist.Add(text);
                ViewBag.UserBankAccounts = ddlBanklist;



                List<SelectListItem> ddlCardList = cardToken.AsEnumerable().Select(c => new SelectListItem { Text = string.Concat(c.nickName, " (", c.maskedLastDigitCard, ")"), Value = c.ClientToken }).ToList();
                ViewBag.UserCardList = ddlCardList;
                return Json(new { balance = ViewBag.userCurrentBalance, data=rule});

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor/User (BalanceReload - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { balance = ViewBag.userCurrentBalance, data = "error" });
            }
            
        }
        public async Task<string> CheckIpgTransactionId(string clientToken)
        {
            string token = string.Empty;
            string ipgTransactionId = string.Empty;
            string expirydate = string.Empty;
            string schemetransactionID = string.Empty;
            using (var client1 = new HttpClient())
            {
                //  ForUserWebHookTokenClientRequestHeader(client1);

                var resultToken = client1.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersWebTokenByClientToken + "?clientToken=" + clientToken).Result;
                if (resultToken.IsSuccessStatusCode)
                {
                    var response = await resultToken.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    GetCardWebHookToken(out token, out ipgTransactionId, out expirydate,out schemetransactionID, response1);
                    ViewBag.IpgId = ipgTransactionId;
                }
            }
            return ipgTransactionId;
        }
       
        /// <summary>
        /// This method is used to reload amount request post data to validate and do the credit of the amount to the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [HttpPost]
        [Authorize(Roles = "Benefactor, Basic User")]
        public async Task<IActionResult> ReloadAmountRequest(Foundry.Web.Models.ReloadRequestModel model)
        {
            try
            {
                int loggedId;
                ReloadRequestDto dataReload;
                ReloadModalGenerate(model, out loggedId, out dataReload);


                using (var client = new HttpClient())
                {
                    if (model.IsPaymentViaBank)
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataReload);

                        ForBankStringContentNClientRequestHeader(client, json);
                        var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.Bank2CardTransfer + "?creditUserId=" + model.ReloadUserId + "&debitUserId=" + loggedId + "&accountSrNum=" + model.AccountReloadSrNo + "&amount=" + model.ReloadAmount + "&programId=" + model.ProgramId).Result;

                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                await PostReloadAmountAfterTransaction(dataReload);
                            }
                            else { return ForUnsuccesfuleBankTransfer(response); }
                        }
                    }
                    else
                    {
                        string token = string.Empty;
                        string ipgTransactionId = string.Empty;
                        string expirydate = string.Empty;
                        string schemetransactionID = string.Empty;
                        using (var client1 = new HttpClient())
                        {
                            ForUserWebHookTokenClientRequestHeader(client1);

                            var resultToken = client1.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersWebTokenByClientToken + "?clientToken=" + model.clientTokenPG).Result;
                            if (resultToken.IsSuccessStatusCode)
                            {
                                var response = await resultToken.Content.ReadAsAsync<ApiResponse>();
                                dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                                GetCardWebHookToken(out token, out ipgTransactionId,out expirydate,out schemetransactionID, response1);
                            }
                        }
                        long timestamp, nonce;
                        string apiKeyFD, apiSecretFD, clientRequestId;
                        SettingsGenerateForReloadBalance(out timestamp, out nonce, out apiKeyFD, out apiSecretFD, out clientRequestId);
                        string payloadJson, URLTransaction;
                        ModalGeneration(model, token, expirydate, ipgTransactionId, out payloadJson, out URLTransaction);

                        string messageSignature;

                        SetMessageHeadersSignature(timestamp, apiKeyFD, apiSecretFD, clientRequestId, payloadJson, out messageSignature);
                        HttpContent stringContent = StringContentSetting(payloadJson);
                        string FiservRequestCreate = string.Empty;
                        RequestHeaderSetting(client, timestamp, nonce, apiKeyFD, clientRequestId, messageSignature);

                        FiservRequestCreate = LogRequestCreateForTransaction(timestamp, nonce, apiKeyFD, clientRequestId, payloadJson, URLTransaction, messageSignature);
                        var result = await client.PostAsync(URLTransaction, stringContent).ConfigureAwait(true);
                        var transactionResult = await result.Content.ReadAsStringAsync().ConfigureAwait(true);
                        await LoggingTransaction(model, dataReload, client, ipgTransactionId, FiservRequestCreate, result, transactionResult);

                        //  model.ReloadUserId = 38;
                        //var response2 = await transactionResult.Content.ReadAsAsync<ApiResponse>();
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

                            var resultcredit = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.FiservCreditFundsi2c + "?creditUserId=" + model.ReloadUserId + "&amount=" + model.ReloadAmount + "&orderId=" + transactionDto.orderId).Result;
                            if(!resultcredit.IsSuccessStatusCode)
                            {
                                return Json(new { Status = false, messageSignature = msgwithcode, responseCode = "auto-refunded" });
                            }
                            if (model.IsAutoReload == true)
                                await setReloadRules(model);

                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                //await PostReloadAmountAfterTransaction(dataReload);
                            }
                            //  else { return ForUnsuccesfulFiservtoI2cTransfer(response); }
                        }
                        else
                        {
                            return Json(new { Status = false, messageSignature = msgwithcode , responseCode= transactionDto.processor.responseCode });
                        }

                    }
                }
                return Json(new { Status = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (ReloadRequest - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { Status = false });
            }

        }

        private async Task<string> LoggingTransaction(Models.ReloadRequestModel model, ReloadRequestDto dataReload, HttpClient client, string ipgTransactionId, string FiservRequestCreate, HttpResponseMessage result, string transactionResult)
        {
            var loggedId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                ipgTransactionId = await ForSuccessfulPayment(model, dataReload, client, ipgTransactionId, transactionResult);
            }
            HttpContent stringContentLogTransaction = CheckForLogTransaction(model, loggedId, client, FiservRequestCreate, transactionResult);
            model.ipgFirstTransactionId = ipgTransactionId;
            await CallMethodForLoggingNOtherDataInsertion(model, client, ipgTransactionId, stringContentLogTransaction);
            return ipgTransactionId;
        }

        private void ModalGeneration(Models.ReloadRequestModel model, string token,string expirydate, string ipgTransactionId, out string payloadJson, out string URLTransaction)
        {
            payloadJson = string.Empty;
            URLTransaction = string.Empty;
            // if (model.IsNewCardTransaction && string.IsNullOrEmpty(ipgTransactionId))    //old condition commenetd
            // {

            // }
            if (!string.IsNullOrEmpty(ipgTransactionId))
            {
                GenerateModelWIthTransactionId(model,token, expirydate, ipgTransactionId, out payloadJson, out URLTransaction);
            }
            else
            {
                GenerateModalForNullTransactionId(model, token, expirydate, out payloadJson, out URLTransaction);

            }
        }

        private void ForUserWebHookTokenClientRequestHeader(HttpClient client1)
        {
            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client1.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
        }

        private void ForBankStringContentNClientRequestHeader(HttpClient client, string json)
        {
            HttpContent stringContent = new StringContent(json);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
        }

        private IActionResult ForUnsuccesfuleBankTransfer(ApiResponse response)
        {
            return Json(new { Status = false, Message = response.Message });
        }
        private IActionResult ForUnsuccesfulFiservtoI2cTransfer(ApiResponse response)
        {
            return Json(new { Status = false, Message = response.Message });
        }
        private static string LogRequestCreateForTransaction(long timestamp, long nonce, string apiKeyFD, string clientRequestId, string payloadJson, string URLTransaction, string messageSignature)
        {
            return string.Concat("Headers:{Api-key: ", apiKeyFD, ",Message-Signature: ", messageSignature, ",Nonce: ", nonce.ToString(), ",Timestamp: ", timestamp.ToString(), ",Client-Request-Id: ", clientRequestId, "},URL:{", URLTransaction, "}Body:{", payloadJson, "}");
        }

        private static void GetCardWebHookToken(out string token, out string ipgTransactionId,out string expirydate,out string schemetransactionID, dynamic response1)
        {
            var CardDetail = response1.ToObject<GatewayCardWebhookTokenDto>();
            token = CardDetail?.Token.ToString();
            ipgTransactionId = CardDetail?.ipgFirstTransactionId?.ToString();
            expirydate = CardDetail?.expiryMonthYear?.ToString();
            schemetransactionID = CardDetail?.schemetransactionID?.ToString();

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

        private async Task CallMethodForLoggingNOtherDataInsertion(Models.ReloadRequestModel model, HttpClient client, string ipgTransactionId, HttpContent stringContentLogTransaction)
        {
            await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddPaymentTransactionLog, stringContentLogTransaction);
            if (model.IsCardDetailToSave == "0")
            {
                GatewayCardWebHookTokenModel objDelCard = new GatewayCardWebHookTokenModel()
                {
                    ClientToken = model.clientTokenPG
                };
                string jsonDelCard = Newtonsoft.Json.JsonConvert.SerializeObject(objDelCard);
                HttpContent stringContentDelCard = new StringContent(jsonDelCard);
                stringContentDelCard.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteGatewayWebToken, stringContentDelCard);
            }
            else
            {
                //if (!model.IsCardSelectionFromDropdown && model.IsNewCardTransaction)
                //  if( model.IsNewCardTransaction)
                //  {
                GatewayCardWebHookTokenModel objUpdCard = new GatewayCardWebHookTokenModel()
                {
                    ipgFirstTransactionId = ipgTransactionId,
                    ClientToken = model.clientTokenPG,
                    schemetransactionID=model.schemetransactionID
                };
                string jsonUpdCard = Newtonsoft.Json.JsonConvert.SerializeObject(objUpdCard);
                HttpContent stringContentUpdCard = new StringContent(jsonUpdCard);
                stringContentUpdCard.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UpdateWebToken, stringContentUpdCard);
                //    }
            }
        }

        private HttpContent CheckForLogTransaction(Models.ReloadRequestModel model, int loggedId, HttpClient client, string FiservRequestCreate, string transactionResult)
        {
            FiservPaymentTransactionLogModel oModelLog = new FiservPaymentTransactionLogModel()
            {
                creditUserId = model.ReloadUserId,
                debitUserId = loggedId,
                FiservRequestContent = FiservRequestCreate,
                FiservRequestDate = DateTime.UtcNow,
                FiservResponseContent = transactionResult,
                FiservResponseDate = DateTime.UtcNow
            };
            string jsonLogTransaction = Newtonsoft.Json.JsonConvert.SerializeObject(oModelLog);
            HttpContent stringContentLogTransaction = new StringContent(jsonLogTransaction);
            stringContentLogTransaction.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
            return stringContentLogTransaction;
        }

        private async Task<string> ForSuccessfulPayment(Models.ReloadRequestModel model, ReloadRequestDto dataReload, HttpClient client, string ipgTransactionId, string transactionResult)
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
                transactionDto.ProgramAccountIdSelected = Convert.ToInt32(model.ProgramAccountIdSelected);
                transactionDto.TransactionDateTime = transactionDateTime;
                transactionDto.TransactionPaymentGateway = "Fiserv";
               
            }
            /**/
            string jsonTransaction = Newtonsoft.Json.JsonConvert.SerializeObject(transactionDto);
            HttpContent stringContentTransaction = new StringContent(jsonTransaction.ToString());
            stringContentTransaction.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            var resultTransaction = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUserTransaction, stringContentTransaction);
            if (resultTransaction.IsSuccessStatusCode)
            {
                await PostReloadAmountAfterTransaction(dataReload);
            }

            return ipgTransactionId;
        }

        private void SetMessageHeadersSignature(long timestamp, string apiKeyFD, string apiSecretFD, string clientRequestId, string payloadJson, out string messageSignature)
        {
            var message = string.Concat(apiKeyFD, clientRequestId, timestamp, payloadJson);
            messageSignature = Sign(apiSecretFD, message);


        }

        private void GenerateModelWIthTransactionId(Models.ReloadRequestModel model,string token,string expirydate, string ipgTransactionId, out string payloadJson, out string URLTransaction)
        {
            //   URLTransaction = string.Concat(_configuration["SaleTransactionFirstData"], "/", ipgTransactionId);
            //var paymentPayload = new PayloadSecondaryTransaction()
            //{
            //    transactionAmount = new TransactionAmount()
            //    {
            //        total = model.ReloadAmount.Value,
            //        currency = "USD"
            //    },
            //    requestType = "ReturnTransaction",

            //};
            string mon = expirydate.Split('/')[0];
            string str = expirydate.Split('/')[1];
            string year = str.Substring(str.Length - 2);
            URLTransaction = _configuration["SaleTransactionFirstData"];
            var paymentPayload = new PayloadPaymentFirst()
            {
                transactionAmount = new TransactionAmount()
                {
                    total = model.ReloadAmount.Value,
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
                            month =mon,
                            year =year
                        }

                    }

                },
                storedCredentials = new StoredCredentials()
                {
                    // scheduled = true,
                    scheduled = false,
                    sequence = "SUBSEQUENT",
                   referencedSchemeTransactionId = model.schemetransactionID

                }
            };
            payloadJson = JsonConvert.SerializeObject(paymentPayload);
        }

        private void GenerateModalForNullTransactionId(Models.ReloadRequestModel model, string token, string expirydate, out string payloadJson, out string URLTransaction)
        {

            string mon = expirydate.Split('/')[0];
            string str = expirydate.Split('/')[1];
            string year = str.Substring(str.Length - 2);
            URLTransaction = _configuration["SaleTransactionFirstData"];
            var paymentPayload = new PayloadPaymentFirst()
            {
                transactionAmount = new TransactionAmount()
                {
                    total = model.ReloadAmount.Value,
                    currency = "USD"
                },
                requestType = "PaymentTokenSaleTransaction",
                storeId = _configuration["IPGPaymentGateway:storeId"],
                paymentMethod = new PaymentMenthod()
                {
                    paymentToken = new PaymentToken()
                    {
                        //function = "DEBIT",
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
                   scheduled=false,
                    sequence = "FIRST",
                }
            };
            payloadJson = JsonConvert.SerializeObject(paymentPayload);
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

        private void ReloadModalGenerate(Models.ReloadRequestModel model, out int loggedId, out ReloadRequestDto dataReload)
        {
            loggedId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
            model.BenefactorUserId = loggedId;
            dataReload = new ReloadRequestDto
            {
                AutoReloadAmount = model.AutoReloadAmount,
                BenefactorUserId = loggedId,
                CheckDroppedAmount = model.CheckDroppedAmount.HasValue ? model.CheckDroppedAmount.Value : 0,
                IsAutoReload = model.IsAutoReload,
                ProgramId = model.ProgramId,
                ReloadAmount = model.ReloadAmount,
                ReloadRequestId = model.ReloadRequestId,
                ReloadUserId = model.ReloadUserId

            };
        }

        /// <summary>
        /// This method is used to post reload amount in table after successful transaction by benefactor.
        /// </summary>
        /// <param name="dataReload"></param>
        /// <returns></returns>
        public async Task<IActionResult> PostReloadAmountAfterTransaction(ReloadRequestDto dataReload)
        {
            using (var client1 = new HttpClient())
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(dataReload);

                HttpContent stringContent1 = new StringContent(json1.ToString());
                stringContent1.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client1.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var result1 = await client1.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ReloadAmount, stringContent1);
                if (result1.IsSuccessStatusCode)
                {
                    var response1 = await result1.Content.ReadAsAsync<ApiResponse>();
                    return Json(new { Status = true, Message = response1.Message });
                }
                return Json(new { Status = false, Message = "" });
            }
        }
        /// <summary>
        /// This method is used to post the bank detail  of the user when Pay Via Bank is selected in reload request page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostBankDetailInformation(BankAccountModel model)
        {
            string resultMessage = "";
            string resultOutput = "0";
            // if (ModelState.IsValid)
            // {
            try
            {
                var loggedId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;

                var dataBankDetail = new I2CCardBankAccountModel()
                {
                    AccountNickName = model.AccountNickName,
                    AccountNumber = model.AccountNumber,
                    AccountTitle = model.AccountTitle,
                    AccountType = model.AccountType,
                    ACHType = model.ACHType,
                    BankName = model.BankName,
                    Comments = model.Comments,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = model.ClientIPAddress,
                    RoutingNumber = model.RoutingNumber,
                    I2CAccountDetailId = model.CardDetailIdI2C,
                    IsSandBoxAccount = false,
                    AccountHolderUniqueId = Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "UserUniqueId".ToLower(CultureInfo.InvariantCulture).Trim())),
                    Id = Convert.ToInt32(loggedId),
                    idRequesteeUserEnc = Cryptography.EncryptPlainToCipher(model.RequesteeUserId.ToString())

                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataBankDetail);
                    HttpContent stringContent = new StringContent(json);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddBankAccountDetail, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        resultOutput = Convert.ToString(response.Result);
                        resultMessage = Convert.ToString(response.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (PostOrganisationDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            // }
            return Json(new { data = resultOutput, dataMessage = resultMessage });
        }

        /// <summary>
        /// This method is used to fetch the existing banks of the user when Pay Via bank is selected.
        /// </summary>
        /// <param name="accountSerialNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBankAccountDropdownList(string accountSerialNo)
        {
            var userBankDetails = new I2CCardBankAccountModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);


                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.AccountBySerialNo + "?accountSrNum=" + accountSerialNo).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();

                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    userBankDetails = response1.ToObject<I2CCardBankAccountModel>();
                }
            }
            return Json(new { data = userBankDetails });
        }

        /// <summary>
        /// This method is used to fetch the existing card of the user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCardDropdownList()
        {
            var userBankDetails = new List<I2CCardBankAccountModel>();
            var userCards = new List<GatewayCardWebhookTokenDto>();
            List<SelectListItem> ddlCardList = new List<SelectListItem>();
            List<SelectListItem> ddlBanklist = new List<SelectListItem>();
            try
            {
                var benefactorId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserCardwithBankAccount + "?byUserId=" + benefactorId + "&toUserId=" + 34).Result;

                    // var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserCardwithBankAccount + "?debitUserId=" + benefactorId + "&creditUserId=" + ViewBag.usertabid).Result;

                    // var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersCards + "?debitUserId=" + benefactorId +"&creditUserId=" + ViewBag.usertabid).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        // userCards = response1.ToObject<List<GatewayCardWebhookTokenDto>>();
                        userBankDetails = response1.ToObject<List<I2CCardBankAccountModel>>();
                    }
                }

                if (userBankDetails.Any())
                {
                    for (var i = 0; i < userBankDetails.Count; i++)
                    {
                        var addBank = new SelectListItem();

                        addBank.Value = userBankDetails[i].IdValue;
                        addBank.Text = userBankDetails[i].CardBankName;
                        ddlBanklist.Add(addBank);

                    }
                }
                var text = new SelectListItem();
                text.Text = "+ add payment method";
                text.Value = "seladd";
                ddlBanklist.Add(text);
                ViewBag.UserBankAccounts = ddlBanklist;
                // ddlCardList = userCards.Select(c => new SelectListItem { Text = string.Concat(c.nickName, " (", c.maskedLastDigitCard, ")"), Value = c.ClientToken }).ToList();//, " (", (c.Status == "V" ? "Verified" : "Non-verified"), ")"
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetCardDropdownList - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = ddlBanklist });
        }
        [HttpGet]
        public async Task<IActionResult> GetCardDropdownList1(int id)
        {
            var userBankDetails = new List<I2CCardBankAccountModel>();
            var userCards = new List<GatewayCardWebhookTokenDto>();
            List<SelectListItem> ddlCardList = new List<SelectListItem>();
            List<SelectListItem> ddlBanklist = new List<SelectListItem>();
            try
            {
                var benefactorId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserCardwithBankAccount + "?byUserId=" + benefactorId + "&toUserId=" + id).Result;

                    // var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserCardwithBankAccount + "?debitUserId=" + benefactorId + "&creditUserId=" + ViewBag.usertabid).Result;

                    // var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersCards + "?debitUserId=" + benefactorId +"&creditUserId=" + ViewBag.usertabid).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        // userCards = response1.ToObject<List<GatewayCardWebhookTokenDto>>();
                        userBankDetails = response1.ToObject<List<I2CCardBankAccountModel>>();
                    }
                }

                if (userBankDetails.Any())
                {
                    for (var i = 0; i < userBankDetails.Count; i++)
                    {
                        var addBank = new SelectListItem();

                        addBank.Value = userBankDetails[i].IdValue;
                        addBank.Text = userBankDetails[i].CardBankName;
                        addBank.Disabled = (!userBankDetails[i].CardStatus);
                        ddlBanklist.Add(addBank);

                    }
                }
                var text = new SelectListItem();
                text.Text = "+ add payment method";
                text.Value = "seladd";
                ddlBanklist.Add(text);
                ViewBag.UserBankAccounts = ddlBanklist;
                // ddlCardList = userCards.Select(c => new SelectListItem { Text = string.Concat(c.nickName, " (", c.maskedLastDigitCard, ")"), Value = c.ClientToken }).ToList();//, " (", (c.Status == "V" ? "Verified" : "Non-verified"), ")"
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetCardDropdownList - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = ddlBanklist });
        }

        /// <summary>
        /// This method is used to get the latest webhool token of the card (not required now).
        /// </summary>
        /// <param name="ReloadUserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLatestHookTkn(string ReloadUserId)
        {
            string userToken = string.Empty;
            var benefactorId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);


                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersWebToken + "?debitUserId=" + benefactorId + "&creditUserId=" + ReloadUserId).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();

                    userToken = response.Result.ToString();

                }
            }
            return Json(new { data = userToken });
        }
        /// <summary>
        /// This method is used to verify bank account of the selected bank.
        /// </summary>
        /// <param name="accountSrNo"></param>
        /// <param name="bankName"></param>
        /// <param name="amountOne"></param>
        /// <param name="amountTwo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyBankAccount(string accountSrNo, string bankName, decimal amountOne, decimal amountTwo)
        {

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);


                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.VerifyAccountDetail + "?accountSrNo=" + accountSrNo + "&bankName=" + bankName + "&amountOne=" + amountOne + "&amountTwo=" + amountTwo).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    return Json(new { Status = Convert.ToBoolean(response.StatusFlagNum), Message = response.Message });
                }
            }

            return Json(null);
        }
        /// <summary>
        /// This method is used to get the columns of the transaction table.
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTableColumns(int tableType)
        {
            if (tableType == TableType.Transactions)
            {
                return Json(JsonConvert.SerializeObject(TransactionsColumn));
            }
            else { return Json(1); }

        }
        /// <summary>
        /// This method is used to get the transaction data in datatable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateMonth"></param>
        /// <param name="planAccount"></param>
        /// <returns></returns>
        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [HttpGet]
        public async Task<IActionResult> GetTransactionsData(int id, string dateMonth = "", string planAccount = "")
        {
            List<LinkedUsersTransactionsDto> linkedUserTransactions = new List<LinkedUsersTransactionsDto>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.LinkedUsersTransactions + "?linkedUserId=" + Convert.ToInt32(id) + "&dateMonth=" + dateMonth + "&plan=" + planAccount).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            linkedUserTransactions = response1.ToObject<List<LinkedUsersTransactionsDto>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetTransactionsData - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = linkedUserTransactions });


        }
        /// <summary>
        /// This method is used to get the current balance of the selected user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [HttpGet]
        public async Task<IActionResult> GetCurrentBalance(int id, int programId)
        {
            var usrAvailableBalance = new List<UserAvailableBalanceDto>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserAvailableBalanceByUserId + "?userId=" + Convert.ToInt32(id) + "&programId=" + programId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        usrAvailableBalance = response1.ToObject<List<UserAvailableBalanceDto>>();
                        ViewBag.userCurrentBalance = usrAvailableBalance[2].DataValue;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetCurrentBalance - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = usrAvailableBalance });

        }
        /// <summary>
        /// This method is used to get the reload rule of the user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetReloadRuleOfUser(int id)
        {
            ReloadRuleDto rule = new ReloadRuleDto();
            try
            {
                using (var client = new HttpClient())
                {
                    var benefactorId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ReloadRuleUser + "?userId=" + Convert.ToInt32(id) + "&benefactorId=" + Convert.ToInt32(benefactorId)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            rule = response1.ToObject<ReloadRuleDto>();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetReloadRuleOfUser - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = rule });

        }
        /// <summary>
        /// This method is used to get the detail of the program in which user comes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserProgramByUserId(int id)
        {
            int programId = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserProgramByUserId + "?userId=" + Convert.ToInt32(id)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.Status)
                            programId = Convert.ToInt32(response.Result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetUserProgramByUserId - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = programId });

        }

        /// <summary>
        /// This method is used to delete the invitation of the user sent for connection to the benefactor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteInvitation(int id, int programId)
        {
            int resultIdDelInv = 0;
            try
            {
                var inviteDelDto = new InvitationConfirmationDto
                {
                    UserId = id,
                    BenefactorId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    ProgramId = programId
                };
                using (var clientDelInv = new HttpClient())
                {
                    string jsonDelInv = Newtonsoft.Json.JsonConvert.SerializeObject(inviteDelDto);
                    HttpContent stringContentDelInv = new StringContent(jsonDelInv);
                    stringContentDelInv.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelInv.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultDelInv = await clientDelInv.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteInvitation, stringContentDelInv);
                    if (resultDelInv.IsSuccessStatusCode)
                    {
                        var responseDelInv = await resultDelInv.Content.ReadAsAsync<ApiResponse>();
                        if (responseDelInv.StatusFlagNum == 1)
                        {
                            resultIdDelInv = Convert.ToInt32(responseDelInv.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (DeleteInvitation - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = resultIdDelInv });

        }
        /// <summary>
        /// This method is used to accept the invitation of the user sent for connection to the benefactor 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AcceptInvitation(int id, int programId)
        {
            int resultIdAcceptInv = 0;
            try
            {
                var inviteDtoAccInv = new InvitationConfirmationDto
                {
                    UserId = id,
                    BenefactorId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    ProgramId = programId
                };
                using (var clientAccInv = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(inviteDtoAccInv);
                    HttpContent stringContentAccInv = new StringContent(json);
                    stringContentAccInv.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientAccInv.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultAccInv = await clientAccInv.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AcceptInvitation, stringContentAccInv);
                    if (resultAccInv.IsSuccessStatusCode)
                    {
                        var responseAccInv = await resultAccInv.Content.ReadAsAsync<ApiResponse>();
                        if (responseAccInv.StatusFlagNum == 1)
                        {
                            resultIdAcceptInv = Convert.ToInt32(responseAccInv.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (AcceptInvitation - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = resultIdAcceptInv });
        }
        /// <summary>
        /// This method is used to get all the users who are connection to the benefactor.
        /// </summary>
        /// <param name="checkForPrivacySettings"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<LinkedUsersModel>> GetLinkedUserOfBenefactor(bool checkForPrivacySettings)
        {
            List<LinkedUsersModel> linkedUsersBenefactor = new List<LinkedUsersModel>();
            try
            {
                using (var clientLinkedBenefactor = new HttpClient())
                {
                    clientLinkedBenefactor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientLinkedBenefactor.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var resultLinkedBenefactor = clientLinkedBenefactor.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.LinkedUsersInformation + "?benefactorId=" + Convert.ToInt32(Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value))).Result;
                    if (resultLinkedBenefactor.IsSuccessStatusCode)
                    {
                        var responseLinkedBenefactor = await resultLinkedBenefactor.Content.ReadAsAsync<ApiResponse>();
                        if (responseLinkedBenefactor.StatusFlagNum == 1)
                        {
                            dynamic responseDesLinkedBenefacor = JsonConvert.DeserializeObject(responseLinkedBenefactor.Result.ToString());
                            linkedUsersBenefactor = responseDesLinkedBenefacor.ToObject<List<LinkedUsersModel>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (GetLinkedUserOfBenefactor) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return linkedUsersBenefactor;
        }
        /// <summary>
        /// This method is used to delete the connection  of the benefactor which was earlier connected.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteBenefactorUser(int userId)
        {
            var linkedusers = new List<LinkedUsersModel>();
            try
            {
                var connectionDetail = new ConnectionDetailModel
                {
                    UserId = userId,
                    BenefactorId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    SessionId = "sddadasds",
                    ProgramId = 0,
                    Amount = 0,
                    Type = 2
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(connectionDetail);

                    HttpContent stringContent = new StringContent(json);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteBenefactorUser, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            linkedusers = await GetLinkedUserOfBenefactor(false).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (DeleteBenefactorUser - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return PartialView("_LinkedAccountsPartial", linkedusers);
        }
        /// <summary>
        /// This method is used to get the bank list view component
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reloadRequestId"></param>
        /// <param name="programId"></param>
        /// <param name="LoggedId"></param>
        /// <returns></returns>
        public IActionResult GetBankListViewComponent(int id, int reloadRequestId, int programId, int LoggedId)
        {
            var loggedId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;

            return ViewComponent("AddBankBalanceReload", new { id = id, reloadRequestId = reloadRequestId, programId = programId, LoggedId = Convert.ToInt32(loggedId) });
        }
        /// <summary>
        /// This method is used to get the balance reload view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reloadRequestId"></param>
        /// <param name="programId"></param>
        /// <param name="LoggedId"></param>
        /// <returns></returns>
        public IActionResult GetBalanceReloadViewComponent(int id, int reloadRequestId, int programId, int LoggedId)
        {
            var loggedId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
            ViewBag.usertabid = id;

            return ViewComponent("BalanceReload", new { id = id, reloadRequestId = reloadRequestId, programId = programId, LoggedId = Convert.ToInt32(loggedId) });
        }


        [TypeFilter(typeof(IsAccountLinkedAttribute), Arguments = new object[] { "islink" })]
        [HttpPost]
        [Authorize(Roles = "Benefactor, Basic User")]
        public async Task<IActionResult> setReloadRules(Foundry.Web.Models.ReloadRequestModel model)
        {
            try
            {
                int loggedId;
                ReloadRulesDto dataReloadRules;
                //ReloadModalGenerate(model, out loggedId, out dataReload);

                loggedId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                model.BenefactorUserId = loggedId;
                dataReloadRules = new ReloadRulesDto
                {
                    AutoReloadAmount = model.AutoReloadAmount,
                    BenefactorUserId = loggedId,
                    CheckDroppedAmount = model.CheckDroppedAmount.HasValue ? model.CheckDroppedAmount.Value : 0,
                    IsAutoReload = model.IsAutoReload,
                    ReloadAmount = model.ReloadAmount,
                    ReloadUserId = model.ReloadUserId,
                    i2cBankAccountId = model.AccountReloadSrNo,
                    CardId = model.CardToken
                };

                using (var client1 = new HttpClient())
                {
                    string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(dataReloadRules);

                    HttpContent stringContent1 = new StringContent(json1.ToString());
                    stringContent1.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client1.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result1 = await client1.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.SetReloadRules, stringContent1);
                    if (result1.IsSuccessStatusCode)
                    {
                        var response1 = await result1.Content.ReadAsAsync<ApiResponse>();
                        return Json(new { Status = true });
                    }
                    return Json(new { Status = false });
                }
                // return Json(new { Status = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (ReloadRequest - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { Status = false });
            }

        }





        [HttpPost]
        public async Task<IActionResult> PostCancelSubscriptionRule(Foundry.Web.Models.ReloadRequestModel model)
        {
            string resultId = "0";
            string message = string.Empty;
            var benefactorId = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
            try
            {
                var data = new ReloadRulesDto()
                {

                    BenefactorUserId = Convert.ToInt32(benefactorId),
                    ReloadUserId = model.ReloadUserId
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.CancelReloadRules, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToString(response.Result);
                            message = response.Message;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (PostCancelRule - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultId, message = message });
        }

        //public async Task<IActionResult> PostReloadAmountAfterTransaction(ReloadRequestDto dataReload)
        //{
        //    using (var client1 = new HttpClient())
        //    {
        //        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(dataReload);

        //        HttpContent stringContent1 = new StringContent(json1.ToString());
        //        stringContent1.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //        client1.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
        //        var result1 = await client1.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ReloadAmount, stringContent1);
        //        if (result1.IsSuccessStatusCode)
        //        {
        //            var response1 = await result1.Content.ReadAsAsync<ApiResponse>();
        //            return Json(new { Status = true, Message = response1.Message });
        //        }
        //        return Json(new { Status = false, Message = "" });
        //    }
        //}





        #region Export Benefactor Transaction Listing
        /// <summary>
        /// This method is used to export the benefactor transaction in excel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateMonth"></param>
        /// <param name="planAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BenefactorTransactionExportExcel(int id, string dateMonth = "", string planAccount = "")
        {
            var fileNameBenefactorTransaction = "Transaction List.xlsx";
            List<LinkedUsersTransactionsDto> linkedUserTransactionsBenefactor = new List<LinkedUsersTransactionsDto>();
            using (var clientBenefactorTran = new HttpClient())
            {
                clientBenefactorTran.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientBenefactorTran.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var resultBenefactorTran = clientBenefactorTran.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.LinkedUsersTransactions + "?linkedUserId=" + Convert.ToInt32(id) + "&dateMonth=" + dateMonth + "&plan=" + planAccount).Result;
                if (resultBenefactorTran.IsSuccessStatusCode)
                {
                    var responseBenefactorTran = await resultBenefactorTran.Content.ReadAsAsync<ApiResponse>();
                    if (responseBenefactorTran.StatusFlagNum == 1)
                    {
                        dynamic responseDescBenefactorTran = JsonConvert.DeserializeObject(responseBenefactorTran.Result.ToString());
                        linkedUserTransactionsBenefactor = responseDescBenefactorTran.ToObject<List<LinkedUsersTransactionsDto>>();
                    }
                }
            }

            // Getting all Organisation Programs data
            var dataTransaction = (from temp in linkedUserTransactionsBenefactor select temp).AsEnumerable();
            ExcelPackage excelBenefactorTran = new ExcelPackage();
            var workSheetBenefactorTran = excelBenefactorTran.Workbook.Worksheets.Add("Sheet1");
            workSheetBenefactorTran.TabColor = System.Drawing.Color.Black;
            workSheetBenefactorTran.DefaultRowHeight = 12;
            //Header of table  
            workSheetBenefactorTran.Row(1).Height = 20;
            workSheetBenefactorTran.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetBenefactorTran.Row(1).Style.Font.Bold = true;
            workSheetBenefactorTran.Cells[1, 1].Value = "MERCHANT NAME";
            workSheetBenefactorTran.Cells[1, 2].Value = "PERIOD";
            workSheetBenefactorTran.Cells[1, 3].Value = "AMOUNT";
            workSheetBenefactorTran.Cells[1, 4].Value = "DATE";
            workSheetBenefactorTran.Cells[1, 5].Value = "TIME";
            workSheetBenefactorTran.Cells[1, 6].Value = "PLAN NAME";
            //Body of table  

            int recordIndex = 2;
            foreach (var d in dataTransaction)
            {
                workSheetBenefactorTran.Cells[recordIndex, 1].Value = d.MerchantFullName;
                workSheetBenefactorTran.Cells[recordIndex, 2].Value = d.Period;
                workSheetBenefactorTran.Cells[recordIndex, 3].Value = d.Amount;
                workSheetBenefactorTran.Cells[recordIndex, 4].Value = d.Date;
                workSheetBenefactorTran.Cells[recordIndex, 5].Value = d.Time;
                workSheetBenefactorTran.Cells[recordIndex, 6].Value = d.PlanName;
                recordIndex++;
            }
            workSheetBenefactorTran.Column(1).AutoFit();
            workSheetBenefactorTran.Column(2).AutoFit();
            workSheetBenefactorTran.Column(3).AutoFit();
            workSheetBenefactorTran.Column(4).AutoFit();
            workSheetBenefactorTran.Column(5).AutoFit();
            workSheetBenefactorTran.Column(6).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelBenefactorTran.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameBenefactorTransaction });
        }

        /// <summary>
        /// This method is used to export the excel and download it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Download(string fileName)
        {
            byte[] data;
            if (HttpContext.Session.TryGetValue("DownloadExcel_OrgProg", out data))
            {
                return File(data, "application/octet-stream", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// This method is used to payment token request from payment js
        /// </summary>
        /// <param name="reloadUserId"></param>
        /// <param name="IsCardDetailToSave"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PaymentGateway(string reloadUserId, string IsCardDetailToSave, string NickName)
        {
            PGResponseTokenization resp = new PGResponseTokenization();
            try
            {
                long timestamp, nonce;
                string apiKeyFD, ipgJson, messageSignature;
                GetSettingAndMessageSignature(out timestamp, out nonce, out apiKeyFD, out ipgJson, out messageSignature);

                using (var client = new HttpClient())
                {
                    HttpContent stringContent = StringContentNHeadersForPayment(timestamp, nonce, apiKeyFD, ipgJson, messageSignature, client);
                    var clientToken = string.Empty; var nonceResp = string.Empty;
                    var result = await client.PostAsync(_configuration["FirstDataAuthorizeURL"], stringContent);

                    if (result.IsSuccessStatusCode && result.Headers.Any())
                    {
                        foreach (var item in result.Headers)
                        {
                            GetRespClientTokenNNonce(ref clientToken, ref nonceResp, item);
                        }

                        if (nonce == Convert.ToDouble(nonceResp))
                        {
                            resp = await SettingClientTokenNNonceNLoggingData(reloadUserId, IsCardDetailToSave, clientToken, nonceResp, NickName, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor (PaymentGateway - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Content(JsonConvert.SerializeObject(resp));
        }

        private async Task<PGResponseTokenization> SettingClientTokenNNonceNLoggingData(string reloadUserId, string IsCardDetailToSave, string clientToken, string nonceResp, string NickName, HttpResponseMessage result)
        {
            var publich64 = await result.Content.ReadAsStringAsync();
            publich64 = publich64.Substring(20, publich64.Length - 20);
            GatewayCardWebhookNLogModel objTuple = GenerateModalForLoggingNUserDetailInsertion(reloadUserId, IsCardDetailToSave, clientToken, nonceResp, NickName);

            using (var client1 = new HttpClient())
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(objTuple);
                HttpContent stringContentHook = new StringContent(json);
                stringContentHook.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client1.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                await client1.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddGatewayWebTokenNLog, stringContentHook);
            }

            return new PGResponseTokenization()
            {
                clientToken = clientToken,
                publicKeyBase64 = publich64.Substring(0, publich64.Length - 2)
            };


        }

        private GatewayCardWebhookNLogModel GenerateModalForLoggingNUserDetailInsertion(string reloadUserId, string IsCardDetailToSave, string clientToken, string nonceResp, string NickName)
        {
            return new GatewayCardWebhookNLogModel()
            {
                GatewayCardWebHookTokenModel = new GatewayCardWebHookTokenModel()
                {
                    Nonce = nonceResp,
                    ClientToken = clientToken,
                    debitUserId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    creditUserId = !string.IsNullOrEmpty(reloadUserId) ? Convert.ToInt32(reloadUserId) : 0,
                    IsCardToSave = Convert.ToBoolean(IsCardDetailToSave == "1" ? 1 : 0),
                    nickName = NickName
                },
                GatewayRequestResponseLogModel = new GatewayRequestResponseLogModel()
                {
                    Nonce = nonceResp,
                    ClientToken = clientToken,
                    debitUserId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    creditUserId = !string.IsNullOrEmpty(reloadUserId) ? Convert.ToInt32(reloadUserId) : 0,
                }
            };
        }

        private static void GetRespClientTokenNNonce(ref string clientToken, ref string nonceResp, KeyValuePair<string, IEnumerable<string>> item)
        {
            if (item.Key.Contains("Client-Token"))
            {
                clientToken = item.Value.FirstOrDefault();
            }
            if (item.Key.Contains("Nonce"))
            {
                nonceResp = item.Value.FirstOrDefault();
            }
        }

        private static HttpContent StringContentNHeadersForPayment(long timestamp, long nonce, string apiKeyFD, string ipgJson, string messageSignature, HttpClient client)
        {
            HttpContent stringContent = new StringContent(ipgJson);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            stringContent.Headers.ContentLength = ipgJson.Length;
            client.DefaultRequestHeaders.Add("Api-key", apiKeyFD);
            client.DefaultRequestHeaders.Add("Message-Signature", messageSignature);
            client.DefaultRequestHeaders.Add("Nonce", nonce.ToString());
            client.DefaultRequestHeaders.Add("Timestamp", timestamp.ToString());
            return stringContent;
        }

        private void GetSettingAndMessageSignature(out long timestamp, out long nonce, out string apiKeyFD, out string ipgJson, out string messageSignature)
        {
            Random generator = new Random();
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            nonce = timestamp + Convert.ToInt16(generator.Next(0, 9999).ToString("D" + 4));
            apiKeyFD = _configuration["credentials:apiKey"];
            var ipgGateway = new PgGatewayModel()
            {
                gateway = _configuration["IPGPaymentGateway:gateway"],
                apiKey = _configuration["IPGPaymentGateway:apiKey"],
                apiSecret = _configuration["IPGPaymentGateway:apiSecret"],
                storeId = _configuration["IPGPaymentGateway:storeId"],
                zeroDollarAuth = Convert.ToBoolean(_configuration["IPGPaymentGateway:zeroDollarAuth"])
            };
            ipgJson = JsonConvert.SerializeObject(ipgGateway);
            messageSignature = Sign(_configuration["credentials:apiSecret"], string.Concat(apiKeyFD, nonce, timestamp, ipgJson));
        }
        #endregion


        #region private methods
        /// <summary>
        /// This method is used to generate signature for payment js
        /// </summary>
        /// <param name="apiSecret"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public string Sign(string apiSecret, String payload = "")
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
    }
}