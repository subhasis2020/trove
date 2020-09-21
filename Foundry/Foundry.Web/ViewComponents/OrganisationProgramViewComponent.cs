using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
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
    public class OrganisationProgramViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public OrganisationProgramViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string name)
        {
            var items = await GetOrganisationProgramInformation(id, name).ConfigureAwait(false);
            return View(items);
        }

        [HttpGet]
        public async Task<List<ProgramDto>> GetOrganisationProgramInformation(string id, string name)
        {
            List<ProgramDto> prgModel = new List<ProgramDto>();
            List<OrganisationProgramDto> orgPrg = new List<OrganisationProgramDto>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var organisationId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                if (organisationId > 0)
                {
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationPrograms + "?organisationId=" + organisationId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.Result != null)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            orgPrg = response1.ToObject<List<OrganisationProgramDto>>();
                        }
                    }
                }
            }
            ViewBag.OrganisationProgramIds = orgPrg;
            return prgModel;

        }
    }
}
