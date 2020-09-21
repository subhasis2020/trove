using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
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

namespace Foundry.Web.ViewComponents
{
    public class AddBankBalanceReloadViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddBankBalanceReloadViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id, int reloadRequestId, int programId, int LoggedId)
        {
            var userBankDetails = new List<I2CCardBankAccountModel>();
            var cardToken = new List<GatewayCardWebhookTokenDto>();
          //  var userReferenceDetailNumber = new I2CAccountDetailModel();
            /* Check for bank account. */
            //using (var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            //    var referenceUserDetail = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.Geti2cUserAccountDetail + "?id=" + Convert.ToInt32(id)).Result;
            //    if (referenceUserDetail.IsSuccessStatusCode)
            //    {
            //        var responseReference = await referenceUserDetail.Content.ReadAsAsync<ApiResponse>();
            //        if (!string.IsNullOrEmpty(Convert.ToString(responseReference.Result)))
            //        {
            //            dynamic referenceNumber = JsonConvert.DeserializeObject(responseReference.Result.ToString());
            //            userReferenceDetailNumber = referenceNumber.ToObject<I2CAccountDetailModel>();
            //        }
            //    }
            //    ViewBag.I2cCardDetailId = userReferenceDetailNumber.Id;
            //    ViewBag.UserReferenceId = userReferenceDetailNumber.ReferenceId;

            //}
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                //    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetBankAccountI2C + "?byUserId=" + LoggedId + "&toUserId=" + id).Result;
                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserCardwithBankAccount + "?byUserId=" + LoggedId + "&toUserId=" + id).Result;


                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();

                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    userBankDetails = response1.ToObject<List<I2CCardBankAccountModel>>();
                }

                var resultCard = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UsersCards + "?debitUserId=" + LoggedId+"&creditUserId=" +id).Result;
                if (resultCard.IsSuccessStatusCode)
                {
                    var responseCards = await resultCard.Content.ReadAsAsync<ApiResponse>();

                    dynamic cards = JsonConvert.DeserializeObject(responseCards.Result.ToString());
                    cardToken = cards.ToObject<List<GatewayCardWebhookTokenDto>>();
                }
            }

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
            List<SelectListItem> ddlBankAccountType = new List<SelectListItem>() {
                new SelectListItem(){ Text="Checking account",Value="01"},
                new SelectListItem(){ Text="Savings account",Value="11"}
            };
            ViewBag.UserBankAccountType = ddlBankAccountType;

            //List<SelectListItem> ddlBankACHType = new List<SelectListItem>() {
            //    new SelectListItem(){ Text="Bank to Card Transfer",Value="1"},
            //    new SelectListItem(){ Text="Card to Bank Transfer",Value="2"}
            //};
           // ViewBag.UserBankACHType = ddlBankACHType;
            return View();
        }
    }
}
