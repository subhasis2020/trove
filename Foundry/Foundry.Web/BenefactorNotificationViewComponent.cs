using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Globalization;
using static Foundry.Domain.Constants;

namespace Foundry.Web
{
    [Authorize]
    public class BenefactorNotificationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await GetBenefactorNotifications();
            return View(items);
        }
        [HttpGet]
        public async Task<List<BenefactorNotificationsModel>> GetBenefactorNotifications()
        {
            List<BenefactorNotificationsModel> notify = new List<BenefactorNotificationsModel>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var result = client.GetAsync(GeneralConstants.ServiceURL + ApiConstants.BenefactorNotifications + "?benefactorId=" + Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value)).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    if (response.StatusFlagNum == 1)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        notify = response1.ToObject<List<BenefactorNotificationsModel>>();
                    }
                }
            }
            return notify;
        }
    }
}
