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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using sys = Microsoft.AspNetCore.Mvc.Rendering;

namespace Foundry.Web.ViewComponents
{
    public class CreateMerchantBusinessInfoViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public CreateMerchantBusinessInfoViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            var items = await GetMerchantBusinessInfo(id);
            ViewBag.MerchantId = id;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.Date = DateTime.UtcNow.ToShortDateString();
            return View(items);
        }
        [HttpGet]
        public async Task<MerchantBusinessInfoModel> GetMerchantBusinessInfo(string id)
        {
            MerchantBusinessInfoModel orgModel = new MerchantBusinessInfoModel();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage result = GetMerchantBusinessInfoData(id, client);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        orgModel = MapResponseToModal(id, response);
                        CheckForHoursOfOperationNOtherMerchantInfo(orgModel);
                        CheckForMerchantTerminalNBusinessType(orgModel);
                        BindingDataForDrpDwnInViewBag(orgModel);
                    }
                }
            }
            return orgModel;
        }

        private static MerchantBusinessInfoModel MapResponseToModal(string id, ApiResponse response)
        {
            MerchantBusinessInfoModel orgModel;
            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
            orgModel = response1.ToObject<MerchantBusinessInfoModel>();
            orgModel.Id = id;
            return orgModel;
        }

        private HttpResponseMessage GetMerchantBusinessInfoData(string id, HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            var organisationId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
            var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetMerchantBusinesInfo + "?organisationId=" + organisationId).Result;
            return result;
        }

        private void CheckForMerchantTerminalNBusinessType(MerchantBusinessInfoModel orgModel)
        {
            if (orgModel.MerchantTerminal == null || !orgModel.MerchantTerminal.Any())
            {
                orgModel.MerchantTerminal = AddEmptyTerminal();
            }
            if (orgModel.MealPeriod == null || !orgModel.MealPeriod.Any())
            {
                orgModel.MealPeriod = AddEmptyMealPeriod();
            }
            else
            {
                var obj = new MealPeriodDto();
                obj.isSelected = true;
                obj.title = "";
                obj.openTime = TimeSpan.Parse("12:00");
                obj.closeTime = TimeSpan.Parse("12:00");
                orgModel.MealPeriod.Insert(0, obj);
            }
        }

        private void CheckForHoursOfOperationNOtherMerchantInfo(MerchantBusinessInfoModel orgModel)
        {
            if (orgModel.HoursOfOperation == null || !orgModel.HoursOfOperation.Any())
            {
                orgModel.HoursOfOperation = AddEmptySchedule();
            }
            if (orgModel.HolidayHours == null || !orgModel.HolidayHours.Any())
            {
                orgModel.HolidayHours = AddEmptyHolidayHrsSchedule();
            }
            else
            {
                orgModel.HolidayHours.ToList().ForEach(x => x.HolidayDate = !string.IsNullOrEmpty(x.HolidayDate) ? (Convert.ToDateTime(x.HolidayDate)).ToShortDateString() : "");
            }
        }

        private void BindingDataForDrpDwnInViewBag(MerchantBusinessInfoModel orgModel)
        {
            List<WeekDayDto> weekDaysList = orgModel.WeekDays;
            List<SelectListItem> ddlWeekDayItemlist = weekDaysList.AsEnumerable().Select(c => new SelectListItem { Text = c.DayName, Value = c.DayName }).ToList();

            ViewBag.WeekDay = ddlWeekDayItemlist;
            List<DwellTimeDto> dwelltime = orgModel.DwellTime;
            List<SelectListItem> ddlDwellItemlist = dwelltime.AsEnumerable().Select(c => new SelectListItem { Text = c.Time, Value = c.Id.ToString() }).ToList();

            ViewBag.DwellTime = ddlDwellItemlist;

            List<TerminalTypeDto> terminalType = orgModel.TerminalType;
            List<SelectListItem> ddlTerminalTypeItemlist = terminalType.AsEnumerable().Select(c => new SelectListItem { Text = c.TerminalType, Value = c.Id.ToString() }).ToList();

            ViewBag.TerminalType = ddlTerminalTypeItemlist;
        }

        public List<OrganisationScheduleDto> AddEmptySchedule()
        {
            var lst = new List<OrganisationScheduleDto>();
            var obj = new OrganisationScheduleDto()
            {
                WorkingDay = "Sunday",
                OpenTime = TimeSpan.Parse("12:00"),
                ClosedTime = TimeSpan.Parse("12:00"),
            };
            lst.Add(obj);

            return lst;
        }
        public List<OrganisationScheduleDto> AddEmptyHolidayHrsSchedule()
        {
            var lst = new List<OrganisationScheduleDto>();
            var obj = new OrganisationScheduleDto()
            {
                HolidayDate = null,
                OpenTime = TimeSpan.Parse("12:00"),
                ClosedTime = TimeSpan.Parse("12:00"),
                IsHoliday = true,
            };
            lst.Add(obj);
            return lst;
        }
        public List<MerchantTerminalDto> AddEmptyTerminal()
        {
            var lst = new List<MerchantTerminalDto>();

            var objTerminal = new MerchantTerminalDto()
            {
                terminalId = "",
                terminalName = "",
                terminalType = 1
            };
            lst.Add(objTerminal);
            return lst;
        }
        public List<MealPeriodDto> AddEmptyMealPeriod()
        {
            var lst = new List<MealPeriodDto>();

            var objMealPeriod = new MealPeriodDto();
            objMealPeriod.isSelected = true;
            objMealPeriod.title = "";
            objMealPeriod.openTime = TimeSpan.Parse("12:00");
            objMealPeriod.closeTime = TimeSpan.Parse("12:00");
            lst.Add(objMealPeriod);
            var objMealPeriod1 = new MealPeriodDto();
            objMealPeriod1.isSelected = true;
            objMealPeriod1.title = "Breakfast";
            objMealPeriod1.openTime = TimeSpan.Parse("12:00");
            objMealPeriod1.closeTime = TimeSpan.Parse("12:00");
            lst.Add(objMealPeriod1);
            var objMealPeriod2 = new MealPeriodDto();
            objMealPeriod2.isSelected = true;
            objMealPeriod2.title = "Lunch";
            objMealPeriod2.openTime = TimeSpan.Parse("12:00");
            objMealPeriod2.closeTime = TimeSpan.Parse("12:00");
            lst.Add(objMealPeriod2);
            var objMealPeriod3 = new MealPeriodDto();
            objMealPeriod3.isSelected = true;
            objMealPeriod3.title = "Dinner";
            objMealPeriod3.openTime = TimeSpan.Parse("12:00");
            objMealPeriod3.closeTime = TimeSpan.Parse("12:00");
            lst.Add(objMealPeriod3);
            var objMealPeriod4 = new MealPeriodDto();
            objMealPeriod4.isSelected = true;
            objMealPeriod4.title = "Brunch";
            objMealPeriod4.openTime = TimeSpan.Parse("12:00");
            objMealPeriod4.closeTime = TimeSpan.Parse("12:00");
            lst.Add(objMealPeriod4);
            return lst;
        }

    }
}
