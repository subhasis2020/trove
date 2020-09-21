using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Foundry.Domain;
using apimodel = Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq.Dynamic;
using ElmahCore;
using Foundry.Web.Models;
using AutoMapper;
using Foundry.Domain.ApiModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using static Foundry.Domain.Constants;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class is used to contain the methods for merchant.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(CustomActionAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class MerchantController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        /// <summary>
        /// This constructor is used to get the interface injection for the services.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="environment"></param>
        public MerchantController(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        /// <summary>
        /// This will show the merchant detail form.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full, Merchant Full")]
        public async Task<IActionResult> Merchants()//Need to add primaryorganisationid and primaryprogramid that will be sent from program listing.
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
            PrimaryMerchantDetail merDetail = new PrimaryMerchantDetail();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetPrimaryOrgNPrgDetailOfMerchantAdmin + "?userId=" + userId).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    merDetail = response1.ToObject<PrimaryMerchantDetail>();
                }
                if (merDetail != null)
                {
                    ViewBag.PrimaryOrgId = Cryptography.EncryptPlainToCipher(merDetail.PrimaryOrganisationId.ToString());
                    ViewBag.ProgramId = Cryptography.EncryptPlainToCipher(merDetail.PrimaryProgramId.ToString());
                    ViewBag.ProgramName = Cryptography.EncryptPlainToCipher(merDetail.PrimaryProgramName.ToString());
                    ViewBag.PrimaryOrgName = Cryptography.EncryptPlainToCipher(merDetail.PrimaryOrgName.ToString());
                    if (merDetail.TotalMerchantInAdmin <= 1)
                    {
                        return RedirectToAction("Create", "Merchant", new { id = Cryptography.EncryptPlainToCipher(merDetail.MerchantId.ToString()), poId = Cryptography.EncryptPlainToCipher(merDetail.PrimaryOrganisationId.ToString()), ppId = Cryptography.EncryptPlainToCipher(merDetail.PrimaryProgramId.ToString()), ppN = Cryptography.EncryptPlainToCipher(merDetail.PrimaryProgramName.ToString()) });
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// This method is used to load admin merchants in the page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoadAdminsMerchantData()
        {
            var drawAdminMerchant = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                var userIdAdminMerchant = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                // Skip number of Rows count  
                var startAdminMerchant = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var lengthAdminMerchant = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumnAdminMerchant = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirectionAdminMerchant = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValueAdminMerchant = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSizeAdminMerchant = lengthAdminMerchant != null ? Convert.ToInt32(lengthAdminMerchant) : 0;

                int skipAdminMerchant = startAdminMerchant != null ? Convert.ToInt32(startAdminMerchant) : 0;

                int recordsTotalAdminMerchant = 0;
                List<MerchantDto> organisations = new List<MerchantDto>();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllMerchantsByAdmin + "?userId=" + userIdAdminMerchant).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        organisations = response1.ToObject<List<MerchantDto>>();
                        organisations.ForEach(x => x.Id = Cryptography.EncryptPlainToCipher(x.Id.ToString()));
                    }
                }
                // getting all Customer data  
                var customerDataAdminMerchant = (from tempcustomer in organisations
                                                 select tempcustomer);
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumnAdminMerchant) && string.IsNullOrEmpty(sortColumnDirectionAdminMerchant)))
                {
                    customerDataAdminMerchant = customerDataAdminMerchant.OrderBy(sortColumnAdminMerchant + " " + sortColumnDirectionAdminMerchant);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValueAdminMerchant))
                {
                    customerDataAdminMerchant = customerDataAdminMerchant.Where(m => m.MerchantName.ToLower().Trim().Contains(searchValueAdminMerchant.ToLower().Trim())
                    || (m.Id != null && m.Id.ToLower().Trim().Contains(searchValueAdminMerchant.ToLower().Trim()))
                    || (m.Location != null && m.Location.ToLower().Trim().Contains(searchValueAdminMerchant.ToLower().Trim()))
                    || (m.AccountType != null && m.AccountType.ToLower().Trim().Contains(searchValueAdminMerchant.ToLower().Trim()))
                    || (m.LastTransaction != null && m.LastTransaction.ToLower().Trim().Contains(searchValueAdminMerchant.ToLower().Trim()))
                    || (m.DateAdded.ToString("dd MM yyyy").ToLower().Trim().Contains(searchValueAdminMerchant.ToLower().Trim())));
                }
                //total number of rows counts   
                recordsTotalAdminMerchant = customerDataAdminMerchant.Count();
                //Paging   
                var dataAdminMerchant = customerDataAdminMerchant.Skip(skipAdminMerchant).Take(pageSizeAdminMerchant).ToList();
                //Returning Json Data  
                return Json(new { draw = drawAdminMerchant, recordsFiltered = recordsTotalAdminMerchant, recordsTotal = recordsTotalAdminMerchant, data = dataAdminMerchant });

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (Create - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawAdminMerchant, recordsFiltered = 0, recordsTotal = 0, data = new List<MerchantDto>() });
            }
        }
        /// <summary>
        /// This method is used to create/edit the merchant showing merchant detail form page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full, Merchant Full")]
        public async Task<IActionResult> Create(string id, string poId, string ppId, string ppN)
        {
            try
            {

                ViewBag.MerchantId = id;
                ViewBag.PrimaryOrgId = poId;
                ViewBag.PrimaryProgId = ppId;
                ViewBag.PrimaryProgName = ppN;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var primaryOrgId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(poId));
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.OrganisationInfoNProgramTypes + "?organisationId=" + primaryOrgId).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apimodel.ApiResponse>();
                        dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                        var organisations = response1.ToObject<OrganisationDetailModel>();
                        var primaryOrgName = organisations.OrganisationSubTitle;
                        ViewBag.PrimaryOrgName = primaryOrgName;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (Create - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }
        /// <summary>
        /// This method is used to post merchant detail form page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostMerchnatDetailInformation(MerchantDetailModel model)
        {
            string resultId = "0";
            if (ModelState.IsValid)
            {
                try
                {
                    OrganisationViewModel dataOrganisationDetail = PostDataInModalForMerchant(model);
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage result = await PostMerchantDetainInformationRefactor(dataOrganisationDetail, client);

                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<apimodel.ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                resultId = Convert.ToString(response.Result);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (Organisations - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new
            {
                data = resultId,
                dataMerchantName = Cryptography.EncryptPlainToCipher(model.OrganisationName)
            });
        }

        private async Task<HttpResponseMessage> PostMerchantDetainInformationRefactor(OrganisationViewModel dataOrganisationDetail, HttpClient client)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataOrganisationDetail);
            HttpContent stringContent = new StringContent(json.ToString());
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

            var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateMerchantDetailInfo, stringContent);
            return result;
        }

        private OrganisationViewModel PostDataInModalForMerchant(MerchantDetailModel model)
        {
            ModalOrganizationProgramType(model);
            ModalOrganizationAccountType(model);
            var dataOrganisationDetail = new apimodel.OrganisationViewModel
            {
                addressLine1 = model.Address,
                businessTypeId = model.BusinessTypeId,
                city = model.City,
                contactNumber = model.ContactNumber,
                country = model.Country,
                description = !string.IsNullOrEmpty(model.Description) ? model.Description.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>") : null,
                facebookURL = model.FacebookURL,
                OrganisationAccountType = _mapper.Map<List<apimodel.OrganisationAccTypeModel>>(model.OrgAccType),
                id = !string.IsNullOrEmpty(model.OrganisationId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.OrganisationId)) : 0,
                name = model.OrganisationName,
                OrganisationProgram = _mapper.Map<List<apimodel.OrganisationProgramModel>>(model.OrgProgram),
                isMapVisible = model.ShowMap,
                InstagramHandle = model.InstagramHandle,
                state = model.State,
                twitterURL = model.TwitterURL,
                websiteURL = model.Website,
                zip = model.Zip,
                emailAddress = string.Empty,
                organisationType = Convert.ToInt32(Constants.OrganasationType.Merchant),
                programId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.PrimaryProgramIdEnc)),
                ImagePath = model.ImagePath,
                location = model.Location,
                JPOS_OrgId = !string.IsNullOrEmpty(model.Jpos_MerchantEncId) ? Cryptography.DecryptCipherToPlain(model.Jpos_MerchantEncId) : ""
            };
            return dataOrganisationDetail;
        }

        private static void ModalOrganizationAccountType(MerchantDetailModel model)
        {
            model.OrgAccType = new List<AccountTypeDto>();
            foreach (var item in model.SelectedOrgAccType)
            {
                AccountTypeDto obj = new AccountTypeDto();
                obj.Id = item;
                model.OrgAccType.Add(obj);
            }
        }

        private static void ModalOrganizationProgramType(MerchantDetailModel model)
        {
            model.OrgProgram = new List<OrganisationProgramDto>();
            foreach (var item in model.OrganisationProgramSelect)
            {
                OrganisationProgramDto objprogramId = new OrganisationProgramDto();
                objprogramId.ProgramId = item.ProgramId;
                objprogramId.IsPrimaryAssociation = item.IsPrimaryAssociation;
                model.OrgProgram.Add(objprogramId);
            }
        }

        /// <summary>
        /// This method is used to post merchant reward information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostMerchnatRewardInformation(MerchantRewardModel model)
        {
            string resultIdReward = "0";
            try
            {
                var dataMerchantReward = new MerchantRewardViewModel
                {
                    amounts = model.Amount,
                    backgroundColor = model.BackGroundColor,
                    businessTypeId = model.BusinessTypeId,
                    description = !string.IsNullOrEmpty(model.Description) ? model.Description.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>") : null,
                    endDate = model.EndDate,
                    endTime = model.EndTime,
                    MerchantId = !string.IsNullOrEmpty(model.MerchantId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.MerchantId)) : 0,
                    offerSubTypeId = model.OfferSubTypeId,
                    offerTypeId = model.OfferTypeId,
                    bannerDescription = model.RewardSubTitle,
                    name = model.RewardTitle,
                    startDate = model.StartDate,
                    startTime = model.StartTime,
                    noOfVisits = model.Visits,
                    id = !string.IsNullOrEmpty(model.Id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.Id)) : 0,
                    isActive = true,
                    IsPublished = model.IsPublished
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataMerchantReward);
                    HttpContent stringContentReward = new StringContent(json);
                    stringContentReward.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var resultReward = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateMerchantRewardInfo, stringContentReward);

                    if (resultReward.IsSuccessStatusCode)
                    {
                        var responseReward = await resultReward.Content.ReadAsAsync<apimodel.ApiResponse>();
                        if (responseReward.StatusFlagNum == 1)
                        {
                            resultIdReward = Convert.ToString(responseReward.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (PostMerchnatRewardInformation - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultIdReward });
        }
        /// <summary>
        /// This method is used to post merchant business information page to the API.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostMerchnatBusinessInformation(MerchantBusinessInfoModel model)
        {
            string resultId = "0";
            if (ModelState.IsValid)
            {
                try
                {
                    var dataMerchantReward = new OrganisationBusinessInformationModel()
                    {
                        Id = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.Id)),
                        HolidayHours = _mapper.Map<List<apimodel.OrganisationScheduleModel>>(model.HolidayHours),
                        HoursOfOperation = _mapper.Map<List<apimodel.OrganisationScheduleModel>>(model.HoursOfOperation),
                        MealPeriod = _mapper.Map<List<apimodel.OrganisationMealPeriodModel>>(model.MealPeriod),
                        Merchant = _mapper.Map<apimodel.OrganisationViewModel>(model.Merchant),
                        MerchantTerminal = _mapper.Map<List<apimodel.OrganisationMerchantTerminalModel>>(model.MerchantTerminal)
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataMerchantReward);
                        HttpContent stringContent = new StringContent(json.ToString());
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                        var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateMerchantBusinessInfo, stringContent);

                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<apimodel.ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                resultId = Convert.ToString(response.Result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (Organisations - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId });
        }
        /// <summary>
        /// This method is used to edit or switch the reward active/inactive status to the page.
        /// </summary>
        /// <param name="oReward"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditPromotionRewardActiveStatus(RewardsSatatusDto oReward)// string id, bool rewardStatus)
        {
            int resultRewardStatusId = 0;
            using (var client = new HttpClient())
            {

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(oReward);
                HttpContent stringContent = new StringContent(json.ToString());
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var result = client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.EditPromotionRewardStatus, stringContent).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    resultRewardStatusId = Convert.ToInt32(response1);
                }
            }

            var resultStatus = false;
            if (resultRewardStatusId > 0)
                resultStatus = true;
            var resultStatusMessage = resultRewardStatusId > 0 ? "Updated Successfully" : MessagesConstants.SomeIssueInProcessing;
            return Json(new { Status = resultStatus, Message = resultStatusMessage });
        }
        /// <summary>
        /// This method is used to delete the merchant and setting the active status to false for the merchant.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="JposMerId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(string ID, string JposMerId)
        {
            int resultIdDeleteMerchant = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    var orgDtoMerchant = new OrganisationDto()
                    {
                        Id = !string.IsNullOrEmpty(ID) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(ID)) : 0,
                        JPOS_MerchantId = !string.IsNullOrEmpty(JposMerId) ? Convert.ToString(Cryptography.DecryptCipherToPlain(JposMerId)) : "",
                        OrganisationType = OrganasationType.Merchant
                    };
                    string jsonMerchantDel = Newtonsoft.Json.JsonConvert.SerializeObject(orgDtoMerchant);
                    HttpContent stringContentMerchantDel = new StringContent(jsonMerchantDel.ToString());
                    stringContentMerchantDel.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultDelMerchant = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteOrganisation, stringContentMerchantDel);
                    if (resultDelMerchant.IsSuccessStatusCode)
                    {
                        var responseDelMerchant = await resultDelMerchant.Content.ReadAsAsync<apimodel.ApiResponse>();
                        if (responseDelMerchant.StatusFlagNum == 1)
                        {
                            resultIdDeleteMerchant = Convert.ToInt32(responseDelMerchant.Result);
                        }
                    }
                }
                return Json(new { draw = resultIdDeleteMerchant, success = true, message = "" });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (Delete - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = resultIdDeleteMerchant, success = false, message = "" });
            }
        }
        /// <summary>
        /// This method is used to get the merchant detail view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult GetMerchantDetailViewComponent(string id, string poId, string ppId, string poN)
        {
            return ViewComponent("MerchantDetail", new { id, ppId, poId, poN });
        }
        /// <summary>
        /// This method is used to get the merchant admin level view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        public IActionResult GetMerchantAdminLevelViewComponent(string id, string poId, string ppId, string poN, string ppN)
        {
            return ViewComponent("MerchantAdminLevel", new { id, poId, ppId, poN, ppN });
        }
        /// <summary>
        /// This method is used to get the merchant transaction view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        public IActionResult GetMerchantTransactionViewComponent(string id, string poId, string ppId, string poN, string ppN)
        {
            return ViewComponent("MerchantTransaction", new { id, poId, ppId, poN, ppN });
        }
        /// <summary>
        /// This method is used to create merchant reward view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pId"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        public IActionResult CreateMerchantRewardViewComponent(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            return ViewComponent("CreateMerchantReward", new { id, pId, poId, ppId, poN, ppN });
        }
        /// <summary>
        /// This method is used to get the merchant business info view component
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pId"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        public IActionResult CreateMerchantBusinessInfoViewComponent(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            return ViewComponent("CreateMerchantBusinessInfo", new { id, pId, poId, ppId, poN, ppN });
        }
        /// <summary>
        /// This method is used to get the merchant reward list view component
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        public IActionResult GetMerchantRewardListViewComponent(string id, string poId, string ppId, string poN, string ppN)
        {
            return ViewComponent("MerchantRewardList", new { id, ppId, poId, poN, ppN });
        }
        /// <summary>
        /// This method is used to load all promotions for the merchant based on its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pId"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="poN"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        public IActionResult LoadAllPromotions(string id, string pId, string poId, string ppId, string poN, string ppN)
        {
            return ViewComponent("MerchantPromotions", new { id, pId, poId, ppId, poN, ppN });
        }
        /// <summary>
        /// This method is used to load all merchant admins
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllMerchantAdmins(string id)
        {
            var drawAdminMer = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startAdminMer = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthAdminMer = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnAdminMer = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionAdminMer = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValueAdminMer = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizeAdminMer = lengthAdminMer != null ? Convert.ToInt32(lengthAdminMer) : 0;

                int skipAdminMer = startAdminMer != null ? Convert.ToInt32(startAdminMer) : 0;

                int recordsTotalAdminMer = 0;
                List<OrganisationsAdminsDto> adminMerchants = new List<OrganisationsAdminsDto>();
                using (var clientAdminMer = new HttpClient())
                {
                    clientAdminMer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientAdminMer.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var organisationIdAdminMer = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    if (organisationIdAdminMer > 0)
                    {
                        var resultAdminMer = clientAdminMer.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MerchantAdmins + "?organisationId=" + organisationIdAdminMer).Result;
                        if (resultAdminMer.IsSuccessStatusCode)
                        {
                            var responseAdminMer = await resultAdminMer.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDesAdminMer = JsonConvert.DeserializeObject(responseAdminMer.Result.ToString());
                            adminMerchants = responseDesAdminMer.ToObject<List<OrganisationsAdminsDto>>();
                            adminMerchants.Where(x => x.RoleName != null).ToList().ForEach(x => x.RoleName = x.RoleName.ToLower(CultureInfo.InvariantCulture).Trim().Replace("organisation ", "").Replace("organization ", "").ToUpper(CultureInfo.InvariantCulture));
                            adminMerchants.Where(x => x.ProgramsAccessibility != null).ToList().ForEach(x => x.ProgramsAccessibility = x.ProgramsAccessibility.Replace(",", "<br/>"));
                        }
                    }
                }
                // Getting all Organisation Programs data
                var merAdminData = (from tempAdminMer in adminMerchants select tempAdminMer);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnAdminMer) && string.IsNullOrEmpty(sortColumnDirectionAdminMer)))
                {
                    merAdminData = merAdminData.OrderBy(sortColumnAdminMer + " " + sortColumnDirectionAdminMer);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValueAdminMer))
                {
                    merAdminData = merAdminData.Where(o => o.Name.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueAdminMer.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.EmailAddress.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueAdminMer.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.Title.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueAdminMer.ToLower(CultureInfo.InvariantCulture).Trim())
                    || o.PhoneNumber.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueAdminMer.ToLower(CultureInfo.InvariantCulture).Trim())
                    || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), DateSuffixModel.GetSuffix(o.DateAdded.Day.ToString())).ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueAdminMer.ToLower(CultureInfo.InvariantCulture).Trim()));
                }

                //total number of rows counts
                recordsTotalAdminMer = merAdminData.Count();
                //Paging
                var dataAdminMer = merAdminData.Skip(skipAdminMer).Take(pageSizeAdminMer).ToList();
                return Json(new { draw = drawAdminMer, recordsFiltered = recordsTotalAdminMer, recordsTotal = recordsTotalAdminMer, data = dataAdminMer });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (LoadAllMerchantAdmins - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawAdminMer, recordsFiltered = 0, recordsTotal = 0, data = new List<OrganisationsAdminsDto>() });
            }
        }
        /// <summary>
        /// This method is used to load all merchant transactions.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllMerchantTransaction(string id, string dateMonth = "")
        {
            var drawMerTran = HttpContext.Request.Form["draw"].FirstOrDefault();

            try
            {

                // Skip number of Rows count  
                var startMerTran = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var lengthMerTran = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumnMerTran = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirectionMerTran = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValueMerTran = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSizeMerTran = lengthMerTran != null ? Convert.ToInt32(lengthMerTran) : 0;

                int skipMerTran = startMerTran != null ? Convert.ToInt32(startMerTran) : 0;

                int recordsTotalMerTran = 0;
                List<MerchantTransactionDto> merchantTransactions = new List<MerchantTransactionDto>();
                using (var clientMerTran = new HttpClient())
                {
                    clientMerTran.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientMerTran.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var organisationIdMerTran = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    if (organisationIdMerTran > 0)
                    {
                        var resultTranMer = clientMerTran.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MerchantTransaction + "?organisationId=" + organisationIdMerTran + "&dateMonth=" + dateMonth).Result;
                        if (resultTranMer.IsSuccessStatusCode)
                        {
                            var responseTranMer = await resultTranMer.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDesc = JsonConvert.DeserializeObject(responseTranMer.Result.ToString());
                            merchantTransactions = responseDesc.ToObject<List<MerchantTransactionDto>>();
                        }
                    }
                }
                // getting all Customer data  
                var merchantdataTran = (from tempmerchant in merchantTransactions
                                    select tempmerchant);
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumnMerTran) && string.IsNullOrEmpty(sortColumnDirectionMerTran)))
                {
                    merchantdataTran = merchantdataTran.OrderBy(sortColumnMerTran + " " + sortColumnDirectionMerTran);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValueMerTran))
                {
                    merchantdataTran = merchantdataTran.Where(m => m.Name.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueMerTran.ToLower(CultureInfo.InvariantCulture).Trim()) 
                    || m.Account.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueMerTran.ToLower(CultureInfo.InvariantCulture).Trim())
                    || m.Amount.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueMerTran.ToLower(CultureInfo.InvariantCulture).Trim()) 
                    || m.MerchantId.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueMerTran.ToLower(CultureInfo.InvariantCulture).Trim())
                    || m.TransactionDate.ToString().ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValueMerTran.ToLower(CultureInfo.InvariantCulture).Trim()));
                }
                //total number of rows counts   
                recordsTotalMerTran = merchantdataTran.Count();
                //Paging   
                var dataMerTran = merchantdataTran.Skip(skipMerTran).Take(pageSizeMerTran).ToList();
                //Returning Json Data  
                return Json(new { draw= drawMerTran, recordsFiltered = recordsTotalMerTran, recordsTotal= recordsTotalMerTran, data= dataMerTran });

            }
            catch (Exception)
            {
                return Json(new { draw= drawMerTran, recordsFiltered = 0, recordsTotal = 0, data = new List<MerchantTransactionDto>() });
            }
        }
        /// <summary>
        /// This method is used to post merchant promotion information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostMerchantPromotionInformation(PromotionDetailModel model)
        {
            string resultId = "0";

            try
            {
                var dataMerchantReward = new MerchantRewardViewModel()
                {
                    id = model.PromotionId,
                    bannerTypeId = Convert.ToInt32(model.BannerTypeId),
                    createdBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    modifiedBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value),
                    promotionDay = model.RepeatDay,
                    IsPromotion = true,
                    name = model.PromotionDescription,
                    description = !string.IsNullOrEmpty(model.PromoDetail) ? model.PromoDetail.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>") : null,
                    endDate = model.EndDate,
                    endTime = model.EndTime,
                    MerchantId = !string.IsNullOrEmpty(model.MerchantId.ToString()) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.MerchantId.ToString())) : 0,
                    bannerDescription = model.BannerDescription,
                    startDate = model.StartDate,
                    startTime = model.StartTime,
                    IsDaily = model.IsDaily,
                    isActive = model.IsActive,
                    ImagePath = model.PromotionImagePath
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataMerchantReward);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateMerchantRewardInfo, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apimodel.ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToString(response.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (PostMerchantPromotionInformation - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = resultId });
        }

        /// <summary>
        /// This method is used to post the organization admin detail information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostMerchantAdminDetailInformation(MerchantAdminDetailModel model)
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
                        ProgramsAccessibility =  new List<OrganisationProgramIdModel>(),
                        OrganisationId = !string.IsNullOrEmpty(model.OrganisationId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.OrganisationId)) : 0,
                        RoleId = model.adminLevelModel.RoleId,
                        UserId = model.adminLevelModel.UserId != null ? model.adminLevelModel.UserId.Value : 0,
                        UserImagePath = model.adminLevelModel.UserImagePath,
                        Custom1 = !string.IsNullOrEmpty(model.Custom1) ? model.Custom1 : string.Empty,
                        MerchantAccessibility = model.MerchantAccessibility.Count == 1 && model.MerchantAccessibility.FirstOrDefault().merchantId == 0 ? new List<MerchantIdModel>() : _mapper.Map<List<MerchantIdModel>>(model.MerchantAccessibility),
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
        /// This method is used to invite merchant admin.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public async Task<IActionResult> InviteMerchantAdmin(string userEmail, string merchantId)
        {
            try
            {
                var resultIdMerAdminInv = "";
                using (var clientAdminMerInv = new HttpClient())
                {
                    clientAdminMerInv.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientAdminMerInv.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var mid = !string.IsNullOrEmpty(merchantId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(merchantId)) : 0;
                    var resultAdminMerInv = clientAdminMerInv.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.InviteAdmin + "?email=" + userEmail + "&id=" + mid).Result;
                    if (resultAdminMerInv.IsSuccessStatusCode)
                    {
                        var responseAdminMerInv = await resultAdminMerInv.Content.ReadAsAsync<ApiResponse>();
                        resultIdMerAdminInv = Convert.ToString(responseAdminMerInv.Result);
                    }

                }
                //Returning Json Data  
                return Json(new { data = resultIdMerAdminInv });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (InviteMerchantAdmin - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = "" });
            }
        }
        /// <summary>
        /// This method is sued to delete reward of the user.
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteReward(string promotionId)
        {
            int resultIdDelReward = 0;
            try
            {
                using (var clientDelReward = new HttpClient())
                {
                    var proDtoDelReward = new MerchantRewardDto()
                    {
                        Id = !string.IsNullOrEmpty(promotionId) ? Cryptography.DecryptCipherToPlain(promotionId) : "0",
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(proDtoDelReward);

                    HttpContent stringContentDelReward = new StringContent(json.ToString());
                    stringContentDelReward.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelReward.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var resultDelReward = await clientDelReward.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeletePromotion, stringContentDelReward);
                    if (resultDelReward.IsSuccessStatusCode)
                    {
                        var responseDelReward = await resultDelReward.Content.ReadAsAsync<ApiResponse>();
                        resultIdDelReward = Convert.ToInt32(responseDelReward.Result);
                    }
                }
                return Json(new { data = resultIdDelReward, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (DeleteOrganisationProgram - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdDelReward, success = false });
            }
        }
        /// <summary>
        /// This method is used to get merchant promotions by merchantId
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantPromotions(string merchantId)
        {
            List<PromotionsDto> promotions = new List<PromotionsDto>();
            try
            {
                var MerchantId = !string.IsNullOrEmpty(merchantId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(merchantId)) : 0;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetMerchantPromotions + "?merchantId=" + MerchantId).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apimodel.ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            promotions = response1.ToObject<List<PromotionsDto>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (GetMerchantPromotions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = promotions });
        }
        /// <summary>
        /// This method is used to get merchant promotions by promotion id.
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantPromotionsById(string promotionId)
        {
            PromotionsDto promotions = new PromotionsDto();
            try
            {
                var Id = !string.IsNullOrEmpty(promotionId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(promotionId)) : 0;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetPromotionsByID + "?promotionId=" + Id).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apimodel.ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                            promotions = response1.ToObject<PromotionsDto>();
                            if (promotions != null && !string.IsNullOrEmpty(promotions.Description))
                                promotions.Description = promotions.Description.Replace("<br/>", "\n").Replace("<br/>", Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (GetMerchantPromotionsById - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = promotions });
        }
        /// <summary>
        /// This method is used to clone merchant in the datatable.
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CloneMerchant(string merchantId, string poId, string ppId, string ppN)
        {
            int resultId = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    var proDto = new MerchantDto()
                    {
                        Id = !string.IsNullOrEmpty(merchantId) ? Cryptography.DecryptCipherToPlain(merchantId) : "0",
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(proDto);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.CloneMerchant, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        resultId = Convert.ToInt32(response.Result);
                    }
                }
                return RedirectToAction("Create", new { id = Cryptography.EncryptPlainToCipher(resultId.ToString()), poId, ppId, ppN });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Merchant (CloneMerchant - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultId, success = false, });
            }
        }

        #region Export Merchant Transaction Listing
        /// <summary>
        /// This method is used to export merchant transaction in excel
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="id"></param>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MerchantTransactionExportExcel(string searchValue, string id, string dateMonth = "")
        {
            var fileNameMerTranExp = "Transaction List.xlsx";
            List<MerchantTransactionDto> merchantTransactionsExp = new List<MerchantTransactionDto>();
            using (var clientMerTranExp = new HttpClient())
            {
                clientMerTranExp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientMerTranExp.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                var organisationIdMerTranExp = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                if (organisationIdMerTranExp > 0)
                {
                    var resultMerTranExp = clientMerTranExp.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MerchantTransaction + "?organisationId=" + organisationIdMerTranExp + "&dateMonth=" + dateMonth).Result;
                    if (resultMerTranExp.IsSuccessStatusCode)
                    {
                        var responseMerTranExp = await resultMerTranExp.Content.ReadAsAsync<ApiResponse>();
                        dynamic responseDesMerTranExp = JsonConvert.DeserializeObject(responseMerTranExp.Result.ToString());
                        merchantTransactionsExp = responseDesMerTranExp.ToObject<List<MerchantTransactionDto>>();
                    }
                }
            }
            // Getting all Organisation Programs data
            var dataMerTranExp = (from temp in merchantTransactionsExp select temp).AsEnumerable();

            //Sorting  
            var sortColumnMerTranExp = "MerchantId";
            var sortColumnDirectionMerTranExp = "desc";
            if (!(string.IsNullOrEmpty(sortColumnMerTranExp) && string.IsNullOrEmpty(sortColumnDirectionMerTranExp)))
            {
                dataMerTranExp = dataMerTranExp.OrderBy(sortColumnMerTranExp + " " + sortColumnDirectionMerTranExp);
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataMerTranExp = dataMerTranExp.Where(m => m.Name.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()) || m.Account.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim())
                || m.Amount.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()) || m.MerchantId.ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()) || m.TransactionDate.ToString().ToLower(CultureInfo.InvariantCulture).Trim().Contains(searchValue.ToLower(CultureInfo.InvariantCulture).Trim()));
            }
            ExcelPackage excelMerTranExp = new ExcelPackage();
            var workSheetMerTranExp = excelMerTranExp.Workbook.Worksheets.Add("Sheet1");
            workSheetMerTranExp.TabColor = System.Drawing.Color.Black;
            workSheetMerTranExp.DefaultRowHeight = 12;
            //Header of table  
            workSheetMerTranExp.Row(1).Height = 20;
            workSheetMerTranExp.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetMerTranExp.Row(1).Style.Font.Bold = true;
            workSheetMerTranExp.Cells[1, 1].Value = "MERCHANT ID";
            workSheetMerTranExp.Cells[1, 2].Value = "DATE";
            workSheetMerTranExp.Cells[1, 3].Value = "TIME";
            workSheetMerTranExp.Cells[1, 4].Value = "ACCOUNT";
            workSheetMerTranExp.Cells[1, 5].Value = "MERCHANT NAME";
            workSheetMerTranExp.Cells[1, 6].Value = "AMOUNT";
            //Body of table  
            //  
            int recordIndex = 2;
            foreach (var d in dataMerTranExp)
            {
                workSheetMerTranExp.Cells[recordIndex, 1].Value = d.MerchantId;
                workSheetMerTranExp.Cells[recordIndex, 2].Value = d.TransactionDate.ToShortDateString();
                workSheetMerTranExp.Cells[recordIndex, 3].Value = d.TransactionDate.ToShortTimeString();
                workSheetMerTranExp.Cells[recordIndex, 4].Value = d.Account;
                workSheetMerTranExp.Cells[recordIndex, 5].Value = d.Name;
                workSheetMerTranExp.Cells[recordIndex, 6].Value = d.Amount;
                recordIndex++;
            }
            workSheetMerTranExp.Column(1).AutoFit();
            workSheetMerTranExp.Column(2).AutoFit();
            workSheetMerTranExp.Column(3).AutoFit();
            workSheetMerTranExp.Column(4).AutoFit();
            workSheetMerTranExp.Column(5).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelMerTranExp.GetAsByteArray());

            //Return the Excel file name
            return Json(new { fileName = fileNameMerTranExp });
        }
        /// <summary>
        /// This method is used to download merchant transaction in excel.
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