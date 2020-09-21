using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
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
    public class AccountHolderListOrganizationViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AccountHolderListOrganizationViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppN, string poN, string orgId="")
        {
             ViewBag.PrimaryOrgName = poN;
           
            List<PlanListingDto> plans = new List<PlanListingDto>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.PlanListing + "?programId=" + (!string.IsNullOrEmpty(id) && id != "0" ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0)).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    if (response.StatusFlagNum == 1)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        plans = response1.ToObject<List<PlanListingDto>>();
                        plans.ForEach(x => x.StrId = Cryptography.EncryptPlainToCipher(x.Id.ToString()));
                    }
                }
            }
            ViewBag.PlansDetails = plans;
            return View();
        }
    }
}
