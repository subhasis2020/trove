using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class MerchantPromotionsViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public MerchantPromotionsViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            PromotionDetailModel orgPromotions = new PromotionDetailModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var programresult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MasterOfferCodeNWeekDays).Result;
                if (programresult.IsSuccessStatusCode)
                {
                    var response = await programresult.Content.ReadAsAsync<ApiResponse>();
                    if (response.StatusFlagNum == 1)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        var model = response1.ToObject<MasterOfferCodeNWeekDayDto>();
                        List<OfferCodeDto> offerCodeItemList = model.OfferCodes;
                        List<WeekDayDto> weekDaysList = model.WeekDays;
                        orgPromotions.OfferCodes = offerCodeItemList;

                        List<SelectListItem> ddlWeekDayItemlist = weekDaysList.AsEnumerable().Select(c => new SelectListItem { Text = c.DayName, Value = c.DayName }).ToList();
                        ViewBag.WeekDay = ddlWeekDayItemlist;
                    }
                }
            }
            ViewBag.OrganisationId = id;
            ViewBag.MerchantId = id;
            ViewBag.PromotionId = pId;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.Offers);
            return View(orgPromotions);

        }
    }
}
