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

namespace Foundry.Web.ViewComponents
{
    public class MerchantDetailViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public MerchantDetailViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string poN)
        {
            var items = await GetMerchantDetailInformation(id, ppId, poId).ConfigureAwait(false);
            ViewBag.MerchantId = id;
            ViewBag.PrimaryOrgId = poId;
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PpId = ppId;
            ViewBag.UserEntityImageType = Convert.ToInt32(Constants.PhotoType.Organisation);
            if (Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) == 0)
            {
                items.BusinessTypeId = 1;
            }
            ViewBag.BaseL = _configuration["ServiceAPIURL"];
            return View(items);
        }
        [HttpGet]
        public async Task<MerchantDetailModel> GetMerchantDetailInformation(string id, string ppId, string poId)
        {
            MerchantDetailModel orgModel = new MerchantDetailModel();
            int primaryprogId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(ppId));
            int primaryOrgId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(poId));
            if (!string.IsNullOrEmpty(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var organisationId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetMerchantDetailInfoWithProgNAccType + "?organisationId=" + organisationId + "&universityId=" + primaryOrgId+ "&programId="+ primaryprogId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        orgModel = response1.ToObject<MerchantDetailModel>();
                        ViewBag.PrimaryProgramName= orgModel.Program.Where(x => x.Id == primaryprogId).Select(x => x.Name).FirstOrDefault();
                      
                        var primaryProgramId = orgModel.OrgProgram.FirstOrDefault(x => x.IsPrimaryAssociation);
                        if (primaryProgramId != null)
                        {
                            orgModel.PrimaryProgramId = orgModel.Program.Where(x => x.Id == primaryProgramId.ProgramId).Select(x => x.Id).FirstOrDefault();
                            orgModel.PrimaryProgramName = orgModel.Program.Where(x => x.Id == primaryProgramId.ProgramId).Select(x => x.Name).FirstOrDefault();
                            orgModel.Program = orgModel.Program.Where(x => x.Id != primaryProgramId.ProgramId).ToList();
                        }
                        else {
                            orgModel.PrimaryProgramId = orgModel.Program.Where(x => x.Id == primaryprogId).Select(x => x.Id).FirstOrDefault();
                            orgModel.PrimaryProgramName = orgModel.Program.Where(x => x.Id == primaryprogId).Select(x => x.Name).FirstOrDefault();
                            orgModel.Program = orgModel.Program.Where(x => x.Id != primaryprogId).ToList();
                        }
                      
                        orgModel.SelectedOrgProgram = orgModel.OrgProgram.Where(x => !x.IsPrimaryAssociation).Select(x => x.ProgramId);
                        orgModel.SelectedOrgAccType = orgModel.OrgAccType.Where(x => x.ProgramId == primaryprogId).Select(x => x.Id);
                        if (!string.IsNullOrEmpty(orgModel.Description))
                            orgModel.Description = orgModel.Description.Replace("<br/>", "\n").Replace("<br/>", Environment.NewLine);
                    }
                    orgModel.Jpos_MerchantEncId = !string.IsNullOrEmpty(orgModel.Jpos_MerchantId) ? Cryptography.EncryptPlainToCipher(orgModel.Jpos_MerchantId) : "";
                }
            }
            return orgModel;
        }
    }
}
