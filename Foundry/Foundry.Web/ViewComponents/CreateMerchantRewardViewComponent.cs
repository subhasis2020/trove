using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using sys = Microsoft.AspNetCore.Mvc.Rendering;

namespace Foundry.Web.ViewComponents
{
    public class CreateMerchantRewardViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public CreateMerchantRewardViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            var items = await GetMerchantRewardDetails(id, pId).ConfigureAwait(false);
            ViewBag.MerchantId = id;
            ViewBag.PromotionId = pId;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            if (string.IsNullOrEmpty(pId))
            {
                items.BusinessTypeId = 1;
            }
            return View(items);
        }
        [HttpGet]
        public async Task<MerchantRewardModel> GetMerchantRewardDetails(string id, string pid)
        {
            MerchantRewardModel orgModel = new MerchantRewardModel();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var organisationId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
                    var promotionId = !string.IsNullOrEmpty(pid) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(pid)) : 0;
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetMerchantRewardDetail + "?organisationId=" + organisationId + "&promotionId=" + promotionId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        orgModel = response1.ToObject<MerchantRewardModel>();
                        if (orgModel != null && !string.IsNullOrEmpty(orgModel.Description))
                            orgModel.Description = orgModel.Description.Replace("<br/>", "\n").Replace("<br/>", Environment.NewLine);
                    }
                }
            }
            ViewBag.BaseL = _configuration["ServiceAPIURL"];
            return orgModel;
        }
    }
}
