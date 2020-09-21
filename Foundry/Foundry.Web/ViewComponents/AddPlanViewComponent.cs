using Foundry.Domain;
using Foundry.Domain.ApiModel;
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

namespace Foundry.Web.ViewComponents
{
    public class AddPlanViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public AddPlanViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgId = poId;
            ViewBag.PrimaryProgId = ppId;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.PrimaryOrgName = poN;
            var items = await GetPlanInformation(id, ppId);
            return View(items);
        }
        [HttpGet]
        public async Task<PlanModel> GetPlanInformation(string id, string ppId)
        {
            PlanModel orgModel = new PlanModel();
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    orgModel = await AddPlanViewComponentRefactor(id, ppId, orgModel, client);
                }
            }
            return orgModel;
        }

        private async Task<PlanModel> AddPlanViewComponentRefactor(string id, string ppId, PlanModel orgModel, HttpClient client)
        {
            var planId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
            var programId = !string.IsNullOrEmpty(ppId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(ppId)) : 0;
            var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetPlanDataById + "?planId=" + planId + "&programId=" + programId).Result;
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsAsync<ApiResponse>();
                dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                orgModel = response1.ToObject<PlanModel>();
                orgModel.Jpos_PlanEncId = !string.IsNullOrEmpty(orgModel.Jpos_PlanId) ? Cryptography.EncryptPlainToCipher(orgModel.Jpos_PlanId) : "";
                if (!string.IsNullOrEmpty(orgModel.description))
                    orgModel.description = orgModel.description.Replace("<br/>", "\n").Replace("<br/>", Environment.NewLine);
                if (planId > 0)
                    orgModel.selectedProgramAccount = orgModel.planProgramAccount.Select(x => x.programAccountId);
            }

            return orgModel;
        }
    }
}
