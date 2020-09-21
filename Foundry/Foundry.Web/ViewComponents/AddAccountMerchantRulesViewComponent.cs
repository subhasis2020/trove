using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
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

namespace Foundry.Web.ViewComponents
{
    public class AddAccountMerchantRulesViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddAccountMerchantRulesViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string ppN, string poN, string btIdNatId, string aId)
        {
            var btIdNatIdSplit = btIdNatId.Split("^");
            ViewBag.PrimaryOrgId = poId;
            ViewBag.PrimaryProgId = ppId;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PaId = id;
            ViewBag.AccountId = aId;
            ViewBag.AccountTypeId = btIdNatIdSplit[1];
            var items = await GetAccountMerchantRulesInformation(ppId, btIdNatIdSplit[0], btIdNatIdSplit[1], aId);
            return View(items);
        }
        [HttpGet]
        public async Task<AccountMerchantRuleModel> GetAccountMerchantRulesInformation(string ppId, string btId, string atId, string aId)
        {
            AccountMerchantRuleModel model = new AccountMerchantRuleModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var programId = !string.IsNullOrEmpty(ppId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(ppId)) : 0;
                var accountId = !string.IsNullOrEmpty(aId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(aId)) : 0;
                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetBusinessTypeAndMerchantListing + "?programId=" + programId + "&businessTypeId=" + btId + "&accountTypeId=" + atId + "&accountId=" + accountId).Result;
                model = await GetAccountMerchantRulesInformationRefactor(btId, model, result);
            }
            return model;
        }

        private async Task<AccountMerchantRuleModel> GetAccountMerchantRulesInformationRefactor(string btId, AccountMerchantRuleModel model, HttpResponseMessage result)
        {
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    model = response1.ToObject<AccountMerchantRuleModel>();
                    if (!string.IsNullOrEmpty(btId))
                    {
                        model.selectedBusinessType = new List<int>();
                        foreach (var item in btId.Split(','))
                        {
                            if (Convert.ToInt32(item) != 0)
                                model.selectedBusinessType.Add(Convert.ToInt32(item));
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web := (ViewComponent := AddAccountMerchantRulesViewComponent)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));

                }
            }

            return model;
        }
    }
}
