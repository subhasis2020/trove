using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using sys = Microsoft.AspNetCore.Mvc.Rendering;
namespace Foundry.Web.ViewComponents
{
    public class ProgramLevelAdminViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public ProgramLevelAdminViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            ProgramLevelAdminDetailModel orgAdmins = new ProgramLevelAdminDetailModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var programresult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MasterRolesNProgramType + "?roleType=" + RoleType.ProgramRoleType).Result;
                if (programresult.IsSuccessStatusCode)
                {
                    var response = await programresult.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var model = response1.ToObject<MasterRoleNProgramTypeDto>();
                    List<RoleDto> roles = model.Roles;
                   
                    List<sys.SelectListItem> ddlroleitemlist = roles.Select(c => new sys.SelectListItem { Text = c.Name.Replace("Program ", "").Replace("Program ", ""), Value = c.Id.ToString() }).ToList();

                    ViewBag.Role = ddlroleitemlist;

                }
                var organisationId = !string.IsNullOrEmpty(poId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(poId)) : 0;
                var programList = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationPrograms + "?organisationId=" + organisationId).Result;
                if (programList.IsSuccessStatusCode)
                {
                    var response = await programList.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var model = response1.ToObject<List<OrganisationProgramDto>>();

                    List<OrganisationProgramDto> programListDrp = model;

                    List<sys.SelectListItem> ddlProgramLst = programListDrp.Select(c => new sys.SelectListItem
                    {
                        Text = c.ProgramName,
                        Value = c.ProgramId.ToString(),
                        Selected = (Cryptography.DecryptCipherToPlain(id) == c.ProgramId.ToString())
                    }).ToList();
                    ViewBag.ProgramList = ddlProgramLst;
                }
            }
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.UserProfile);
            return View(orgAdmins);          
        }
    }
}
