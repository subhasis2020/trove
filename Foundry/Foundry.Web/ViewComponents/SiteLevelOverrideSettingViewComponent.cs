using Foundry.Domain;

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
using Foundry.Services;
using AutoMapper;
using Foundry.Domain.ApiModel;

namespace Foundry.Web.ViewComponents
{
    public class SiteLevelOverrideSettingViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public SiteLevelOverrideSettingViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
      
           
        }
        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppN, string poN)
        {
            var items = await GetSiteLevelOverrideSettings(id, poId,ppN,poN).ConfigureAwait(false);
            return View(items);
        }
        public async Task<List<Foundry.Web.Models.SiteLevelOverrideSettingViewModel>> GetSiteLevelOverrideSettings(string id, string poId, string ppN, string poN)
        {
            int Amount = 50;
            List<SiteLevelOverrideSettingViewModel> sitesettingModel = new List<SiteLevelOverrideSettingViewModel>();
            ViewBag.programid = Cryptography.DecryptCipherToPlain(id);
            List <OrgLoyalityGlobalSettingsDto> globalSettingViewModel = new List<OrgLoyalityGlobalSettingsDto>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);


                //getting org global settings//
               var organisationId = Convert.ToInt32(_configuration["SodexhoOrgId"]);
                if (organisationId > 0)
                {
                    var orgresult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationLoyalityGlobalSettings + "?id=" + organisationId).Result;
                    if (orgresult.IsSuccessStatusCode)
                    {
                        var response = await orgresult.Content.ReadAsAsync<ApiResponse>();
                        if (response.Result != null)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            globalSettingViewModel = response1.ToObject<List<OrgLoyalityGlobalSettingsDto>>();
                            foreach (var item in globalSettingViewModel)
                            {
                                ViewBag.bitePayRatio = item.bitePayRatio;
                                ViewBag.loyalityThreshhold = item.loyalityThreshhold;
                                ViewBag.globalReward = item.globalReward;
                                ViewBag.userStatusRegularRatio = item.userStatusRegularRatio;
                                ViewBag.userStatusVipRatio = item.userStatusVipRatio;
                                ViewBag.globalRatePoints = item.globalRatePoints;
                                ViewBag.dcbFlexRatio = item.dcbFlexRatio;
                                ViewBag.FirstTransactionBonus = item.FirstTransactionBonus;
                            }

                        }
                    }
                }
                //site level settings
                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.SiteLevelOverrideSettings + "?id=" + (!string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0)).Result;
                if (result.IsSuccessStatusCode)
                {

                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    if (response.Result != null)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());                      
                        sitesettingModel = response1.ToObject<List<SiteLevelOverrideSettingViewModel>>();

                        
                        foreach (var obj in sitesettingModel)
                        {

                            var VipDcb = obj.siteLevelDcbFlexRatio * obj.siteLevelUserStatusVipRatio * Amount;
                            var VipBitePay = obj.siteLevelUserStatusVipRatio * obj.siteLevelBitePayRatio * Amount;
                            var RegularDcb = obj.siteLevelDcbFlexRatio * obj.siteLevelUserStatusRegularRatio * Amount;
                            var RegularBitePay = obj.siteLevelBitePayRatio * obj.siteLevelUserStatusRegularRatio * Amount;

                            ViewBag.VipDcb = VipDcb;
                            ViewBag.VipBitePay = VipBitePay;
                            ViewBag.RegularDcb = RegularDcb;
                            ViewBag.RegulatBitePay = RegularBitePay;

                            //Reward Amount Calculation
                            //ViewBag.RegularUserRateBitePayRate = (RegularBitePay / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (RegularBitePay / obj.loyalityThreshhold) * obj.globalReward;
                            //ViewBag.VipUserRateBitePayRate = (VipBitePay / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (VipBitePay / obj.loyalityThreshhold) * obj.globalReward;
                            //ViewBag.RegularUserCampusCardRate = (RegularDcb / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (RegularDcb / obj.loyalityThreshhold) * obj.globalReward;
                            //ViewBag.VipUserRateCampusCardRate = (VipDcb / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (VipDcb / obj.loyalityThreshhold) * obj.globalReward;

                            //Reward as % Discount Calculation
                            ViewBag.RegularUserBitePayDiscount = (ViewBag.RegularUserRateBitePayRate / Amount) * 100;
                            ViewBag.VipUserBitePayDiscount = (ViewBag.VipUserRateBitePayRate / Amount) * 100;
                            ViewBag.RegularUserCampusCardDiscount = (ViewBag.RegularUserCampusCardRate / Amount) * 100;
                            ViewBag.VipUserCampusCardDiscount = (ViewBag.VipUserRateCampusCardRate / Amount) * 100;

                        }
                    }
                   
                }
            }
            return sitesettingModel;
        }
      
       
    }
}
