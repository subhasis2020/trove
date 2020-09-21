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
    public class ProgramInfoViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public ProgramInfoViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgId = poId;
            ViewBag.ProgramId = id;
            ViewBag.ProgramName = ppN;
            ViewBag.PrimaryOrgName = poN;
            ProgramInfoModel programDetail = new ProgramInfoModel();
            var timeZones = GeneralSettingData.GetTimeZonesInfo();
            List<sys.SelectListItem> ddltimeZoneitemlist = timeZones.Select(c => new sys.SelectListItem { Text = c.Key, Value = c.Value.ToString() }).ToList();//, Selected = selectedIds.Contains(c.Id) ? true : false
            ViewBag.TimeZones = ddltimeZoneitemlist;
            var dataTypesDDL = GeneralSettingData.GetDataTypesOfDB();
            List<sys.SelectListItem> ddlDataTypeItemlist = dataTypesDDL.Select(c => new sys.SelectListItem { Text = c.Key, Value = c.Value.ToString() }).ToList();//, Selected = selectedIds.Contains(c.Id) ? true : false
            ViewBag.DataTypesDDL = ddlDataTypeItemlist;
            var IsRequiredDDL = GeneralSettingData.GetDataForIsRequired();
            List<sys.SelectListItem> ddlIsRequiredItemlist = IsRequiredDDL.Select(c => new sys.SelectListItem { Text = c.Key, Value = c.Value.ToString() }).ToList();//, Selected = selectedIds.Contains(c.Id) ? true : false
            ViewBag.IsRequiredDDL = ddlIsRequiredItemlist;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var programInfo = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramInfoById + "?prgId=" + (!string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0)).Result;
                if (programInfo.IsSuccessStatusCode)
                {
                    var response = await programInfo.Content.ReadAsAsync<ApiResponse>();
                    dynamic prgResponse = JsonConvert.DeserializeObject(response.Result.ToString());
                    programDetail = prgResponse.ToObject<ProgramInfoModel>();
                }
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var programresult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramType).Result;
                if (programresult.IsSuccessStatusCode)
                {
                    var response = await programresult.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var programtype = response1.ToObject<List<ProgramTypesDto>>();
                    List<ProgramTypesDto> programTypeItemList = programtype;
                    List<sys.SelectListItem> ddlitemlist = programTypeItemList.Select(c => new sys.SelectListItem { Text = c.ProgramTypeName, Value = c.Id.ToString(), Selected = false }).ToList();
                    ViewBag.ProgramType = ddlitemlist;
                }

                //Generate Issuer properties

                string programidString = programDetail.ProgramCodeId;                
                int programid = 0;
                if (int.TryParse(programidString, out int output))
                {
                    programid = output;
                }
                var issuerResult = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetIssuerProperties + "?ProgramId="+ programid + "").Result;// + Convert.ToInt32(Cryptography.DecryptCipherToPlain(id))).Result;
                if (issuerResult.IsSuccessStatusCode)
                {
                    var response = await issuerResult.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    IssuerDetails issuerDetails = response1.ToObject<IssuerDetails>();
                    programDetail.IssuerProps = new List<IssuerProp>();
                    if (issuerDetails.issuer != null)
                    {
                        foreach (var keyValue in issuerDetails?.issuer?.issuerprop)
                        {
                            programDetail.IssuerProps.Add(new IssuerProp(keyValue.Key, keyValue.Value));
                        }
                    }
                }
            }
            return View(programDetail);
        }
    }
}
