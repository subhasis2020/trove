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

namespace Foundry.Web.ViewComponents
{
    public class BrandingListViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public BrandingListViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string poN, string ppN)
        {
            var items = await GetMerchantRewardDetails(id);
            ViewBag.StartDate = DateTime.UtcNow.ToShortDateString();
            ViewBag.EndDate = DateTime.UtcNow.ToShortDateString();
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.MerchantId = id;
            return View(items);
        }
        [HttpGet]
        public async Task<List<ProgramBrandingModel>> GetMerchantRewardDetails(string id)
        {
            List<ProgramBrandingModel> model = new List<ProgramBrandingModel>();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var programId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.BrandingListing + "?programId=" + programId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        model = response1.ToObject<List<ProgramBrandingModel>>();
                    }
                }
            }
            return model.ToList();
        }
    }
}
