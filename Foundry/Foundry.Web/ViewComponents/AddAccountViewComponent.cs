using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
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
    public class AddAccountViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddAccountViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgId = poId;
            ViewBag.PrimaryProgId = ppId;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PaId = id;
            var items = await GetProgramAccountInformation(id, ppId);
            return View(items);
        }
        [HttpGet]
        public async Task<ProgramAccountModel> GetProgramAccountInformation(string id, string ppId)
        {
            ProgramAccountModel model = new ProgramAccountModel();
            if (!string.IsNullOrEmpty(ppId))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var programAccountId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetProgramAccountDataById + "?id=" + programAccountId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        try
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            model = response1.ToObject<ProgramAccountModel>();
                            model.Jpos_ProgramAccountEncId = !string.IsNullOrEmpty(model?.Jpos_ProgramAccountId) ? Cryptography.EncryptPlainToCipher(model.Jpos_ProgramAccountId) : "";
                            List<SelectListItem> ddlAccountTypelist = model.accountType.AsEnumerable().Select(c => new SelectListItem { Text = c.AccountType, Value = c.Id.ToString() }).ToList();
                            ViewBag.accountType = ddlAccountTypelist;
                            List<SelectListItem> ddlPassTypelist = model.lstPassType.AsEnumerable().Select(c => new SelectListItem { Text = c.Type, Value = c.Id.ToString() }).ToList();
                            ViewBag.lstPassType = ddlPassTypelist;
                            List<SelectListItem> ddlResetPeriodlist = model.ResetPeriod.AsEnumerable().Select(c => new SelectListItem { Text = c.Type, Value = c.Id.ToString() }).ToList();
                            ViewBag.ResetPeriod = ddlResetPeriodlist;
                            List<SelectListItem> ddlExResetPeriodlist = model.ExchangeResetPeriod.AsEnumerable().Select(c => new SelectListItem { Text = c.Type, Value = c.Id.ToString() }).ToList();
                            ViewBag.ExResetPeriod = ddlExResetPeriodlist;
                            List<SelectListItem> ddlResetDayofWeeklist = model.WeekDays.AsEnumerable().Select(c => new SelectListItem { Text = c.DayName, Value = c.Id.ToString() }).ToList();
                            ViewBag.WeekDays = ddlResetDayofWeeklist;
                        }
                        catch (Exception ex)
                        {
                            HttpContext.RiseError(new Exception(string.Concat("Web := (ViewComponent := AddAccountViewComponent)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                        }

                    }
                }
            }
            return model;
        }
    }
}
