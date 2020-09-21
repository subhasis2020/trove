using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Web.ViewComponents
{
    public class BalanceReloadViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public BalanceReloadViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id, int reloadRequestId, int programId, int LoggedId)
        {
            try
            {
                var usrAvailableBalance = new List<UserAvailableBalanceDto>();
                var userBankDetails1 = new List<BankAccountModel>();
                var userBankDetails = new List<I2CCardBankAccountModel>();
                var cardToken = new List<GatewayCardWebhookTokenDto>();
                var programAccountsUser = new List<AccountListingDto>();
                List<LinkedUsersModel> linkedUsers = new List<LinkedUsersModel>();
                var loggedId = Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                                      
                    var resultCard = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramAccountDropdownForUser + "?userId=" + id).Result;
                    if (resultCard.IsSuccessStatusCode)
                    {
                        var responseCards = await resultCard.Content.ReadAsAsync<ApiResponse>();

                        dynamic cards = JsonConvert.DeserializeObject(responseCards.Result.ToString());
                        programAccountsUser = cards.ToObject<List<AccountListingDto>>();
                    }
               //     var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetBankAccountI2C + "?byUserId=" + LoggedId + "&toUserId=" + id).Result;
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserCardwithBankAccount + "?byUserId=" + LoggedId + "&toUserId=" + id).Result;
                  

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();

                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        userBankDetails = response1.ToObject<List<I2CCardBankAccountModel>>();
                     
                    }
                    var resultCards = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersCards + "?debitUserId=" + LoggedId+"&creditUserId=" +id).Result;
                    //  var resultCards = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersCards + "?debitUserId=" + LoggedId).Result;
                    if (resultCards.IsSuccessStatusCode)
                    {
                        var responseCards = await resultCards.Content.ReadAsAsync<ApiResponse>();

                        dynamic cards = JsonConvert.DeserializeObject(responseCards.Result.ToString());
                        cardToken = cards.ToObject<List<GatewayCardWebhookTokenDto>>();
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
                }
                if (User.IsInRole(Roles.Benefactor))
                {
                    ViewBag.UserId = id == 0 ? linkedUsers.Select(x => x.linkedUserId).FirstOrDefault() : id;
                }
                else
                {
                    ViewBag.UserId = Convert.ToInt32(loggedId);
                }
                ViewBag.ReloadRequestId = reloadRequestId;
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
                        //addBank.Value = userBankDetails[i].AccountSrNo;
                        //if (userBankDetails[i].Status)
                        //    addBank.Text = string.Concat(userBankDetails[i].BankName, " (Verified)");
                        //else
                        //    addBank.Text = string.Concat(userBankDetails[i].BankName, " (Non-verified)");
                        //ddlBanklist.Add(addBank);

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

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Benefactor/User (BalanceReload - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
             return View();
           
        }
        
    }
}
