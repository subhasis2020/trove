using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Domain.Enums;
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
using sys = Microsoft.AspNetCore.Mvc.Rendering;

namespace Foundry.Web.ViewComponents
{
    public class AddAccountHolderViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddAccountHolderViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string prgId, string poId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgId = poId;
            ViewBag.ProgramId = prgId;
            ViewBag.ProgramName = ppN;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.UserEncId = id;
            List<int> selectedIds = new List<int>();
            var items = await GetAccountHolderDetailInformation(id, prgId);
            if (items != null && items.planIds != null && items.planIds.Count() > 0)
                items.SelectedPlanIds = selectedIds = items.planIds.Select(m => m.PlanId).ToList();

            var gender = GeneralSettingData.GetGenderDrpDwn();
            List<sys.SelectListItem> ddlgenderitemlist = gender.ToList().Select(c => new sys.SelectListItem { Text = c.Key, Value = c.Value.ToString() }).ToList();//, Selected = selectedIds.Contains(c.Id) ? true : false
            ViewBag.GenderList = ddlgenderitemlist;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.PlanListing + "?programId=" + (!string.IsNullOrEmpty(prgId) && prgId != "0" ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(prgId)) : 0)).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                   
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var planList = response1.ToObject<List<PlanListingDto>>();
                    List<PlanListingDto> PlanItemList = planList;
                    List<sys.SelectListItem> ddlitemlist = PlanItemList.ToList().Select(c => new sys.SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = selectedIds.Contains(c.Id) ? true : false }).ToList();

                    ViewBag.PlansList = ddlitemlist;
                }
            }
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.UserProfile);
            //  var items = await GetOrganisationProgramInformation(id, name);
            return View(items);
        }

        [HttpGet]
        public async Task<AccountHolderModel> GetAccountHolderDetailInformation(string id, string prgId)
        {
            AccountHolderModel usrModel = new AccountHolderModel();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    //var userId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAccountHolderDetail + "?userEncId=" + id + "&programEncId=" + prgId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                      
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        usrModel = response1.ToObject<AccountHolderModel>();
                        usrModel.ProgramCustomJsonFields = response.CustomResult;
                        usrModel.UserEncId = id;
                        
                    }

                }
            }
            //var jsontest = JsonConvert.SerializeObject(linkedUserTransactions);

            return usrModel;          //return Json(linkedUserTransactions);

        }
    }
}
