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
    public class OrganisationAdminLevelViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public OrganisationAdminLevelViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            OrganisationAdminDetailModel orgAdmins = new OrganisationAdminDetailModel();
            using (var client = new HttpClient())
            {
                var organisationId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programresult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MasterRolesNOrganizationPrograms + "?roleType=" + RoleType.OrganizationRoleType + "&organisationId=" + organisationId).Result;
                if (programresult.IsSuccessStatusCode)
                {
                    var response = await programresult.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var model = response1.ToObject<MasterRoleNOrganizationProgramDto>();
                    List<OrganisationProgramDto> programTypeItemList = model.OrgProgram;
                    List<RoleDto> roles = model.Roles;
                    List<sys.SelectListItem> ddlitemlist = new List<sys.SelectListItem>();
                    if (programTypeItemList.Count > 0)
                        ddlitemlist = programTypeItemList.Select(c => new sys.SelectListItem { Text = c.ProgramName, Value = c.ProgramId.ToString() }).ToList();

                    ViewBag.ProgramType = ddlitemlist;

                    List<sys.SelectListItem> ddlroleitemlist = roles.Select(c => new sys.SelectListItem { Text = c.Name.Replace("Organisation ", "").Replace("Organization ", ""), Value = c.Id.ToString() }).ToList();

                    ViewBag.Role = ddlroleitemlist;

                }
            }
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.UserProfile);
            ViewBag.OrganisationId = id;
            return View(orgAdmins);
        }

    }
}
