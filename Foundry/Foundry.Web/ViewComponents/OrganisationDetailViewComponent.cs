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
    public class OrganisationDetailViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public OrganisationDetailViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(string idEnc)
        {
            List<int> selectedIds = new List<int>();
            var itemsOrgDetail = await GetOrganisationDetailInformation(idEnc).ConfigureAwait(false);
            if (itemsOrgDetail != null && itemsOrgDetail.OrganisationProgramType != null && itemsOrgDetail.OrganisationProgramType.Count > 0)
            {
                itemsOrgDetail.SelectedOrganisationProgramType = selectedIds = itemsOrgDetail.OrganisationProgramType.Select(m => m.ProgramTypeId).ToList();
            }
            using (var clientInvokeOrgDetail = new HttpClient())
            {
                clientInvokeOrgDetail.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientInvokeOrgDetail.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programresult = clientInvokeOrgDetail.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramType).Result;
                if (programresult.IsSuccessStatusCode)
                {
                    var response = await programresult.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var programtype = response1.ToObject<List<ProgramTypesDto>>();
                    List<ProgramTypesDto> programTypeItemList = programtype;
                    List<sys.SelectListItem> ddlitemlist = programTypeItemList.Select(c => new sys.SelectListItem { Text = c.ProgramTypeName, Value = c.Id.ToString() }).ToList(); 
                    if (ddlitemlist.Any())
                    {
                        for (var i = 0; i < ddlitemlist.Count; i++)
                        {
                            if (selectedIds.Contains(Convert.ToInt32(ddlitemlist[i].Value)))
                                ddlitemlist[i].Selected = true;
                        }
                    }
                    ViewBag.ProgramType = ddlitemlist;
                }
            }
            ViewBag.OrganisationId = idEnc;
            if (itemsOrgDetail != null && !string.IsNullOrEmpty(itemsOrgDetail.Description))
                itemsOrgDetail.Description = itemsOrgDetail.Description.Replace("<br/>", "\n").Replace("<br/>", Environment.NewLine);
            return View(itemsOrgDetail);
        }

        [HttpGet]
        public async Task<OrganisationDetailModel> GetOrganisationDetailInformation(string idEnc)
        {
            OrganisationDetailModel orgModelDetail = new OrganisationDetailModel();
            if (!string.IsNullOrEmpty(idEnc))
            {
                using (var clientOrgDetail = new HttpClient())
                {
                    clientOrgDetail.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientOrgDetail.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var organisationIdInOrgDetail = Convert.ToInt32(Cryptography.DecryptCipherToPlain(idEnc));
                    if (organisationIdInOrgDetail > 0)
                    {
                        var resultOrgDetail = clientOrgDetail.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationInfoNProgramTypes + "?organisationId=" + organisationIdInOrgDetail).Result;
                        if (resultOrgDetail.IsSuccessStatusCode)
                        {
                            var responseOrgDetail = await resultOrgDetail.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDesOrgDetail = JsonConvert.DeserializeObject(responseOrgDetail.Result.ToString());
                            orgModelDetail = responseDesOrgDetail.ToObject<OrganisationDetailModel>();
                        }
                    }
                }
            }
            return orgModelDetail;
        }
    }
}
