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
    public class MerchantAdminLevelViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public MerchantAdminLevelViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string poN, string ppN)
        {
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            MerchantAdminDetailModel orgAdmins = new MerchantAdminDetailModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var programresult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MasterRolesNProgramType + "?roleType=" + RoleType.MerchantRoleType).Result;
                if (programresult.IsSuccessStatusCode)
                {
                    var response = await programresult.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var model = response1.ToObject<MasterRoleNProgramTypeDto>();
                    List<RoleDto> roles = model.Roles;

                    List<sys.SelectListItem> ddlroleitemlist = roles.AsEnumerable().Select(c => new sys.SelectListItem { Text = c.Name.Replace("Merchant ", "").Replace("Merchant ", ""), Value = c.Id.ToString() }).ToList();
                    ViewBag.Role = ddlroleitemlist;
                }
                var merchantList = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MerchantListWithTransaction + "?programId=" + Cryptography.DecryptCipherToPlain(ppId)).Result;
                if (merchantList.IsSuccessStatusCode)
                {
                    var response = await merchantList.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var model = response1.ToObject<List<MerchantDto>>();

                    List<MerchantDto> merchantLst = model;

                    List<sys.SelectListItem> ddlMerchantLst = merchantLst.AsEnumerable().Select(c => new sys.SelectListItem
                    {
                        Text = c.MerchantName,
                        Value = c.Id.ToString(),
                        Selected = (Cryptography.DecryptCipherToPlain(id) == c.Id)
                    }).ToList();
                    ViewBag.MerchantList = ddlMerchantLst;
                }
            }
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.UserProfile);
            ViewBag.OrganisationId = id;
            return View(orgAdmins);
        }

    }
}
