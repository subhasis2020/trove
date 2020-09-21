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
    public class AddBrandingViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddBrandingViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            var items = await GetBrandingDetails(id, ppId);
            ViewBag.PrimaryOrgId = poId;
            ViewBag.PrimaryProgId = ppId;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.UserEntityImageType = Convert.ToInt32(PhotoEntityType.BrandingLogo);

            if (string.IsNullOrEmpty(id))
            {
                items.id = 0;
            }
            return View(items);
        }
        [HttpGet]
        public async Task<ProgramBrandingModel> GetBrandingDetails(string id, string ppId)
        {
            ProgramBrandingModel orgModel = new ProgramBrandingModel();
            if (!string.IsNullOrEmpty(ppId))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var brandId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
                    var programId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(ppId));
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetBrandingDataById + "?brandId=" + brandId + "&programId=" + programId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        orgModel = response1.ToObject<ProgramBrandingModel>();
                        List<sys.SelectListItem> ddlprogramAccountlist = orgModel.programAccount.AsEnumerable().Select(c => new sys.SelectListItem { Text = c.accountName, Value = c.id.ToString() }).ToList();
                        ViewBag.programAccount = ddlprogramAccountlist;
                    }
                }
            }
            return orgModel;
        }
    }
}
