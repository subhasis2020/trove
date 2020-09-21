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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class MerchantRewardListViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public MerchantRewardListViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string poN, string ppN)
        {
            var items = await GetMerchantRewardDetails(id).ConfigureAwait(false);
            ViewBag.StartDate = DateTime.UtcNow.ToShortDateString();
            ViewBag.EndDate = DateTime.UtcNow.ToShortDateString();
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.MerchantId = id;
            ViewBag.BaseL = _configuration["ServiceAPIURL"];
            return View(items);
        }
        [HttpGet]
        public async Task<List<MerchantRewardModel>> GetMerchantRewardDetails(string id)
        {
            List<MerchantRewardModel> orgModel = new List<MerchantRewardModel>();

            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var organisationId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetMerchantRewardList + "?organisationId=" + organisationId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        orgModel = response1.ToObject<List<MerchantRewardModel>>();
                    }
                }
            }
            return orgModel.ToList();
        }
    }
}
