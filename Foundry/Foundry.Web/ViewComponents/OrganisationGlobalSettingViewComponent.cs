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

namespace Foundry.Web.ViewComponents
{
    public class OrganisationGlobalSettingViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
       
        public OrganisationGlobalSettingViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
           
        }
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
              var items = await GetOrganisationLoyalityGlobalSetting(id).ConfigureAwait(false);        
              return View(items);
        }
        [HttpGet]
        public async Task<List<LoyalityGlobalSettingDetailModel>> GetOrganisationLoyalityGlobalSetting(string id)
        {
            decimal Amount = 50;
            List<LoyalityGlobalSettingDetailModel> settingModel = new List<LoyalityGlobalSettingDetailModel>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                
                var organisationId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                organisationId =Convert.ToInt32( _configuration["SodexhoOrgId"]);
                if (organisationId > 0)
                {
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationLoyalityGlobalSettings + "?id=" + organisationId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.Result != null)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            settingModel = response1.ToObject<List<LoyalityGlobalSettingDetailModel>>();
                            foreach (var obj in settingModel)
                            {

                                var VipDcb = obj.dcbFlexRatio * obj.userStatusVipRatio * Amount;
                                var VipBitePay = obj.userStatusVipRatio * obj.bitePayRatio * Amount;
                                var RegularDcb = obj.dcbFlexRatio * obj.userStatusRegularRatio * Amount;
                                var RegularBitePay = obj.bitePayRatio * obj.userStatusRegularRatio * Amount;

                                ViewBag.VipDcb = VipDcb;
                                ViewBag.VipBitePay = VipBitePay;
                                ViewBag.RegularDcb = RegularDcb;
                                ViewBag.RegulatBitePay = RegularBitePay;

                                //Overall Results
                                ViewBag.Result1 = String.Format("{0:.##}", (obj.bitePayRatio / obj.dcbFlexRatio));
                                ViewBag.Result2 = String.Format("{0:.##}", (obj.userStatusVipRatio / obj.userStatusRegularRatio));                     

                                //Reward Amount Calculation
                                ViewBag.RegularUserRateBitePayRate = (RegularBitePay / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (RegularBitePay / obj.loyalityThreshhold) * obj.globalReward;
                                ViewBag.VipUserRateBitePayRate = (VipBitePay / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (VipBitePay / obj.loyalityThreshhold) * obj.globalReward;
                                ViewBag.RegularUserCampusCardRate = (RegularDcb / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (RegularDcb / obj.loyalityThreshhold) * obj.globalReward;
                                ViewBag.VipUserRateCampusCardRate = (VipDcb / obj.loyalityThreshhold) * obj.globalReward < obj.globalReward ? 0 : (VipDcb / obj.loyalityThreshhold) * obj.globalReward;

                                //Reward as % Discount Calculation
                                ViewBag.RegularUserBitePayDiscount = Math.Round((ViewBag.RegularUserRateBitePayRate / Amount) * 100,0);
                                ViewBag.VipUserBitePayDiscount = (ViewBag.VipUserRateBitePayRate / Amount) * 100;
                                ViewBag.RegularUserCampusCardDiscount = (ViewBag.RegularUserCampusCardRate / Amount) * 100;
                                ViewBag.VipUserCampusCardDiscount = (ViewBag.VipUserRateCampusCardRate / Amount) * 100;



                            }
                        }
                    }
                }
            }
        
            return settingModel;

        }
    }
}
