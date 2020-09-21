using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class SharedJPOSService : ISharedJPOSService
    {
        #region Private Variables

        private string consumerId = "";
        private string version = "";
        private string url = "";
        private string n = "";
        #endregion
        private IGeneralSettingService _generalSettingService;
        private IJPOSCallLogService _jposCallLogService;
        public SharedJPOSService(IGeneralSettingService generalSettingService, IJPOSCallLogService jposCallLogService)
        {
            _generalSettingService = generalSettingService;
            _jposCallLogService = jposCallLogService;
        }

        public async Task<List<OrganizationJPOSDto>> GetAllDataJPOS(string UrlCall, string clientIpAddress, string entityName)
        {
            try
            {
                List<OrganizationJPOSDto> oOrganisations = new List<OrganizationJPOSDto>();
                using (var client = new HttpClient())
                {
                    if (entityName.ToLower().Trim() == "plans" || entityName.ToLower().Trim() == "accounts")
                    {
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Apiary);
                    }
                    else
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Staging);
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {

                        StringBuilder JposRequestCreate = new StringBuilder();
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);
                        var hostURL = new Uri($"" + url + UrlCall);
                        JposRequestCreate.Append("Headers:{version:" + version + ",consumer-id:" + consumerId + ",nonce:" + n + "},URL:{" + hostURL + "}Body:{}");
                        JPOSCallLog oJposLog = new JPOSCallLog();
                        oJposLog.entity = url;
                        oJposLog.httpMethod = "Get";
                        oJposLog.referenceId = "0";
                        oJposLog.requestDateTime = DateTime.UtcNow;
                        oJposLog.requestJPOSContent = JposRequestCreate.ToString();

                        var response = await client.GetAsync(hostURL);
                        string json;
                        using (var content = response.Content)
                        {
                            json = await content.ReadAsStringAsync();
                            dynamic response1 = JsonConvert.DeserializeObject(json.ToString());
                            oOrganisations = response1.Value.ToObject<List<OrganizationJPOSDto>>();
                            oJposLog.responseStatus = response.IsSuccessStatusCode;
                            oJposLog.responseJPOSContent = json;
                            oJposLog.responseDateTime = DateTime.UtcNow;
                            oJposLog.ClientIpAddress = clientIpAddress;
                        }
                        await _jposCallLogService.AddAsync(oJposLog);
                    }
                    return oOrganisations;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task GeneralSettingValuesForJpos(string keyGroup)
        {
            var jposSettings = (await _generalSettingService.GetGeneratSettingValueByKeyGroup(keyGroup)).ToList();
            if (jposSettings.Count > 0)
            {
                foreach (var item in jposSettings)
                {
                    switch (item.KeyName)
                    {
                        case Constants.JPOSConstants.JPOS_Version:
                            version = item.Value;
                            break;
                        case Constants.JPOSConstants.JPOS_ConsumerId:
                            consumerId = item.Value;
                            break;
                        case Constants.JPOSConstants.JPOS_HostURL:
                            url = item.Value;
                            break;
                        case Constants.JPOSConstants.JPOS_N:
                            n = item.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public async Task<OrganizationJPOSDto> GetRespectiveDataJPOS(string UrlCall, string id, string clientIpAddress, string entityName)
        {
            try
            {
                OrganizationJPOSDto oOrganisation = new OrganizationJPOSDto();
                using (var client = new HttpClient())
                {
                    if (entityName.ToLower().Trim() == "plans" || entityName.ToLower().Trim() == "accounts")
                    {
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Apiary);
                    }
                    else
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Staging);
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        StringBuilder JposRequestCreate = new StringBuilder();
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);
                        var hostURL = new Uri($"" + url + UrlCall + id);
                        JposRequestCreate.Append("Headers:{version:" + version + ",consumer-id:" + consumerId + ",nonce:" + n + "},URL:{" + hostURL + "}Body:{}");
                        JPOSCallLog oJposLog = new JPOSCallLog();
                        oJposLog.entity = url;
                        oJposLog.httpMethod = "Get";
                        oJposLog.referenceId = id.ToString();
                        oJposLog.requestDateTime = DateTime.UtcNow;
                        oJposLog.requestJPOSContent = JposRequestCreate.ToString();
                        var response = await client.GetAsync(hostURL);
                        string json;
                        using (var content = response.Content)
                        {
                            json = await content.ReadAsStringAsync();
                            dynamic response1 = JsonConvert.DeserializeObject(json.ToString());
                            oOrganisation = response1.ToObject<OrganizationJPOSDto>();
                            oJposLog.responseStatus = response.IsSuccessStatusCode;
                            oJposLog.responseJPOSContent = json;
                            oJposLog.responseDateTime = DateTime.UtcNow;
                            oJposLog.ClientIpAddress = clientIpAddress;
                        }
                        await _jposCallLogService.AddAsync(oJposLog);
                    }
                }
                return oOrganisation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> PostRespectiveDataJPOS(string UrlCall, object data, string id, string clientIpAddress, string entityName)
        {
            try
            {
                JPOSResponse oOrganisation = new JPOSResponse();
                using (var client = new HttpClient())
                {
                    if (entityName.ToLower().Trim() == "plans" || entityName.ToLower().Trim() == "accounts")
                    {
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Apiary);
                    }
                    else
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Staging);
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        StringBuilder JposRequestCreate = new StringBuilder();
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                        HttpContent stringContent = new StringContent(json);
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var hostURL = new Uri($"" + url + UrlCall + (!string.IsNullOrEmpty(id) ? "/" + id : ""));
                        JposRequestCreate.Append("Headers:{version:" + version + ",consumer-id:" + consumerId + ",nonce:" + n + "},URL:{" + hostURL + "}Body:{" + json + "}");
                        JPOSCallLog oJposLog = new JPOSCallLog();
                        oJposLog.entity = url;
                        oJposLog.requestDateTime = DateTime.UtcNow;
                        oJposLog.requestJPOSContent = JposRequestCreate.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            var response = await client.PutAsync(hostURL, stringContent);
                            oJposLog.referenceId = id.ToString();
                            oJposLog.httpMethod = "Put";
                            oJposLog.responseStatus = response.IsSuccessStatusCode;
                            if (response.IsSuccessStatusCode)
                            {
                                var jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic response1 = JsonConvert.DeserializeObject(jsonResponse.ToString());
                                oOrganisation = response1.ToObject<JPOSResponse>();
                            }
                        }
                        else
                        {
                            var response = await client.PostAsync(hostURL, stringContent);
                            oJposLog.httpMethod = "Post";
                            oJposLog.responseStatus = response.IsSuccessStatusCode;
                            var content = await response.Content.ReadAsStringAsync();
                            oJposLog.responseJPOSContent = content;

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic response1 = JsonConvert.DeserializeObject(jsonResponse.ToString());
                                oOrganisation = response1.ToObject<JPOSResponse>();
                            }
                        }

                       
                        oJposLog.responseDateTime = DateTime.UtcNow;
                        oJposLog.ClientIpAddress = clientIpAddress;
                        await _jposCallLogService.AddAsync(oJposLog);
                    }
                    return oOrganisation.id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteRespectiveDataJPOS(string UrlCall, object data, string id, string clientIpAddress, string entityName)
        {
            try
            {
                JPOSResponse oOrganisation = new JPOSResponse();
                using (var client = new HttpClient())
                {
                    if (entityName.ToLower().Trim() == "plans" || entityName.ToLower().Trim() == "accounts")
                    {
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Apiary);
                    }
                    else
                        await GeneralSettingValuesForJpos(Constants.GeneralSettingsKeyGroup.JPOS_Staging);
                    if (!string.IsNullOrEmpty(consumerId) && !string.IsNullOrEmpty(url))
                    {
                        StringBuilder JposRequestCreate = new StringBuilder();
                        client.DefaultRequestHeaders.Add("version", version);
                        client.DefaultRequestHeaders.Add("consumer-id", consumerId);
                        client.DefaultRequestHeaders.Add("nonce", n);
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                        HttpContent stringContent = new StringContent(json);
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var hostURL = new Uri($"" + url + UrlCall + (!string.IsNullOrEmpty(id) ? "/" + id : ""));
                        JposRequestCreate.Append("Headers:{version:" + version + ",consumer-id:" + consumerId + ",nonce:" + n + "},URL:{" + hostURL + "}Body:{" + data + "}");
                        JPOSCallLog oJposLog = new JPOSCallLog();
                        oJposLog.entity = url;
                        oJposLog.referenceId = id.ToString();
                        oJposLog.requestDateTime = DateTime.UtcNow;
                        oJposLog.requestJPOSContent = JposRequestCreate.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            var response = await client.PutAsync(hostURL, stringContent);
                            oJposLog.httpMethod = "Put/Delete";  //Calling put for delete as soft delete.
                            oJposLog.responseStatus = response.IsSuccessStatusCode;
                            if (response.IsSuccessStatusCode)
                            {
                                var jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic response1 = JsonConvert.DeserializeObject(jsonResponse.ToString());
                                oOrganisation = response1.ToObject<JPOSResponse>();
                            }
                        }


                        oJposLog.responseJPOSContent = json;
                        oJposLog.responseDateTime = DateTime.UtcNow;
                        oJposLog.ClientIpAddress = clientIpAddress;
                        await _jposCallLogService.AddAsync(oJposLog);
                    }

                    return oOrganisation.id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
