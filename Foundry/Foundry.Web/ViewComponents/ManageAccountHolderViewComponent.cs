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
    public class ManageAccountHolderViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public ManageAccountHolderViewComponent(IConfiguration configuration)
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
            if (items != null && items.planIds != null && items.planIds.Any())
                items.SelectedPlanIds = selectedIds = items.planIds.Select(m => m.PlanId).ToList();

            var gender = GeneralSettingData.GetGenderDrpDwn();
            List<sys.SelectListItem> ddlgenderitemlist = gender.AsEnumerable().Select(c => new sys.SelectListItem { Text = c.Key, Value = c.Value.ToString() }).ToList();//, Selected = selectedIds.Contains(c.Id) ? true : false
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
                    List<sys.SelectListItem> ddlitemlist = PlanItemList.AsEnumerable().Where(x => x.Status).Select(c => new sys.SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
                    if (ddlitemlist.Any())
                    {
                        for (var i = 0; i < ddlitemlist.Count; i++)
                        {
                            if (selectedIds.Contains(Convert.ToInt32(ddlitemlist[i].Value)))
                                ddlitemlist[i].Selected = true;
                        }
                    }
                    ViewBag.PlansList = ddlitemlist;
                }
            }
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.UserProfile);
            return View(items);
        }


        public async Task<AccountHolderModel> GetAccountHolderDetailInformation(string id, string prgId)
        {
            var usrBitePayBalance = new List<UserAvailableBalanceDto>();
            var jposusrbalance = new JPOSBiteBalanceApiModel();
            AccountHolderModel usrModel = new AccountHolderModel();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAccountHolderDetail + "?userEncId=" + id + "&programEncId=" + prgId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        usrModel = response1.ToObject<AccountHolderModel>();
                        usrModel.ProgramCustomJsonFields = response.CustomResult;
                        usrModel.UserEncId = id;
                        usrModel.Jpos_EncUserID = !string.IsNullOrEmpty(usrModel.Jpos_AccountHolderId) ? Cryptography.EncryptPlainToCipher(usrModel.Jpos_AccountHolderId) : "";
                        usrModel.ProgramUniqueColumnName = response.CustomResult1;
                    }
                    //balances
                    ViewBag.uid = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
                    var result1 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetJPOSBiteUserBalance + "?userId=" + Convert.ToInt32(Cryptography.DecryptCipherToPlain(id))).Result;
                    if (result1.IsSuccessStatusCode)
                    {
                        var response = await result1.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        jposusrbalance = response1.ToObject<JPOSBiteBalanceApiModel>();
                        if (jposusrbalance.success != false)
                        {
                            IEnumerable<double> objbitepay = from s in jposusrbalance.accounts.Where(x => x.name == "bite pay")
                                                             select s.balance;
                            ViewBag.usrBitePayBalance = objbitepay.FirstOrDefault();
                        }
                        else
                        {
                            ViewBag.usrBitePayBalance = 0;
                        }
                        if (jposusrbalance.success != false)
                        {
                            IEnumerable<double> objrewards = from s in jposusrbalance.accounts.Where(x => x.name == "bite rewards")
                                                             select s.balance;
                            ViewBag.usrBiteRewardsBalance = objrewards.FirstOrDefault();
                        }
                        else
                        {
                            ViewBag.usrBiteRewardsBalance = 0;
                        }




                    }
                    var result2 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetBiteUserLoyaltyTrackingBalance + "?userId=" + Convert.ToInt32(Cryptography.DecryptCipherToPlain(id))).Result;
                    if (result2.IsSuccessStatusCode)
                    {
                        var response = await result2.Content.ReadAsAsync<ApiResponse>();

                        ViewBag.usrLoyaltyTrackingBalance = response.Result;
                    }

                    var benefactorsDetails = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.BenefactorDetails + "?benefactorId=" + Convert.ToInt32(Cryptography.DecryptCipherToPlain(id))).Result;
                    if (benefactorsDetails.IsSuccessStatusCode)
                    {
                        var responseTransaction = await benefactorsDetails.Content.ReadAsAsync<ApiResponse>();
                        if (responseTransaction.Result != null)
                        {
                            //  transactions = JsonConvert.DeserializeObject<List<response.TranlogViewModel>>(responseTransaction.Result.ToString());
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseTransaction.Result.ToString());
                            List<LinkedUsersDto> benefactors = responseDesACH.ToObject<List<LinkedUsersDto>>();
                            if (benefactors != null)
                                ViewBag.BenefectorsCounts = benefactors.Count();
                            else
                                ViewBag.BenefectorsCounts = 0;
                        }

                    }



                }
            }
            return usrModel;
        }
    }
}
