using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Foundry.Services.PartnerNotificationsLogs;
using Foundry.Domain.ApiModel.PartnerApiModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Net.Http.Headers;
using System.Globalization;

namespace Foundry.Web.ViewComponents
{
    public class NotificationLogsViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public NotificationLogsViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            // await Task.FromResult(0);
            List<PartnerNotificationsLogModel> apinames = new List<PartnerNotificationsLogModel>();
            List<PartnerNotificationsLogModel> status = new List<PartnerNotificationsLogModel>();
            List<ProgramModel> prg = new List<ProgramModel>();
            List<SelectListItem> ddlapilist = new List<SelectListItem>();
            List<SelectListItem> ddlstatuslist = new List<SelectListItem>();
            List<SelectListItem> ddlPrg = new List<SelectListItem>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllApiNames).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    if (response.Result != null)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        apinames = response1.ToObject<List<PartnerNotificationsLogModel>>();
                        foreach (var item in apinames)
                        {
                            var api = new SelectListItem();
                            api.Value = item.ApiName;
                            api.Text = item.ApiName;
                            ddlapilist.Add(api);
                        }
                    }
                }
                var result1 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllStatus).Result;
                if (result1.IsSuccessStatusCode)
                {
                    var response = await result1.Content.ReadAsAsync<ApiResponse>();
                    if (response.Result != null)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        status = response1.ToObject<List<PartnerNotificationsLogModel>>();
                        foreach (var item in status)
                        {
                            var st = new SelectListItem();
                            st.Value = item.Status;
                            st.Text = item.Status;
                            ddlstatuslist.Add(st);
                        }
                    }
                }


                var result2 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetorgAllPrograms + "?organisationId=" + Cryptography.DecryptCipherToPlain( id)).Result;
                if (result2.IsSuccessStatusCode)
                {
                    var response = await result2.Content.ReadAsAsync<ApiResponse>();
                    if (response.Result != null)
                    {
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        prg = response1.ToObject<List<ProgramModel>>();
                        foreach (var item in prg)
                        {
                            var st = new SelectListItem();
                            st.Value =item.id.ToString();
                            st.Text = item.name;
                            ddlPrg.Add(st);
                        }
                    }
                }

                ViewBag.ApiNames = ddlapilist;
                ViewBag.Status = ddlstatuslist;
                ViewBag.Prg = ddlPrg;
                return View();
            }
        }
    }
}
