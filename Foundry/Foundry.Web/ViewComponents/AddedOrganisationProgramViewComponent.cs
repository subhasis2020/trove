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

namespace Foundry.Web.ViewComponents
{
    public class AddedOrganisationProgramViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddedOrganisationProgramViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            ViewBag.OrganisationId = id;
            await Task.FromResult(0);
            return View();
        }

        [HttpGet]
        public async Task<OrganisationDetailModel> GetOrganisationProgramsInformation(string id)
        {
            OrganisationDetailModel orgModel = new OrganisationDetailModel();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var organisationId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
                    if (organisationId > 0)
                    {
                        var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationPrograms + "?organisationId=" + organisationId).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            orgModel = response1.ToObject<OrganisationDetailModel>();
                        }
                    }
                }
            }
            return orgModel;
        }
    }
}
