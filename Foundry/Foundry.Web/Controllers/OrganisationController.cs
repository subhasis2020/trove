using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Foundry.Domain.ApiModel;
using Foundry.Domain;
using Newtonsoft.Json;
using ElmahCore;
using System.Net.Http.Headers;
using Foundry.Web.Models;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using AutoMapper;
using static Foundry.Domain.Constants;
using System.Linq.Dynamic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class contains the methods for organisation.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(CustomActionAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class OrganisationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        /// <summary>
        /// This constructor is used to inject the services in the class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        public OrganisationController(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        /// <summary>
        /// This is the default method for the controller.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method is used to get the organisations list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Super Admin, Organization Full")]
        public async Task<IActionResult> GetOrganisations()
        {
            try
            {
                await Task.FromResult(1);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (Organisations - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }
        /// <summary>
        /// This method is used to show the form for create an organization.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Super Admin, Organization Full")]
        public async Task<IActionResult> CreateOrganisations(string id, string org)
        {
            try
            {
                
                ViewBag.OrganisationId = id;
                if(_configuration["SodexhoOrgId"] == Cryptography.DecryptCipherToPlain(id))
                {
                    ViewBag.OrgId = _configuration["SodexhoOrgId"];
                }
                else
                {
                    ViewBag.OrgId = "1111111";

                }
                
                ViewBag.OrgName = !string.IsNullOrEmpty(org) ? Cryptography.DecryptCipherToPlain(org) : "";
                ViewBag.ReportURL = _configuration["ReportURL"] + "PlanBalanceDetail/PlanBalance.aspx?orgId=" + id;
                await Task.FromResult(1);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (Organisations - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }
        /// <summary>
        /// This method is the post method for posting organization detail information on submission.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostOrganisationDetailInformation(OrganisationDetailModel model)
        {
            string resultId = "0";
            if (ModelState.IsValid)
            {
                try
                {
                    OrganisationViewModel dataOrganisationDetail = DataModalBindingForOrganization(model);
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage result = await PostOrganizationDetailInfoRefactor(dataOrganisationDetail, client);

                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                resultId = Convert.ToString(response.Result);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (PostOrganisationDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId, dataOrgAppend = Cryptography.EncryptPlainToCipher(model.OrganisationSubTitle) });
        }

        /// <summary>
        /// This method is used to load account holders.
        /// </summary>
        /// <param name="prgId"></param>
        /// <param name="orgId"></param>
        /// <param name="planId"></param>
        /// <param name="currentPageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAccountHoldersByOrganization(string prgId, string orgId, string planId, int currentPageNumber)
        {
            var drawACH = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startACH = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthACH = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnACH = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionACH = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValueACH = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizeACH = lengthACH != null ? Convert.ToInt32(lengthACH) : 0;

                int recordsTotalACH = 0;
                List<AccountHolderDto> userAccountHolder = new List<AccountHolderDto>();
                var organisationIdCHA = !string.IsNullOrEmpty(orgId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(orgId)) : 0;
                var programIdCHA = !string.IsNullOrEmpty(prgId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(prgId)) : 0;
                var pageNumberCHA = (Convert.ToInt32(startACH) / pageSizeACH) + 1;
                using (var clientACH = new HttpClient())
                {
                    clientACH.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientACH.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultACH = clientACH.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAccountHoldersListByOrganization + "?organisationId=" + organisationIdCHA + "&programId=" + programIdCHA + "&searchValue=" + searchValueACH + "&pageNumber=" + pageNumberCHA + "&pageSize=" + pageSizeACH + "&sortColumnName=" + sortColumnACH + "&sortOrderDirection=" + sortColumnDirectionACH + "&planId=" + (!string.IsNullOrEmpty(planId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(planId)) : 0)).Result;
                    if (resultACH.IsSuccessStatusCode)
                    {
                        var responseACH = await resultACH.Content.ReadAsAsync<ApiResponse>();
                        if (responseACH.StatusFlagNum == 1)
                        {
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseACH.Result.ToString());
                            userAccountHolder = responseDesACH.ToObject<List<AccountHolderDto>>();
                            userAccountHolder.ForEach(x => { x.UserEncId = Cryptography.EncryptPlainToCipher(x.Id.ToString());
                                x.Jpos_AccountEncId = !string.IsNullOrEmpty(x.Jpos_AccountHolderId) 
                                ? Cryptography.EncryptPlainToCipher(x.Jpos_AccountHolderId) : "";
                                x.ProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString());
                            });
                        }
                    }
                }
                // Getting all Account holder Programs data
                var userAccountHolderData = (from tempUserAccountHolder in userAccountHolder select tempUserAccountHolder);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnACH) && string.IsNullOrEmpty(sortColumnDirectionACH)))
                {
                    userAccountHolderData = userAccountHolderData.OrderBy(sortColumnACH + " " + sortColumnDirectionACH);
                }
                //total number of rows counts
                recordsTotalACH = userAccountHolder.Count;


                return Json(new { draw = drawACH, recordsFiltered = recordsTotalACH, recordsTotal = recordsTotalACH, data = userAccountHolderData.Skip(int.Parse(startACH)).Take(int.Parse(lengthACH)).ToList() });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAccountHolders - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawACH, recordsFiltered = 0, recordsTotal = 0, data = new List<AccountHolderDto>() });
            }
        }

        /// <summary>
        /// This method is used to get the accountholder list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult AccountHolderList(string id, string poId, string ppN, string poN, string orgId = "")
        {
            return ViewComponent("AccountHolderListOrganization", new { id = id, poId = poId, ppN = ppN, poN = poN, orgId = orgId });
        }

        /// <summary>
        /// Update Issuers
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateIssuer()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    HttpContent stringContent = new StringContent("");
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UpdateIssuer, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        Issuers issuerDetails = response1.ToObject<Issuers>();
                    }
                }
            }
            catch(Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (PostOrganisationDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
           // HttpResponseMessage result = await PostOrganizationDetailInfoRefactor(dataOrganisationDetail, client);
            return Json(new { data = "" });
        }

        private async Task<HttpResponseMessage> PostOrganizationDetailInfoRefactor(OrganisationViewModel dataOrganisationDetail, HttpClient client)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataOrganisationDetail);
            HttpContent stringContent = new StringContent(json);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

            var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateOrganisationInfo, stringContent);
            return result;
        }

        private OrganisationViewModel DataModalBindingForOrganization(OrganisationDetailModel model)
        {
            model.OrganisationProgramType = new List<ProgramTypeIdDto>();
            foreach (var item in model.SelectedOrganisationProgramType)
            {
                ProgramTypeIdDto objprogramTypeId = new ProgramTypeIdDto();
                objprogramTypeId.ProgramTypeId = item;
                model.OrganisationProgramType.Add(objprogramTypeId);
            }
            var dataOrganisationDetail = new OrganisationViewModel
            {
                addressLine1 = model.Address,
                ContactName = model.ContactName,
                contactNumber = model.ContactNumber,
                ContactTitle = model.ContactTitle,
                description = !string.IsNullOrEmpty(model.Description) ? model.Description.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>") : null,
                emailAddress = model.EmailAddress,
                OrganisationSubTitle = model.OrganisationSubTitle,
                id = !string.IsNullOrEmpty(model.OrganisationId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.OrganisationId)) : 0,
                name = model.OrganisationName,
                OrganisationProgramTypes = _mapper.Map<List<OrganisationProgramTypeModel>>(model.OrganisationProgramType),
                websiteURL = model.Website,
                organisationType = OrganasationType.University,
                facebookURL = model.FacebookURL,
                twitterURL = model.TwitterURL,
                skypeHandle = model.SkypeHandle,
                JPOS_OrgId = model.JPOS_MerchantId
            };
            return dataOrganisationDetail;
        }

        /// <summary>
        /// This method is used to post organisation program information.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="orgnisationId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostOrganisationProgramsDetailInformation(List<int> programId, string orgnisationId)
        {
            string resultId = "0";
            if (ModelState.IsValid)
            {
                try
                {
                    List<OrganisationProgramDBDto> model = new List<OrganisationProgramDBDto>();
                    foreach (var item in programId)
                    {
                        model.Add(new OrganisationProgramDBDto { programId = item, organisationId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(orgnisationId)) });
                    }
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        HttpContent stringContent = new StringContent(json);
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                        var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateOrganisationPrograms, stringContent);
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                resultId = Convert.ToString(response.Result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (PostOrganisationProgramsDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId });
        }
        /// <summary>
        /// This method shows the organization programs list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Super Admin, Organization Full")]
        public IActionResult OrganisationsPrograms(string id, string org)
        {
            ViewBag.OrganisationId = id;
            ViewBag.OrganisationName = !string.IsNullOrEmpty(org) ? Cryptography.DecryptCipherToPlain(org) : "";
            return View();
        }
        /// <summary>
        /// This method is used to get the user admin based on id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full")]
        [HttpGet]
        public async Task<IActionResult> UserAdminInfo(int userId)
        {
            UserWithProgramTypeDto user = new UserWithProgramTypeDto();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.AdminUserInfoNProgramType + "?userId=" + userId).Result;

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    user = response1.ToObject<UserWithProgramTypeDto>();
                }

            }
            return Json(new { data = user });
        }
        /// <summary>
        /// This method is used to get the organistion detail view component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetOrganisationDetailViewComponent(string id)
        {
            return ViewComponent("OrganisationDetail", new { idEnc = id });
        }
        /// <summary>
        /// This method is used to get the organisation global settings detail view component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetOrganisationGlobalSettingViewComponent(string id)
        {
            return ViewComponent("OrganisationGlobalSetting", new { id });
        }
        /// <summary>
        /// This method is used to get the notification logs detail view component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetNotificationLogsViewComponent(string id)
        {
            return ViewComponent("NotificationLogs", new { id });
        }
        /// <summary>
        /// This method is used to get organisation program detail view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult GetOrganisationProgramDetailViewComponent(string id, string name)
        {
            return ViewComponent("OrganisationProgram", new { id });
        }
        /// <summary>
        /// This method is used to get the organization admin level view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult GetOrganisationAdminLevelViewComponent(string id, string name)
        {
            return ViewComponent("OrganisationAdminLevel", new { id });
        }
        /// <summary>
        /// This method is used to get the added organization program view component.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetAddedOrganisationProgramViewComponent(string id)
        {
            return ViewComponent("AddedOrganisationProgram", new { id = id });
        }
        /// <summary>
        /// This method is used to get the organization list view component. 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IActionResult GetOrganisationListViewComponent(string search)
        {
            return ViewComponent("OrganisationList", new { searchOrgText = search });
        }



        /// <summary>
        /// This method is used to load all notification logs.
        /// </summary>
        /// <param name="currentPageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllNotificationLogs(int currentPageNumber)
        {
            var drawlogs= HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startlogs = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthlogs = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnlogs = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionlogs = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
              //  var searchValueOrgAdmin = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizelogs = lengthlogs != null ? Convert.ToInt32(lengthlogs) : 0;

                int skiplogs = startlogs != null ? Convert.ToInt32(startlogs) : 0;
                var pageNumberCHA = (Convert.ToInt32(startlogs) / pageSizelogs) + 1;
                long recordsTotallogs = 0;
                List<PartnerNotificationsLogDto> logs = new List<PartnerNotificationsLogDto>();
                using (var clientACH = new HttpClient())
                {
                    clientACH.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientACH.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultACH = clientACH.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllNotificationLogs + "?pageNumber=" + pageNumberCHA + "&pageSize=" + pageSizelogs + "&sortColumnName=" + sortColumnlogs + "&sortOrderDirection=" + sortColumnDirectionlogs).Result;
                    if (resultACH.IsSuccessStatusCode)
                    {
                        var responseACH = await resultACH.Content.ReadAsAsync<ApiResponse>();
                      //  if (responseACH.StatusFlagNum == 1)
                       // {
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseACH.Result.ToString());
                            logs = responseDesACH.ToObject<List<PartnerNotificationsLogDto>>();
                       // }
                    }
                }
                // Getting all Organisation Programs data
                var logsData = (from templogs in logs select templogs);

                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumnlogs) && string.IsNullOrEmpty(sortColumnDirectionlogs)))
                //{
                //    logsData = logsData.OrderBy(sortColumnlogs + " " + sortColumnDirectionlogs);
                //}

                //total number of rows counts
                if (logs.Count > 0)
                {
                    recordsTotallogs = Convert.ToInt64(logs.FirstOrDefault().TotalCount);
                }
            
                //Paging
              //  var data = logsData.Skip(skiplogs).Take(pageSizelogs).ToList();
                return Json(new { draw = drawlogs, recordsFiltered = recordsTotallogs, recordsTotal = recordsTotallogs, data =logsData});
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (LoadAllNotificationsLogs - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawlogs, recordsFiltered = 0, recordsTotal = 0, data = new List<PartnerNotificationsLogDto>() });
            }
           // return Json(new {  data = new List<OrganisationsAdminsDto>() });

        }
        /// <summary>
        /// This method is used to load all notification logs.
        /// </summary>
        /// <param name="currentPageNumber"></param>
        /// <returns></returns>

        public async Task<IActionResult> LoadAllNotificationLogsWIthFiter(string apiname,string status,string date,string programid)
        {
            var drawlogs = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startlogs = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthlogs = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnlogs = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionlogs = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                //  var searchValueOrgAdmin = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizelogs = lengthlogs != null ? Convert.ToInt32(lengthlogs) : 0;

                int skiplogs = startlogs != null ? Convert.ToInt32(startlogs) : 0;
                var pageNumberCHA = (Convert.ToInt32(startlogs) / pageSizelogs) + 1;
                long recordsTotallogs = 0;
                List<PartnerNotificationsLogDto> logs = new List<PartnerNotificationsLogDto>();
                using (var clientACH = new HttpClient())
                {
                    clientACH.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientACH.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultACH = clientACH.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllNotificationLogsWithFilter + "?pageNumber=" + pageNumberCHA + "&pageSize=" + pageSizelogs + "&sortColumnName=" + sortColumnlogs + "&sortOrderDirection=" + sortColumnDirectionlogs + "&apiname=" + apiname + "&status=" + status + "&date=" + date + "&programid=" + programid).Result;
                    if (resultACH.IsSuccessStatusCode)
                    {
                        var responseACH = await resultACH.Content.ReadAsAsync<ApiResponse>();
                        //  if (responseACH.StatusFlagNum == 1)
                        // {
                        dynamic responseDesACH = JsonConvert.DeserializeObject(responseACH.Result.ToString());
                        logs = responseDesACH.ToObject<List<PartnerNotificationsLogDto>>();
                        // }
                    }
                }
                // Getting all Organisation Programs data
                var logsData = (from templogs in logs select templogs);

                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumnlogs) && string.IsNullOrEmpty(sortColumnDirectionlogs)))
                //{
                //    logsData = logsData.OrderBy(sortColumnlogs + " " + sortColumnDirectionlogs);
                //}

                //total number of rows counts
                if (logs.Count > 0)
                {
                    recordsTotallogs = Convert.ToInt64(logs.FirstOrDefault().TotalCount);
                }

                //Paging
                //  var data = logsData.Skip(skiplogs).Take(pageSizelogs).ToList();
                return Json(new { draw = drawlogs, recordsFiltered = recordsTotallogs, recordsTotal = recordsTotallogs, data = logsData });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (LoadAllNotificationsLogs - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawlogs, recordsFiltered = 0, recordsTotal = 0, data = new List<PartnerNotificationsLogDto>() });
            }
            // return Json(new {  data = new List<OrganisationsAdminsDto>() });

        }
        /// <summary>
        /// This method is used to load all program of the organisation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllOrgProgram(string id)
        {
            var drawOrgPrg = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startOrgPrg = Request.Form["start"].FirstOrDefault();
                //Paging Length 10,20
                var lengthOrgPrg = Request.Form["length"].FirstOrDefault();
                //Sort Column Name
                var sortColumnOrgPrg = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                //Sort Column Direction(asc,desc)
                var sortColumnDirectionOrgPrg = Request.Form["order[0][dir]"].FirstOrDefault();
                //Search Value from (Search box)
                var searchValueOrgPrg = Request.Form["search[value]"].FirstOrDefault();
                //Paging Size (10, 20, 50, 100)
                int pageSizeOrgPrg = lengthOrgPrg != null ? Convert.ToInt32(lengthOrgPrg) : 0;
                int skip = startOrgPrg != null ? Convert.ToInt32(startOrgPrg) : 0;
                int recordsTotalOrgPrg = 0;
                List<OrganisationProgramDto> organisationPrograms = new List<OrganisationProgramDto>();
                var organisationIdOrgPrg = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                if (organisationIdOrgPrg > 0)
                {
                    using (var clientOrgPrg = new HttpClient())
                    {
                        clientOrgPrg.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        clientOrgPrg.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                        var resultOrgPrg = clientOrgPrg.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationPrograms + "?organisationId=" + organisationIdOrgPrg).Result;
                        if (resultOrgPrg.IsSuccessStatusCode)
                        {
                            var responseOrgPrg = await resultOrgPrg.Content.ReadAsAsync<ApiResponse>();
                            if (responseOrgPrg.StatusFlagNum == 1)
                            {
                                dynamic responseDesOrgPrg = JsonConvert.DeserializeObject(responseOrgPrg.Result.ToString());
                                organisationPrograms = responseDesOrgPrg.ToObject<List<OrganisationProgramDto>>();
                                organisationPrograms.ForEach(x => { x.strProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()); x.EncProgName = Cryptography.EncryptPlainToCipher(x.ProgramName); });
                            }
                        }
                    }
                }
                // Getting all Organisation Programs data
                var orgProgramData = (from tempOrgPrg in organisationPrograms select tempOrgPrg);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnOrgPrg) && string.IsNullOrEmpty(sortColumnDirectionOrgPrg)))
                {
                    orgProgramData = orgProgramData.OrderBy(sortColumnOrgPrg + " " + sortColumnDirectionOrgPrg);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValueOrgPrg))
                {
                    orgProgramData = orgProgramData.Where(o => o.ProgramName.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueOrgPrg.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.ProgramCodeId.ToLower().Trim().Contains(searchValueOrgPrg.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.ProgramType.ToLower().Trim().Contains(searchValueOrgPrg.ToLower(CultureInfo.InvariantCulture).Trim())
                    || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), DateSuffixModel.GetSuffix(o.DateAdded.Day.ToString())).ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueOrgPrg.ToLower(CultureInfo.InvariantCulture).Trim()));
                }
                //total number of rows counts
                recordsTotalOrgPrg = orgProgramData.Count();
                //Paging
                var dataOrgPrg = orgProgramData.Skip(skip).Take(pageSizeOrgPrg).ToList();
                return Json(new { draw = drawOrgPrg, recordsFiltered = recordsTotalOrgPrg, recordsTotal = recordsTotalOrgPrg, data = dataOrgPrg });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (Organisations - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawOrgPrg, recordsFiltered = 0, recordsTotal = 0, data = new List<OrganisationProgramDto>() });
            }
        }
        /// <summary>
        /// This method is used to load all organization admins.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllOrgAdmins(string id)
        {
            var drawOrgAdmin = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startOrgAdmin = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthOrgAdmin = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnOrgAdmin = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionOrgAdmin = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValueOrgAdmin = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizeOrgAdmin = lengthOrgAdmin != null ? Convert.ToInt32(lengthOrgAdmin) : 0;

                int skipOrgAdmin = startOrgAdmin != null ? Convert.ToInt32(startOrgAdmin) : 0;

                int recordsTotalOrgAdmin = 0;
                List<OrganisationsAdminsDto> orgAdmins = new List<OrganisationsAdminsDto>();
                using (var clientOrgAdmin = new HttpClient())
                {
                    clientOrgAdmin.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientOrgAdmin.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var organisationIdOrgAdmin = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    if (organisationIdOrgAdmin > 0)
                    {
                        var resultOrgAdmin = clientOrgAdmin.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationsAdminInfo + "?organisationId=" + organisationIdOrgAdmin).Result;
                        if (resultOrgAdmin.IsSuccessStatusCode)
                        {
                            var responseOrgAdmin = await resultOrgAdmin.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDescOrgAdmin = JsonConvert.DeserializeObject(responseOrgAdmin.Result.ToString());
                            orgAdmins = responseDescOrgAdmin.ToObject<List<OrganisationsAdminsDto>>();
                            orgAdmins.Where(x => x.RoleName != null).ToList().ForEach(x => x.RoleName = x.RoleName.ToLower(CultureInfo.InvariantCulture).Trim().Replace("organisation ", "").Replace("organization ", "").ToUpper(CultureInfo.InvariantCulture));
                            orgAdmins.Where(x => x.ProgramsAccessibility != null).ToList().ForEach(x => x.ProgramsAccessibility = x.ProgramsAccessibility.Replace(",", "<br/>"));
                        }
                    }
                }
                // Getting all Organisation Programs data
                var orgAdminData = (from tempOrgAdmin in orgAdmins select tempOrgAdmin);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnOrgAdmin) && string.IsNullOrEmpty(sortColumnDirectionOrgAdmin)))
                {
                    orgAdminData = orgAdminData.OrderBy(sortColumnOrgAdmin + " " + sortColumnDirectionOrgAdmin);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValueOrgAdmin))
                {
                    orgAdminData = orgAdminData.Where(o => o.Name.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueOrgAdmin.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.EmailAddress.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueOrgAdmin.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.PhoneNumber.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueOrgAdmin.ToLower(CultureInfo.InvariantCulture).Trim()));
                }

                //total number of rows counts
                recordsTotalOrgAdmin = orgAdminData.Count();
                //Paging
                var data = orgAdminData.Skip(skipOrgAdmin).Take(pageSizeOrgAdmin).ToList();
                return Json(new { draw = drawOrgAdmin, recordsFiltered = recordsTotalOrgAdmin, recordsTotal = recordsTotalOrgAdmin, data });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (LoadAllOrgAdmins - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawOrgAdmin, recordsFiltered = 0, recordsTotal = 0, data = new List<OrganisationsAdminsDto>() });
            }
        }
        /// <summary>
        /// This method is used to delete organization program.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteOrganisationProgram(int programId, string organisationId)
        {
            int resultIdOrgPrgDel = 0;
            try
            {
                var orgPrgDto = new OrganisationProgramDBDto
                {
                    organisationId = !string.IsNullOrEmpty(organisationId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(organisationId)) : 0,
                    programId = programId
                };
                using (var clientOrgPrgDel = new HttpClient())
                {
                    string jsonOrgPrgDel = Newtonsoft.Json.JsonConvert.SerializeObject(orgPrgDto);
                    HttpContent stringContentOrgPrgDel = new StringContent(jsonOrgPrgDel);
                    stringContentOrgPrgDel.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientOrgPrgDel.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultOrgPrgDel = await clientOrgPrgDel.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteOrganisationsProgram, stringContentOrgPrgDel);
                    if (resultOrgPrgDel.IsSuccessStatusCode)
                    {
                        var responseOrgPrg = await resultOrgPrgDel.Content.ReadAsAsync<ApiResponse>();
                        if (responseOrgPrg.StatusFlagNum == 1)
                        {
                            resultIdOrgPrgDel = Convert.ToInt32(responseOrgPrg.Result);
                        }
                    }
                }
                return Json(new { data = resultIdOrgPrgDel, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (DeleteOrganisationProgram - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdOrgPrgDel, success = false });
            }
        }
        /// <summary>
        /// This method is used to delete an organization.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="JposId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteOrganisation(string organisationId, string JposId)
        {
            int resultIdDelOrg = 0;
            try
            {
                using (var clientDelOrg = new HttpClient())
                {
                    var orgDtoDelOrg = new OrganisationDto
                    {
                        Id = !string.IsNullOrEmpty(organisationId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(organisationId)) : 0,
                        JPOS_MerchantId = !string.IsNullOrEmpty(JposId) ? Cryptography.DecryptCipherToPlain(JposId) : ""
                    };
                    string jsonDelOrg = Newtonsoft.Json.JsonConvert.SerializeObject(orgDtoDelOrg);
                    HttpContent stringContentDelOrg = new StringContent(jsonDelOrg);
                    stringContentDelOrg.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelOrg.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultDelOrg = await clientDelOrg.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteOrganisation, stringContentDelOrg);
                    if (resultDelOrg.IsSuccessStatusCode)
                    {
                        var responseDelOrg = await resultDelOrg.Content.ReadAsAsync<ApiResponse>();
                        resultIdDelOrg = Convert.ToInt32(responseDelOrg.Result);
                    }
                }
                return Json(new { data = resultIdDelOrg, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (DeleteOrganisationProgram - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdDelOrg, success = false });
            }


        }
        /// <summary>
        /// This method is used to delete organization admin user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteOrganisationAdminUser(string userId)
        {
            int resultIdDelOrgAdmin = 0;
            try
            {
                using (var clientDelOrgAdmin = new HttpClient())
                {
                    var usrDtoDelOrgAdmin = new UserDto
                    {
                        Id = Convert.ToInt32(userId),
                    };
                    string jsonDelOrgAdmin = Newtonsoft.Json.JsonConvert.SerializeObject(usrDtoDelOrgAdmin);

                    HttpContent stringContentDelOrgAdmin = new StringContent(jsonDelOrgAdmin);
                    stringContentDelOrgAdmin.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelOrgAdmin.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultDelOrgAdmin = await clientDelOrgAdmin.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteAdminUser, stringContentDelOrgAdmin);
                    if (resultDelOrgAdmin.IsSuccessStatusCode)
                    {
                        var responseDelOrgAdmin = await resultDelOrgAdmin.Content.ReadAsAsync<ApiResponse>();
                        resultIdDelOrgAdmin = Convert.ToInt32(responseDelOrgAdmin.Result);
                    }
                }
                return Json(new { data = resultIdDelOrgAdmin, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (DeleteOrganisationAdminUser - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdDelOrgAdmin, success = false });
            }


        }
        /// <summary>
        /// This method is used to post the organization admin detail information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostOrganisationAdminDetailInformation(OrganisationAdminDetailModel model)
        {
            string resultId = "0";
            string message = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var dataOrganisationAdminDetail = new OrganisationAdminViewDetail
                    {
                        EmailAddress = model.adminLevelModel.EmailAddress,
                        LastName = model.adminLevelModel.LastName,
                        Name = model.adminLevelModel.Name,
                        PhoneNumber = model.adminLevelModel.PhoneNumber,
                        ProgramsAccessibility = model.ProgramsAccessibility.Count == 1 && model.ProgramsAccessibility.FirstOrDefault().ProgramTypeId == 0 ? new List<OrganisationProgramIdModel>() : _mapper.Map<List<OrganisationProgramIdModel>>(model.ProgramsAccessibility),
                        OrganisationId = !string.IsNullOrEmpty(model.OrganisationId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.OrganisationId)) : 0,
                        RoleId = model.adminLevelModel.RoleId,
                        UserId = model.adminLevelModel.UserId != null ? model.adminLevelModel.UserId.Value : 0,
                        UserImagePath = model.adminLevelModel.UserImagePath,
                        Custom1 = !string.IsNullOrEmpty(model.Custom1) ? model.Custom1 : string.Empty,
                        MerchantAccessibility = new List<MerchantIdModel>(),
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataOrganisationAdminDetail);
                        HttpContent stringContent = new StringContent(json);
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                        var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateAdminUser, stringContent);

                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();

                            resultId = Convert.ToString(response.Result);
                            message = Convert.ToString(response.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (PostOrganisationAdminDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId, message });
        }
        /// <summary>
        /// This method is used to change the admin status to active or inactive
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeOrganisationAdminStatus(int userId, bool isActive)
        {
            int resultId = 0;
            try
            {
                OrganisationAdminStatusDto adminStatus = new OrganisationAdminStatusDto
                {
                    UserId = userId,
                    isActive = isActive
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(adminStatus);

                    HttpContent stringContent = new StringContent(json);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UpdateOrganisationAdminStatus, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToInt32(response.Result);
                        }
                    }
                }
                return Json(new { data = resultId, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (ChangeOrganisationAdminStatus - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultId, success = false });
            }
        }
        /// <summary>
        /// This method is used to remove an image from web and s3
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveImage(string userId, string imgPath)
        {
            int resultIdRemoveImgOrg = 0;
            try
            {
                using (var clientRemImgOrg = new HttpClient())
                {
                    var photoDto = new PhotosDto()
                    {
                        entityId = Convert.ToInt32(userId),
                        photoType = Convert.ToInt32(Constants.PhotoType.UserProfile),
                        photoPath = imgPath
                    };
                    string jsonRemImgOrg = Newtonsoft.Json.JsonConvert.SerializeObject(photoDto);
                    HttpContent stringContentRemImgOrg = new StringContent(jsonRemImgOrg);
                    stringContentRemImgOrg.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientRemImgOrg.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultDelImgOrg = await clientRemImgOrg.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteImage, stringContentRemImgOrg);
                    if (resultDelImgOrg.IsSuccessStatusCode)
                    {
                        var response = await resultDelImgOrg.Content.ReadAsAsync<ApiResponse>();
                        resultIdRemoveImgOrg = Convert.ToInt32(response.Result);
                    }
                }
                return Json(new { data = resultIdRemoveImgOrg, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (RemoveImage - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdRemoveImgOrg, success = false });
            }
        }
        /// <summary>
        /// This method is used to get cryptographic data for the ids
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCryptographicData(string value)
        {
            await Task.FromResult(1);
            return Json(new { data = Cryptography.EncryptPlainToCipher(value) });
        }
        /// <summary>
        /// This method is used to get unencrypted data for the ids
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDecryptCryptographicData(string value)
        {
            await Task.FromResult(1);
            return Json(new { data = Cryptography.DecryptCipherToPlain(value) });
        }
        [HttpPost]
        public async Task<IActionResult> PostOrgLoyalityGlobalSettings(LoyalityGlobalSettingDetailModel model)
        {
            string resultId = "0";
            string message = string.Empty;

            try
            {
                var data = new LoyalityGlobalSettingDetailModel()
                {

                    id = model.id,
                    organisationId = model.organisationId,
                    loyalityThreshhold = model.loyalityThreshhold,
                    globalReward = model.globalReward,
                    globalRatePoints = model.globalRatePoints,
                    userStatusRegularRatio = model.userStatusRegularRatio,
                    userStatusVipRatio = model.userStatusVipRatio,
                    createdDate = DateTime.Now,
                    modifiedDate = DateTime.Now,
                    bitePayRatio=model.bitePayRatio,
                    dcbFlexRatio=model.dcbFlexRatio,
                    FirstTransactionBonus=model.FirstTransactionBonus

                  
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateOrgLoyalityGlobalSetting, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToString(response.Result);
                            message = response.Message;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostPlan - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultId, message = message });
        }
        #region  Export Organisation Program
        /// <summary>
        /// This method is used to export the organisation program in excel.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrganisationProgramExportExcel(string searchValue, string id)
        {
            var fileName = "Program List.xlsx";
            List<OrganisationProgramDto> organisationPrograms = new List<OrganisationProgramDto>();
            var organisationId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
            if (organisationId > 0)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationPrograms + "?organisationId=" + organisationId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            organisationPrograms = response1.ToObject<List<OrganisationProgramDto>>();
                            organisationPrograms.ForEach(x => { x.strProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()); x.EncProgName = Cryptography.EncryptPlainToCipher(x.ProgramName); });
                        }

                    }
                }
            }
            // Getting all Organisation Programs data
            var orgProgramData = (from tempOrgPrg in organisationPrograms select tempOrgPrg).AsEnumerable();

            //Sorting
            var sortColumn = "DateAdded";
            var sortColumnDirection = "desc";
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                orgProgramData = orgProgramData.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                orgProgramData = orgProgramData.Where(o => o.ProgramName.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()) || o.ProgramCodeId.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()) || o.ProgramType.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()) || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), DateSuffixModel.GetSuffix(o.DateAdded.Day.ToString())).ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()));
            }
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "PROGRAM NAME";
            workSheet.Cells[1, 2].Value = "PROGRAM ID";
            workSheet.Cells[1, 3].Value = "DATE ADDED";
            workSheet.Cells[1, 4].Value = "PROGRAM TYPES";
            //Body of table  

            int recordIndex = 2;
            foreach (var prog in orgProgramData)
            {
                workSheet.Cells[recordIndex, 1].Value = prog.ProgramName;
                workSheet.Cells[recordIndex, 2].Value = prog.ProgramCodeId;
                workSheet.Cells[recordIndex, 3].Value = prog.DateAdded.ToShortDateString();
                workSheet.Cells[recordIndex, 4].Value = prog.ProgramType;
                recordIndex++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excel.GetAsByteArray());

            //Return the Excel file name
            return Json(new { fileName = fileName });
        }
        /// <summary>
        /// This method is used to download the excel file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Download(string fileName)
        {
            byte[] data;
            if (HttpContext.Session.TryGetValue("DownloadExcel_OrgProg", out data))
            {
                return File(data, "application/octet-stream", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }
        #endregion
    }
}