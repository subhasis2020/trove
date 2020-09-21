using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Authorization;
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
using sys = Microsoft.AspNetCore.Mvc.Rendering;

namespace Foundry.Web.ViewComponents
{
    [Authorize]
    public class OrganisationListViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public OrganisationListViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string searchOrgText)
        {
            var items = await GetOrganisationInformation(searchOrgText).ConfigureAwait(false);
            return View(items);
        }

        [HttpGet]
        public async Task<List<OrganisationDto>> GetOrganisationInformation(string searchOrgText)
        {
            List<OrganisationDto> organisations = new List<OrganisationDto>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.Organisations).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    if (response.StatusFlagNum == 1)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        organisations = response1.ToObject<List<OrganisationDto>>();
                    }
                }
            }
            if (!string.IsNullOrEmpty(searchOrgText))
            {
                organisations = organisations.Where(x => (x.Name != null && x.Name.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchOrgText.ToLower(CultureInfo.InvariantCulture).Trim())) || (x.OrganisationSubTitle != null && x.OrganisationSubTitle.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchOrgText))).ToList();
            }
            return organisations;
        }
    }
}

