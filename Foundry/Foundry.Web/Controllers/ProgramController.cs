using AutoMapper;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using apiModel = Foundry.Domain.ApiModel;
using OfficeOpenXml.Style;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class contains the methods related to programs and its entities.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(CustomActionAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ProgramController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _env;
        /// <summary>
        /// This method is used to inject the services in the class to be used.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="environment"></param>
        public ProgramController(IConfiguration configuration, IMapper mapper, IHostingEnvironment environment
            )
        {
            _configuration = configuration;
            _mapper = mapper;
            _env = environment;
        }
        /// <summary>
        /// This method is used to show the program info page with form fields.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full")]
        public async Task<IActionResult> Index(string id, string poId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgId = poId;
            ViewBag.ProgramId = id;
            ViewBag.ProgramName = ppN;
            ViewBag.PrimaryOrgName = poN;
            if (_configuration["SodexhoOrgId"] == Cryptography.DecryptCipherToPlain(poId))
            {
                ViewBag.OrgId = _configuration["SodexhoOrgId"];
            }
            else
            {
                ViewBag.OrgId = "1111111";

            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CardHolderAgreementsExistence + "?programId=" + Cryptography.DecryptCipherToPlain(id)).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<ApiResponse>();

                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    var cardholderExistenceCheck = response1.ToObject<CardholderAgreementDto>();
                    if (cardholderExistenceCheck != null && cardholderExistenceCheck.AccountTypeId != null && cardholderExistenceCheck.AccountTypeId > 0)
                    {
                        ViewBag.CardHolderAgreementViewCheck = 1;
                    }

                }

            }
            ViewBag.ReportURL = _configuration["ReportURL"] + "PlanBalanceSummary/PlanBalance.aspx?orgId=" + poId + "&programId=" + id;
            return View();
        }
        /// <summary>
        /// This method is used to show the list of the programs irrespective of the organiszations.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full")]
        public IActionResult Programs()
        {
            return View();
        }
        /// <summary>
        /// This method is used to get the program in the organizations.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full")]
        public async Task<IActionResult> Program()//Need to add primaryorganisationid and primaryprogramid that will be sent from program listing.
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
            PrimaryMerchantDetail merDetailInPrg = new PrimaryMerchantDetail();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var resultPrg = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetPrimaryOrgNPrgDetailOfProgramAdmin + "?userId=" + userId).Result;
                if (resultPrg.IsSuccessStatusCode)
                {
                    var responsePrg = await resultPrg.Content.ReadAsAsync<ApiResponse>();
                    dynamic responseDesPrg = JsonConvert.DeserializeObject(responsePrg.Result.ToString());
                    merDetailInPrg = responseDesPrg.ToObject<PrimaryMerchantDetail>();
                }
                if (merDetailInPrg != null)
                {
                    ViewBag.PrimaryOrgId = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryOrganisationId.ToString());
                    ViewBag.ProgramId = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryProgramId.ToString());
                    ViewBag.ProgramName = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryProgramName.ToString());
                    ViewBag.PrimaryOrgName = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryOrgName.ToString());
                    if (merDetailInPrg.TotalProgramInAdmin <= 1)
                    {
                        return RedirectToAction("Index", "Program", new { id = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryProgramId.ToString()), poId = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryOrganisationId.ToString()), ppN = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryProgramName.ToString()), poN = Cryptography.EncryptPlainToCipher(merDetailInPrg.PrimaryOrgSubtitle.ToString()) });
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// This method is used to load admin programs.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoadPrgAdminProgram()
        {
            var drawPrgAdminPrg = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startPrgAdmPrg = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthPrgAdmPrg = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnPrgAdmPrg = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionPrgAdmPrg = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValuePrgAdmPrg = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizePrgAdmPrg = lengthPrgAdmPrg != null ? Convert.ToInt32(lengthPrgAdmPrg) : 0;

                int skipPrgAdmPrg = startPrgAdmPrg != null ? Convert.ToInt32(startPrgAdmPrg) : 0;

                int recordsTotalPrgAdmPrg = 0;
                List<ProgramListDto> organisationPrograms = new List<ProgramListDto>();
                using (var clientPrgAdmPrg = new HttpClient())
                {
                    clientPrgAdmPrg.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgAdmPrg.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultPrgAdmPrg = clientPrgAdmPrg.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllProgramsForPrgAdminList + "?isActive=" + true + "&isDeleted=" + false).Result;
                    if (resultPrgAdmPrg.IsSuccessStatusCode)
                    {
                        var responsePrgAdmPrg = await resultPrgAdmPrg.Content.ReadAsAsync<ApiResponse>();
                        if (responsePrgAdmPrg.StatusFlagNum == 1)
                        {
                            dynamic responseDesPrgAdmPrg = JsonConvert.DeserializeObject(responsePrgAdmPrg.Result.ToString());
                            organisationPrograms = responseDesPrgAdmPrg.ToObject<List<ProgramListDto>>();
                            organisationPrograms.ForEach(x => { x.strProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()); x.EncProgName = Cryptography.EncryptPlainToCipher(x.ProgramName); x.OrganisationEncId = Cryptography.EncryptPlainToCipher(x.OrganisationId.ToString()); x.EncOrganisationName = Cryptography.EncryptPlainToCipher(x.OrganisationName.ToString()); });
                        }

                    }
                }
                // Getting all Organisation Programs data
                var orgProgramData = (from tempOrgPrg in organisationPrograms select tempOrgPrg).AsEnumerable();

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnPrgAdmPrg) && string.IsNullOrEmpty(sortColumnDirectionPrgAdmPrg)))
                {
                    orgProgramData = orgProgramData.OrderBy(sortColumnPrgAdmPrg + " " + sortColumnDirectionPrgAdmPrg);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValuePrgAdmPrg))
                {
                    orgProgramData = orgProgramData.Where(o => o.ProgramName.ToLower().Trim().Contains(searchValuePrgAdmPrg.ToLower().Trim())
                    || o.ProgramCodeId.ToLower().Trim().Contains(searchValuePrgAdmPrg.ToLower().Trim())
                    || o.ProgramType.ToLower().Trim().Contains(searchValuePrgAdmPrg.ToLower().Trim())
                    || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), GetSuffix(o.DateAdded.Day.ToString())).ToLower().Trim().Contains(searchValuePrgAdmPrg.ToLower().Trim())
                    || (o.OrganisationSubTitle != null && o.OrganisationSubTitle.ToLower().Trim().Contains(searchValuePrgAdmPrg.ToLower().Trim()))).AsEnumerable();
                }
                //total number of rows counts
                recordsTotalPrgAdmPrg = orgProgramData.Count();
                //Paging
                var dataPrgAdmPrg = orgProgramData.Skip(skipPrgAdmPrg).Take(pageSizePrgAdmPrg).ToList();
                return Json(new { draw = drawPrgAdminPrg, recordsFiltered = recordsTotalPrgAdmPrg, recordsTotal = recordsTotalPrgAdmPrg, data = dataPrgAdmPrg });

            }
            catch (Exception)
            {
                return Json(new { draw = drawPrgAdminPrg, recordsFiltered = 0, recordsTotal = 0, data = new List<ProgramListDto>() });
            }

        }
        /// <summary>
        /// This method is used to get the program info.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult ProgramInfo(string id, string poId, string ppN, string poN)
        {
            return ViewComponent("ProgramInfo", new { id = id, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to add account in the program.
        /// </summary>
        /// <returns></returns>
        public IActionResult AddAccount()
        {
            return ViewComponent("AddAccount", new { });
        }
        /// <summary>
        /// This method is used to get the account holder detail page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prgId"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult ManageAccountHolder(string id, string prgId, string poId, string ppN, string poN)
        {
            return ViewComponent("ManageAccountHolder", new { id = id, prgId = prgId, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to get the program level admins.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult ProgramLevelAdmin(string id, string poId, string ppN, string poN)
        {
            return ViewComponent("ProgramLevelAdmin", new { id = id, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to get the card holder agreement page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult CardHolderAgreement(string id, string poId, string ppN, string poN)
        {
            return ViewComponent("CardHolderAgreement", new { id = id, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to get the card holder agreement history.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult CardHolderAgreementHistory(string id, string poId, string ppN, string poN)
        {
            return ViewComponent("CardHolderAgreementHistory", new { id = id, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to get the account list.
        /// </summary>
        /// <returns></returns>
        public IActionResult AccountList()
        {
            return ViewComponent("AccountList", new { });
        }
        /// <summary>
        /// This method is used to get the accountholder list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult AccountHolderList(string id, string poId, string ppN, string poN)
        {
            return ViewComponent("AccountHolderList", new { id = id, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to get the import account holder list.
        /// </summary>
        /// <returns></returns>
        public IActionResult UploadAccountHolderList()
        {
            ViewBag.Url = "/Import/Sample/demo.xlsx";
            return ViewComponent("UploadAccountHolderList", new { });
        }
        /// <summary>
        /// This method is used to show the transaction view for the program.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult TransactionView(string id, string poId, string ppN, string poN)
        {
            return ViewComponent("Transactions", new { id = id, poId = poId, ppN = ppN, poN = poN });
        }
        /// <summary>
        /// This method is used to load all program level admins.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllProgramLevelAdmins(string id)
        {
            var drawPrgAdm = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startPrgAdm = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthPrgAdm = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnPrgAdm = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionPrgAdm = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValuePrgAdm = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizePrgAdm = lengthPrgAdm != null ? Convert.ToInt32(lengthPrgAdm) : 0;

                int skipPrgAdm = startPrgAdm != null ? Convert.ToInt32(startPrgAdm) : 0;

                int recordsTotalPrgAdm = 0;
                List<ProgramLevelAdminDto> proLevelAdmins = new List<ProgramLevelAdminDto>();
                using (var clientPrgAdm = new HttpClient())
                {
                    clientPrgAdm.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgAdm.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    //make it dynamic.

                    var programIdPrgAdm = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    if (programIdPrgAdm > 0)
                    {
                        var resultPrgAdm = clientPrgAdm.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramLevelAdminList + "?programId=" + programIdPrgAdm).Result;
                        if (resultPrgAdm.IsSuccessStatusCode)
                        {
                            var responsePrgAdm = await resultPrgAdm.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDesPrgAdm = JsonConvert.DeserializeObject(responsePrgAdm.Result.ToString());
                            proLevelAdmins = responseDesPrgAdm.ToObject<List<ProgramLevelAdminDto>>();
                            proLevelAdmins.Where(x => x.RoleName != null).ToList().ForEach(x => x.RoleName = x.RoleName.ToLower().Trim().Replace("organisation ", "").Replace("organization ", "").ToUpper());
                            proLevelAdmins.Where(x => x.ProgramsAccessibility != null).ToList().ForEach(x => x.ProgramsAccessibility = x.ProgramsAccessibility.Replace(",", "<br/>"));
                        }
                    }
                }
                // Getting all Organisation Programs data
                var prgAdminData = (from tempOrgAdmin in proLevelAdmins select tempOrgAdmin);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnPrgAdm) && string.IsNullOrEmpty(sortColumnDirectionPrgAdm)))
                {
                    prgAdminData = prgAdminData.OrderBy(sortColumnPrgAdm + " " + sortColumnDirectionPrgAdm);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValuePrgAdm))
                {
                    prgAdminData = prgAdminData.Where(o => o.Name.ToLower().Trim().Contains(searchValuePrgAdm.ToLower().Trim())
                    || o.EmailAddress.ToLower().Trim().Contains(searchValuePrgAdm.ToLower().Trim())
                    || o.PhoneNumber.ToLower().Trim().Contains(searchValuePrgAdm.ToLower().Trim())
                    || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), GetSuffix(o.DateAdded.Day.ToString())).ToLower().Trim().Contains(searchValuePrgAdm.ToLower().Trim()));
                }

                //total number of rows counts
                recordsTotalPrgAdm = prgAdminData.Count();
                //Paging
                var dataPrgAdm = prgAdminData.Skip(skipPrgAdm).Take(pageSizePrgAdm).ToList();
                return Json(new { draw = drawPrgAdm, recordsFiltered = recordsTotalPrgAdm, recordsTotal = recordsTotalPrgAdm, data = dataPrgAdm });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllProgramLevelAdmins - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawPrgAdm, recordsFiltered = 0, recordsTotal = 0, data = new List<ProgramLevelAdminDto>() });
            }
        }
        /// <summary>
        /// This method is used to load all card holder agreements
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllCardholderAgreements(string id)
        {
            var drawCHA = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startCHA = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthCHA = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnCHA = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionCHA = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValueCHA = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizeCHA = lengthCHA != null ? Convert.ToInt32(lengthCHA) : 0;

                int skipCHA = startCHA != null ? Convert.ToInt32(startCHA) : 0;

                int recordsTotalCHA = 0;
                List<CardholderAgreementDto> cardHolderAgreements = new List<CardholderAgreementDto>();
                using (var clientCHA = new HttpClient())
                {
                    clientCHA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientCHA.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    //make it dynamic.

                    var programIdCHA = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    if (programIdCHA > 0)
                    {
                        var resultCHA = clientCHA.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CardholderAgreements + "?programId=" + programIdCHA).Result;
                        if (resultCHA.IsSuccessStatusCode)
                        {
                            var responseCHA = await resultCHA.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDesCHA = JsonConvert.DeserializeObject(responseCHA.Result.ToString());
                            cardHolderAgreements = responseDesCHA.ToObject<List<CardholderAgreementDto>>();
                        }
                    }
                }
                // Getting all Organisation Programs data
                var cardHolderData = (from tempOrgAdmin in cardHolderAgreements select tempOrgAdmin);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnCHA) && string.IsNullOrEmpty(sortColumnDirectionCHA)))
                {
                    cardHolderData = cardHolderData.OrderBy(sortColumnCHA + " " + sortColumnDirectionCHA);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValueCHA))
                {
                    cardHolderData = cardHolderData.Where(o => o.versionNo.ToLower().Trim().Contains(searchValueCHA.ToLower().Trim())
                                       || String.Format(o.CreatedDate.ToString("MMMM - dd{0} - yyyy"), GetSuffix(o.CreatedDate.Day.ToString())).ToLower().Trim().Contains(searchValueCHA.ToLower().Trim()));
                }

                //total number of rows counts
                recordsTotalCHA = cardHolderData.Count();
                //Paging
                var dataCHA = cardHolderData.Skip(skipCHA).Take(pageSizeCHA).ToList();
                return Json(new { draw = drawCHA, recordsFiltered = recordsTotalCHA, recordsTotal = recordsTotalCHA, data = dataCHA });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllCardholderAgreements - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawCHA, recordsFiltered = 0, recordsTotal = 0, data = new List<CardholderAgreementDto>() });
            }
        }
        /// <summary>
        /// This method is used to load all card holder agreement history.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllCardholderAgreementsHistory(string id)
        {
            var drawCHAHist = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startCHAHist = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthCHAHist = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnCHAHist = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionCHAHist = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValueCHAHist = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizeCHAHist = lengthCHAHist != null ? Convert.ToInt32(lengthCHAHist) : 0;

                int skipCHAHist = startCHAHist != null ? Convert.ToInt32(startCHAHist) : 0;

                int recordsTotalCHAHist = 0;
                List<UserAgreementHistoryDto> cardHolderAgreements = new List<UserAgreementHistoryDto>();
                using (var clientCHAHist = new HttpClient())
                {
                    clientCHAHist.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientCHAHist.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    //make it dynamic.

                    var programIdCHAHist = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    if (programIdCHAHist > 0)
                    {
                        var resultCHAHist = clientCHAHist.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CardholderAgreementsUserHistory + "?programId=" + programIdCHAHist).Result;
                        if (resultCHAHist.IsSuccessStatusCode)
                        {
                            var responseCHAHist = await resultCHAHist.Content.ReadAsAsync<ApiResponse>();
                            dynamic responseDesCHAHist = JsonConvert.DeserializeObject(responseCHAHist.Result.ToString());
                            cardHolderAgreements = responseDesCHAHist.ToObject<List<UserAgreementHistoryDto>>();

                        }
                    }
                }
                // Getting all Organisation Programs data
                var cardHolderDataHist = (from tempOrgAdmin in cardHolderAgreements select tempOrgAdmin);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnCHAHist) && string.IsNullOrEmpty(sortColumnDirectionCHAHist)))
                {
                    cardHolderDataHist = cardHolderDataHist.OrderBy(sortColumnCHAHist + " " + sortColumnDirectionCHAHist);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValueCHAHist))
                {
                    cardHolderDataHist = cardHolderDataHist.Where(o => o.CardHolderName.ToLower().Trim().Contains(searchValueCHAHist.ToLower().Trim()));
                }

                //total number of rows counts
                recordsTotalCHAHist = cardHolderDataHist.Count();
                //Paging
                var dataCHAHist = cardHolderDataHist.Skip(skipCHAHist).Take(pageSizeCHAHist).ToList();
                return Json(new { draw = drawCHAHist, recordsFiltered = recordsTotalCHAHist, recordsTotal = recordsTotalCHAHist, data = dataCHAHist });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllCardholderAgreementsHistory - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawCHAHist, recordsFiltered = 0, recordsTotal = 0, data = new List<UserAgreementHistoryDto>() });
            }
        }
        /// <summary>
        /// This method is used to post program level admin detail information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostProgramLevelAdminDetailInformation(ProgramLevelAdminDetailModel model)
        {
            string resultIdPostPrgAdmin = "0";
            string messagePostPrgAdmin = "";

            if (ModelState.IsValid)
            {
                try
                {
                    var dataProgramAdminDetail = new ProgramLevelAdminViewDetail()
                    {
                        EmailAddress = model.adminLevelModel.EmailAddress,
                        LastName = model.adminLevelModel.LastName,
                        Name = model.adminLevelModel.Name,
                        PhoneNumber = model.adminLevelModel.PhoneNumber,
                        ProgramsAccessibility = model.ProgramsAccessibility.Count == 1 && model.ProgramsAccessibility.FirstOrDefault().ProgramTypeId == 0 ? new List<ProgramLevelAdminTypeModel>() : _mapper.Map<List<ProgramLevelAdminTypeModel>>(model.ProgramsAccessibility),
                        ProgramId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.ProgramId)),
                        RoleId = model.adminLevelModel.RoleId,
                        UserId = model.adminLevelModel.UserId!=null? model.adminLevelModel.UserId.Value : 0,
                        UserImagePath = model.adminLevelModel.UserImagePath,
                        Custom1 = !string.IsNullOrEmpty(model.Custom1) ? model.Custom1 : string.Empty,
                        ProgramAdminAccessibility = model.AdminProgramAccessibility.Count == 1 && model.AdminProgramAccessibility.FirstOrDefault().programId == 0 ? new List<ProgramIdModel>() : _mapper.Map<List<ProgramIdModel>>(model.AdminProgramAccessibility)
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataProgramAdminDetail);
                        HttpContent stringContent = new StringContent(json.ToString());
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                        var resultPostPrgAdmin = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateProgramLevelAdminUser, stringContent);

                        if (resultPostPrgAdmin.IsSuccessStatusCode)
                        {
                            var responsePrgAdminPost = await resultPostPrgAdmin.Content.ReadAsAsync<ApiResponse>();
                            resultIdPostPrgAdmin = Convert.ToString(responsePrgAdminPost.Result);
                            messagePostPrgAdmin = Convert.ToString(responsePrgAdminPost.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostProgramLevelAdminDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultIdPostPrgAdmin, messagePostPrgAdmin });
        }
        /// <summary>
        /// This method is used to invite program level admin
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<IActionResult> InviteProgramLevelAdmin(string userEmail, string programId)
        {
            bool isInvited = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var mid = Convert.ToInt32(Cryptography.DecryptCipherToPlain(programId));
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.InviteProgramLevelAdmin + "?email=" + userEmail + "&id=" + mid).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        isInvited = true;
                    }

                }
                await Task.FromResult(0);
                //Returning Json Data  
                return Json(new { data = 1, isInvited });
            }
            catch (Exception)
            {
                return Json(new { data = 0, isInvited });
            }
        }
        /// <summary>
        /// This method is used to load all transactions for the program level.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadAllTransactions(string id, string dateMonth = "")
        {
            var drawPrgTransactions = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startPrgTransaction = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthPrgTransaction = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnPrgTransaction = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionPrgTransaction = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValuePrgTransaction = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizePrgTransaction = lengthPrgTransaction != null ? Convert.ToInt32(lengthPrgTransaction) : 0;

                int skipPrgTransaction = startPrgTransaction != null ? Convert.ToInt32(startPrgTransaction) : 0;

                int recordsTotalPrgTransaction = 0;
                List<TransactionViewDto> transactions = new List<TransactionViewDto>();
                using (var clientPrgTransaction = new HttpClient())
                {
                    var prgIdPrgTransaction = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(id)) : 0;
                    clientPrgTransaction.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgTransaction.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultPrgTransaction = clientPrgTransaction.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.Transactions + "?programId=" + prgIdPrgTransaction + "&dateMonth=" + dateMonth).Result;
                    if (resultPrgTransaction.IsSuccessStatusCode)
                    {
                        var responsePrgTransaction = await resultPrgTransaction.Content.ReadAsAsync<ApiResponse>();
                        if (responsePrgTransaction.Result != null)
                        {
                            transactions = JsonConvert.DeserializeObject<List<TransactionViewDto>>(responsePrgTransaction.Result.ToString());
                        }

                    }
                }
                // Getting all Organisation Programs data
                var prgTransaction = (from tempOrgAdmin in transactions select tempOrgAdmin);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnPrgTransaction) && string.IsNullOrEmpty(sortColumnDirectionPrgTransaction)))
                {
                    prgTransaction = prgTransaction.OrderBy(sortColumnPrgTransaction + " " + sortColumnDirectionPrgTransaction);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValuePrgTransaction))
                {
                    prgTransaction = prgTransaction.Where(o => Convert.ToString(o.Amount).ToLower().Trim().Contains(searchValuePrgTransaction.ToLower().Trim())
                    || o.AccountType.ToLower().Trim().Contains(searchValuePrgTransaction.ToLower().Trim())
                    || o.TransactionDate.ToString().ToLower().Trim().Contains(searchValuePrgTransaction.ToLower().Trim())
                    || o.MerchantName.ToLower().Trim().Contains(searchValuePrgTransaction.ToLower().Trim()));
                }

                //total number of rows counts
                recordsTotalPrgTransaction = prgTransaction.Count();
                //Paging
                var dataPrgTransaction = prgTransaction.Skip(skipPrgTransaction).Take(pageSizePrgTransaction).ToList();
                return Json(new { draw = drawPrgTransactions, recordsFiltered = recordsTotalPrgTransaction, recordsTotal = recordsTotalPrgTransaction, data = dataPrgTransaction });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllTransactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }
        /// <summary>
        /// This method is used to get the merchant listing.
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult MerchantListing(string poId, string ppId, string ppN, string poN)
        {
            return ViewComponent("MerchantListing", new { ppId, poId, ppN, poN });
        }
        /// <summary>
        /// This method is used to load merchant list data based on program Id.
        /// </summary>
        /// <param name="ppId"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadMerchantData(string ppId)
        {
            var drawMerchantLst = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                var primaryPrgIdMerchantLst = Cryptography.DecryptCipherToPlain(ppId);

                // Skip number of Rows count  
                var startMerchantLst = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var lengthMerchantLst = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumnMerchantLst = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirectionMerchantLst = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValueMerchantLst = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSizeMerchantLst = lengthMerchantLst != null ? Convert.ToInt32(lengthMerchantLst) : 0;

                int skipMerchantLst = startMerchantLst != null ? Convert.ToInt32(startMerchantLst) : 0;

                int recordsTotalMerchantLst = 0;
                List<MerchantDto> merchantLst = new List<MerchantDto>();
                using (var clientMerchantLst = new HttpClient())
                {
                    clientMerchantLst.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientMerchantLst.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultMerchantLst = clientMerchantLst.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MerchantListWithTransaction + "?programId=" + primaryPrgIdMerchantLst).Result;
                    if (resultMerchantLst.IsSuccessStatusCode)
                    {
                        var responseMerchantLst = await resultMerchantLst.Content.ReadAsAsync<apiModel.ApiResponse>();
                        dynamic responseDescMerchantLst = JsonConvert.DeserializeObject(responseMerchantLst.Result.ToString());
                        merchantLst = responseDescMerchantLst.ToObject<List<MerchantDto>>();
                        merchantLst.ForEach(x => { x.Jpos_MerchantEncId = !string.IsNullOrEmpty(x.Jpos_MerchantId) ? Cryptography.EncryptPlainToCipher(x.Jpos_MerchantId) : ""; x.Id = Cryptography.EncryptPlainToCipher(x.Id.ToString()); });
                    }
                }
                // getting all Customer data  
                var merchantListContent = (from tempcustomer in merchantLst
                                           select tempcustomer);
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumnMerchantLst) && string.IsNullOrEmpty(sortColumnDirectionMerchantLst)))
                {
                    merchantListContent = merchantListContent.OrderBy(sortColumnMerchantLst + " " + sortColumnDirectionMerchantLst);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValueMerchantLst))
                {
                    merchantListContent = merchantListContent.Where(m => m.MerchantName.ToLower().Trim().Contains(searchValueMerchantLst.ToLower().Trim())
                    || (m.Id != null && m.Id.ToLower().Trim().Contains(searchValueMerchantLst.ToLower().Trim()))
                    || (m.Location != null && m.Location.ToLower().Trim().Contains(searchValueMerchantLst.ToLower().Trim()))
                    || (m.AccountType != null && m.AccountType.ToLower().Trim().Contains(searchValueMerchantLst.ToLower().Trim()))
                    || (m.LastTransaction != null && m.LastTransaction.ToLower().Trim().Contains(searchValueMerchantLst.ToLower().Trim()))
                    || (m.DateAdded.ToString("dd MM yyyy").ToLower().Trim().Contains(searchValueMerchantLst.ToLower().Trim())));
                }
                //total number of rows counts   
                recordsTotalMerchantLst = merchantListContent.Count();
                //Paging   
                var dataMerchantLst = merchantListContent.Skip(skipMerchantLst).Take(pageSizeMerchantLst).ToList();
                //Returning Json Data  
                return Json(new { draw = drawMerchantLst, recordsFiltered = recordsTotalMerchantLst, recordsTotal = recordsTotalMerchantLst, data = dataMerchantLst });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadMerchantData - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawMerchantLst, recordsFiltered = 0, recordsTotal = 0, data = new List<MerchantDto>() });
            }
        }
        #region Import User
        /// <summary>
        /// This method is used to upload excel file for account holder.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadExcel(Microsoft.AspNetCore.Http.IFormFile file)
        {
            try
            {
                string sWebRootFolder = _env.WebRootPath + "/Import";
                DirectoryInfo dir = new DirectoryInfo(sWebRootFolder);
                if (!dir.Exists)
                {
                    dir.Create();
                    dir.Refresh();
                }

                string filePath = System.IO.Path.Combine(sWebRootFolder, file.FileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var fileSteam = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    file.CopyTo(fileSteam);
                }
                return Json(new { data = filePath });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (LoadAllTransactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = "" });
            }
        }
        /// <summary>
        /// This method is used to validate file for the imported account holder data.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateFile(Microsoft.AspNetCore.Http.IFormFile file)
        {
            try
            {
                int sampleSheetColCount;
                string folderName;
                ExcelWorksheet SampleWorksheet;
                FileInfo files;
                CreateFile(file, out sampleSheetColCount, out folderName, out SampleWorksheet, out files);
                return await ValidateFileRefactor(file, sampleSheetColCount, folderName, SampleWorksheet, files);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "Some error occured." + ex.Message, message = string.Empty });

            }
        }

        private async Task<IActionResult> ValidateFileRefactor(Microsoft.AspNetCore.Http.IFormFile file, int sampleSheetColCount, string folderName, ExcelWorksheet SampleWorksheet, FileInfo files)
        {
            try
            {
                bool isError = false;
                using (ExcelPackage package = new ExcelPackage(files))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;

                    if (rowCount < 2)
                    {
                        return Json(new { success = false, message = "No record found in document \n Please check the file" });
                    }

                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage result = await GetMaximumSheetRowsAPI(client);

                        if (result.IsSuccessStatusCode)
                        {
                            int maxRowsCount = await GetMaxRowFromResult(result);
                            if (rowCount > maxRowsCount)
                            {
                                return Json(new { success = false, message = "Number of rows are exceeded. Maximum uploaded rows are: " + maxRowsCount, fileName = file.FileName });
                            }
                        }
                    }

                    if (sampleSheetColCount != ColCount)
                    {
                        return Json(new { success = false, message = "Columns do not match.", fileName = file.FileName });
                    }


                    //Check Column names with sample sheet
                    for (int col = 1; col <= sampleSheetColCount; col++)
                    {
                        string HeaderColValue = worksheet.Cells[1, col].Value.ToString();
                        string HeaderSampleColValue = SampleWorksheet.Cells[1, col].Value.ToString();
                        if (HeaderColValue != HeaderSampleColValue)
                        {
                            return Json(new { success = false, message = "Column headers are different", fileName = file.FileName });
                        }
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        int loopCountColumn = ColCount - 2;
                        for (int col = 1; col <= loopCountColumn; col++)
                        {
                            bool iscolError = await ValidateEmailNSecondaryEmail(package, worksheet, ColCount, row, col);
                            if (!isError)
                                isError = iscolError;
                        }
                    }
                }
                return CheckForErrorInFile(file, folderName, isError);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "Some error occured while importing." + ex.Message, message = string.Empty });
            }
        }

        private static async Task<int> GetMaxRowFromResult(HttpResponseMessage result)
        {
            var response = await result.Content.ReadAsAsync<ApiResponse>();
            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
            var generalSettingObj = response1.ToObject<GeneralSettingDto>();
            int maxRowsCount = Convert.ToInt16(generalSettingObj.Value);
            return maxRowsCount;
        }

        private IActionResult CheckForErrorInFile(Microsoft.AspNetCore.Http.IFormFile file, string folderName, bool isError)
        {
            if (isError)
            {
                return Json(new { success = false, data = folderName + "/" + file.FileName, message = "File is not validated \n  Reason for failure can be checked in status column by downloading the file through download link.", fileName = file.FileName });
            }
            else
            {
                return Json(new { success = true, data = folderName + "/" + file.FileName, message = string.Empty });
            }
        }

        private async Task<HttpResponseMessage> GetMaximumSheetRowsAPI(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

            var result = await client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetMaximumSheetRows);
            return result;
        }

        private void CreateFile(Microsoft.AspNetCore.Http.IFormFile file, out int sampleSheetColCount, out string folderName, out ExcelWorksheet SampleWorksheet, out FileInfo files)
        {
            sampleSheetColCount = 0;
            folderName = "/Import";
            string sWebRootFolder = _env.WebRootPath + folderName;
            string sWebRootFolderSample = _env.WebRootPath + folderName + "/Sample";
            string sFileName = file.FileName;
            string sFileName1 = @"demo.xlsx";

            FileInfo file1 = new FileInfo(Path.Combine(sWebRootFolderSample, sFileName1));
            ExcelPackage package1 = new ExcelPackage(file1);
            SampleWorksheet = package1.Workbook.Worksheets[1];
            sampleSheetColCount = SampleWorksheet.Dimension.Columns;

            files = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        }

        /// <summary>
        /// This method is used to validate account holder's primary and secondary email for the imported data.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="worksheet"></param>
        /// <param name="ColCount"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private async Task<bool> ValidateEmailNSecondaryEmail(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col)
        {
            var isError = false;


            isError = await CheckForMailAddressValidity(package, worksheet, ColCount, row, col, isError);
            return isError;
        }

        private async Task<bool> CheckForMailAddressValidity(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, bool isError)
        {
            var namePattern = @"^[a-zA-Z ]+$";
            var phonePattern = "^((\\(\\d{3}\\) ?)|(\\d{3}-))?\\d{3}-\\d{4}$";
            var headerValue = worksheet.Cells[1, col].Value.ToString();  // Get header text by column number
            switch (headerValue)
            {
                case nameof(FieldSet.First_Name):
                    var enumColumnValue = Convert.ToInt16(FieldSet.First_Name);
                    var nullOrEmptyMsg = headerValue + " is null or Empty \n";
                    var lengthExceedMsg = headerValue + " length exceeded \n";
                    isError = ValidateColumnValueAndLength(package, worksheet, ColCount, row, col, enumColumnValue, nullOrEmptyMsg + "^" + lengthExceedMsg);
                    if (!isError)
                    {
                        isError = ValidateDataForNumberRegex(package, worksheet, ColCount, row, col, namePattern, headerValue);
                    }

                    break;
                case nameof(FieldSet.Last_Name):
                    enumColumnValue = Convert.ToInt16(FieldSet.Last_Name);
                    nullOrEmptyMsg = headerValue + " is null or Empty \n";
                    lengthExceedMsg = headerValue + " length exceeded \n";
                    isError = ValidateColumnValueAndLength(package, worksheet, ColCount, row, col, enumColumnValue, nullOrEmptyMsg + "^" + lengthExceedMsg);
                    if (!isError)
                    {
                        isError = ValidateNumberREgex(headerValue, package, worksheet, ColCount, row, col, namePattern);
                    }
                    break;
                case nameof(FieldSet.Email):
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        GiveFailureErrorForSheet(package, worksheet, ColCount, row);
                    }
                    else
                    {
                        await EmailCheck(package, worksheet, ColCount, row, col);
                    }
                    break;
                case nameof(FieldSet.Phone_Number):
                    SetMessagesForPhoneNum(headerValue, out enumColumnValue, out nullOrEmptyMsg, out lengthExceedMsg);
                    isError = ValidateColumnValueAndLength(package, worksheet, ColCount, row, col, enumColumnValue, nullOrEmptyMsg + "^" + lengthExceedMsg);
                    if (!isError)
                    {
                        isError = await CheckForPhoneNumberExistence(package, worksheet, ColCount, row, col, phonePattern, headerValue);
                    }
                    break;
                case nameof(FieldSet.Date_Of_Birth):
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        isError = DateOfBirthFailure(worksheet, ColCount, row);
                    }
                    else
                    {
                        isError = DateOfBirthCheck(worksheet, row, col, headerValue, out enumColumnValue, out nullOrEmptyMsg, out lengthExceedMsg);
                    }

                    break;
                case nameof(FieldSet.Account_Holder_Unique_Id):
                    enumColumnValue = Convert.ToInt16(FieldSet.Account_Holder_Unique_Id);
                    nullOrEmptyMsg = headerValue + " is null or Empty \n";
                    lengthExceedMsg = headerValue + " length exceeded \n";
                    isError = await ValidateColumnValNLength(package, worksheet, ColCount, row, col, enumColumnValue, nullOrEmptyMsg + "^" + lengthExceedMsg);

                    break;
                case nameof(FieldSet.Secondary_Email):
                    if (worksheet.Cells[row, col].Value != null)
                    {
                        string email = worksheet.Cells[row, col].Value.ToString();
                        CheckSecondaryEmail(package, worksheet, ColCount, row, email);
                    }

                    break;
                case nameof(FieldSet.Home_Address):
                    enumColumnValue = Convert.ToInt16(FieldSet.Home_Address);
                    nullOrEmptyMsg = headerValue + " is null or Empty \n";
                    lengthExceedMsg = headerValue + " length exceeded \n";
                    isError = ValidateColumnValueAndLength(package, worksheet, ColCount, row, col, enumColumnValue, nullOrEmptyMsg + "^" + lengthExceedMsg);
                    break;
                default:
                    break;
            }

            return isError;
        }

        private static void CheckSecondaryEmail(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, string email)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + "Invalid secondary email";
                package.Save();
            }
        }

        private async Task<bool> ValidateColumnValNLength(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, short enumColumnValue, string nullOrEmptyMsgNlengthExceedMsg)
        {
            bool isError = ValidateColumnValueAndLength(package, worksheet, ColCount, row, col, enumColumnValue, nullOrEmptyMsgNlengthExceedMsg);
            if (!isError)
            {
                //check account holder existence
                var client = GetHttpClient();
                string userCode = worksheet.Cells[row, col].Value.ToString();
                bool isUserCodeExists = await GetUserbyUserCode(userCode, client);
                if (isUserCodeExists)
                {
                    worksheet.Cells[row, (ColCount - 1)].Value = "Account holder unique id exists";
                    worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + "Account Holder Unique Id  Already Exists \n";
                    package.Save();
                    isError = true;
                }
            }

            return isError;
        }

        private static bool DateOfBirthFailure(ExcelWorksheet worksheet, int ColCount, int row)
        {
            bool isError;
            worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
            worksheet.Cells[row, ColCount].Value = "Date Of Birth is null or empty.";
            isError = true;
            return isError;
        }

        private bool DateOfBirthCheck(ExcelWorksheet worksheet, int row, int col, string headerValue, out short enumColumnValue, out string nullOrEmptyMsg, out string lengthExceedMsg)
        {
            enumColumnValue = Convert.ToInt16(FieldSet.Date_Of_Birth);
            nullOrEmptyMsg = headerValue + " is null or Empty \n";
            lengthExceedMsg = headerValue + " length exceeded \n";
            return ValidateDateTime(worksheet, row, col);
        }

        private async Task EmailCheck(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col)
        {
            string secEmail = worksheet.Cells[row, col].Value.ToString();
            bool isSecEmail = Regex.IsMatch(secEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (!isSecEmail)
            {
                GiveFailureErrorForMail(package, worksheet, ColCount, row);
            }
            else
            {
                await CheckForExistenceOfEmailRefactor(package, worksheet, ColCount, row, col);
            }
        }

        private static void SetMessagesForPhoneNum(string headerValue, out short enumColumnValue, out string nullOrEmptyMsg, out string lengthExceedMsg)
        {
            enumColumnValue = Convert.ToInt16(FieldSet.Phone_Number);
            nullOrEmptyMsg = headerValue + " is null or Empty \n";
            lengthExceedMsg = headerValue + " length exceeded \n";
        }

        private async Task<bool> CheckForPhoneNumberExistence(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, string phonePattern, string headerValue)
        {
            bool isError;
            //check email existence
            var client = GetHttpClient();
            string phone = worksheet.Cells[row, col].Value.ToString();
            bool isPhoneExists = await CheckPhoneNumberExistence(phone, client);
            if (isPhoneExists)
            {
                worksheet.Cells[row, (ColCount - 1)].Value = "Phone number exists";
                worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + "Phone number Already Exists \n";
                package.Save();
                isError = true;
            }
            else
            {
                string regexErrorMsg = headerValue + " contains wrong pattern \n";
                isError = ValidatePhoneNumberRegex(regexErrorMsg, package, worksheet, ColCount, row, col, phonePattern);
            }

            return isError;
        }

        private async Task CheckForExistenceOfEmailRefactor(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col)
        {
            //check email existence
            var client = GetHttpClient();
            string email = worksheet.Cells[row, col].Value.ToString();
            try
            {
                await CheckForExistenceOfEmail(package, worksheet, ColCount, row, client, email);
            }
            catch (Exception ex)
            {
                ExceptionForEmail(package, worksheet, ColCount, row, ex);
            }
        }

        private static void ExceptionForEmail(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, Exception ex)
        {
            worksheet.Cells[row, (ColCount - 1)].Value = "Error";
            worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + ex.Message + " \n";
            package.Save();
        }

        private async Task CheckForExistenceOfEmail(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, HttpClient client, string email)
        {
            bool isEmailExists = await CheckUserbyEmail(email, client);
            if (isEmailExists)
            {
                worksheet.Cells[row, (ColCount - 1)].Value = "Email Exists";
                worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + "Email Already Exists \n";
                package.Save();
            }
        }

        private static void GiveFailureErrorForMail(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row)
        {
            worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
            worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + "Invalid Email \n";
            package.Save();
        }

        private static void GiveFailureErrorForSheet(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row)
        {
            worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
            worksheet.Cells[row, ColCount].Value = "Email is null or empty.";
            package.Save();
        }

        private static bool ValidateDataForNumberRegex(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, string namePattern, string headerValue)
        {
            bool isError;
            string regexErrorMsg = headerValue + " contains only letters \n";
            isError = ValidateNumberREgex(regexErrorMsg, package, worksheet, ColCount, row, col, namePattern);
            return isError;
        }

        /// <summary>
        /// This method is used to validate number using the given expression.
        /// </summary>
        /// <param name="regexErrorMsg"></param>
        /// <param name="package"></param>
        /// <param name="worksheet"></param>
        /// <param name="ColCount"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="namePattern"></param>
        /// <returns></returns>
        private static bool ValidateNumberREgex(string regexErrorMsg, ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, string namePattern)
        {
            bool isError = !Regex.IsMatch(worksheet.Cells[row, col].Value.ToString(), namePattern);
            if (isError)
            {
                worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
                worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + " " + regexErrorMsg;
                package.Save();
            }
            return isError;
        }
        /// <summary>
        /// This method is used to validate phone number expression.
        /// </summary>
        /// <param name="regexErrorMsg"></param>
        /// <param name="package"></param>
        /// <param name="worksheet"></param>
        /// <param name="ColCount"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="phonePattern"></param>
        /// <returns></returns>
        private static bool ValidatePhoneNumberRegex(string regexErrorMsg, ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, string phonePattern)
        {
            bool isError = !Regex.IsMatch(worksheet.Cells[row, col].Value.ToString(), phonePattern);
            if (isError)
            {
                worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
                worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + " " + regexErrorMsg;
                package.Save();
            }
            return isError;
        }
        /// <summary>
        /// This method is used to get the HttpClient object.
        /// </summary>
        /// <returns></returns>
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            return client;
        }
        /// <summary>
        /// This method is used to check the user by given email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<bool> CheckUserbyEmail(string email, HttpClient client)
        {
            bool isEmailExist = false;
            var result = await client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CheckUserByEmail + "?email=" + email);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
                if (response.Result != null)
                {
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    UserDto userObj = response1.ToObject<UserDto>();
                    if (userObj.Id > 0)
                        isEmailExist = true;
                }
            }
            return isEmailExist;
        }
        /// <summary>
        /// This method is used to check the phone number existence.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<bool> CheckPhoneNumberExistence(string phoneNumber, HttpClient client)
        {
            bool isPhoneExist = false;
            var result = await client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CheckPhoneNumberExistence + "?phoneNumber=" + phoneNumber);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
                if (response.Result != null)
                {
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    UserDto userObj = response1.ToObject<UserDto>();
                    if (userObj?.Id > 0)
                        isPhoneExist = true;
                }
            }
            return isPhoneExist;
        }
        /// <summary>
        /// This method is used to get the user code based on given program.
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<bool> GetUserbyUserCode(string userCode, HttpClient client)
        {
            bool isUserCodeExist = false;
            var result = await client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserbyUserCode + "?userCode=" + userCode);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsAsync<ApiResponse>();
                if (response.Result != null)
                {
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    UserDto userObj = response1.ToObject<UserDto>();
                    if (userObj?.Id > 0)
                        isUserCodeExist = true;
                }
            }
            return isUserCodeExist;
        }
        /// <summary>
        /// This method is used to validate data given in the excel sheet for the import account holder data.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="worksheet"></param>
        /// <param name="colCount"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="enumColumnValue"></param>
        /// <param name="nullOrEmptyMsg"></param>
        /// <param name="lengthExceedMsg"></param>
        /// <returns></returns>
        private bool ValidateDateTime(ExcelWorksheet worksheet, int row, int col)
        {

            DateTime dt;
            string[] formats = { "MM/dd/yyyy", "M/d/yyyy" };
            var a = Convert.ToDateTime(worksheet.Cells[row, col].Value.ToString()).ToString("d");
            if (DateTime.TryParseExact(a, formats, CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out dt))
            {
                // Successfully parse
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// This method is used validate column length and value with the database schema and excel.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="worksheet"></param>
        /// <param name="ColCount"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="enumColumnValue"></param>
        /// <param name="nullOrEmptyMsg"></param>
        /// <param name="lengthExceedMsg"></param>
        /// <returns></returns>
        private static bool ValidateColumnValueAndLength(ExcelPackage package, ExcelWorksheet worksheet, int ColCount, int row, int col, int enumColumnValue, string nullOrEmptyMsgOrlengthExceedMsg)
        {
            bool isError = false;
            var nullOrEmptyMsgOrlengthExceedMsgSplit = nullOrEmptyMsgOrlengthExceedMsg.Split("^");
            if (worksheet.Cells[row, col].Value == null)
            {
                isError = true;
                worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
                worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + nullOrEmptyMsgOrlengthExceedMsgSplit[0];
                package.Save();
            }
            else
            {

                worksheet.Cells[row, (ColCount - 1)].Value = "Failure";
                int valueLength = worksheet.Cells[row, col].Value.ToString().Length;
                int length = enumColumnValue;
                if (valueLength > length)
                {
                    isError = true;
                    worksheet.Cells[row, ColCount].Value = worksheet.Cells[row, ColCount].Value + nullOrEmptyMsgOrlengthExceedMsgSplit[1];
                    package.Save();
                }
            }
            return isError;
        }
        /// <summary>
        /// This method is used to import the account holder detail from excel in Database.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="programId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Import(string fileName, string programId, string orgId)
        {
            int prgId, organizationId;
            string folderName, sWebRootFolder;
            DataInitialization(programId, orgId, out prgId, out organizationId, out folderName, out sWebRootFolder);

            FileInfo files = new FileInfo(Path.Combine(sWebRootFolder, fileName));
            return await ExcelPackageDataImport(fileName, prgId, organizationId, folderName, files);
        }

        private async Task<IActionResult> ExcelPackageDataImport(string fileName, int prgId, int organizationId, string folderName, FileInfo files)
        {
            using (ExcelPackage package = new ExcelPackage(files))
            {
                ExcelWorksheet worksheet;
                int rowCount, ColCount;
                User userObj;
                GetRowColFromExcelAndIntializeUser(package, out worksheet, out rowCount, out ColCount, out userObj);
                try
                {
                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (worksheet.Cells[row, (ColCount - 1)].Value == null || worksheet.Cells[row, (ColCount - 1)].Value.ToString() == "Failure")
                        {
                            ModalBindingForUserDetail(prgId, organizationId, worksheet, userObj, row);
                            using (var client = new HttpClient())
                            {
                                string json;
                                HttpContent stringContent;
                                SerializeJsonNStringContentDataImport(userObj, out json, out stringContent);
                                HttpResponseMessage result = await CallingAddUserFromExcelAPI(client, stringContent);
                                if (result.IsSuccessStatusCode)
                                {
                                    List<int> userIds = await SuccessMessageNUserIds(worksheet, ColCount, userObj, row, result);
                                    StringContentNRequestHeadersForMagicLink(client, out json, out stringContent, userIds);
                                    try
                                    {
                                        ApiResponse response2 = await SendMagicLinkApiCall(client, stringContent);
                                        if (response2.Result != null)
                                        {
                                            FirebaseDynamicResultModel objFirebaseResponse = FireBaseResp(response2);
                                            if (objFirebaseResponse == null || string.IsNullOrEmpty(objFirebaseResponse.result.shortLink))
                                            {
                                                return ReturnShortLink(fileName, folderName);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        HttpContext.RiseError(new Exception(string.Concat("Web: Program (Import (Inner) - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                                        return Json(new { success = false, data = folderName + "/" + fileName, message = ex.Message });
                                    }
                                }
                                else
                                {
                                    return Json(new { success = false, data = folderName + "/" + fileName, message = "failure" });
                                }
                            }
                        }
                    }
                    package.Save();

                    return Json(new { success = true, data = folderName + "/" + fileName, message = "The file is imported successfully \n Please review the excel sheet" });
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Program (Import (Outer)- GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                    return Json(new { success = false, data = folderName + "/" + fileName, message = "failure" });
                }
            }
        }

        private static void SerializeJsonNStringContentDataImport(User userObj, out string json, out HttpContent stringContent)
        {
            json = JsonConvert.SerializeObject(userObj);
            stringContent = new StringContent(json.ToString());
        }

        private static FirebaseDynamicResultModel FireBaseResp(ApiResponse response2)
        {
            dynamic response3 = JsonConvert.DeserializeObject(response2.Result.ToString());
            FirebaseDynamicResultModel objFirebaseResponse = response3.ToObject<FirebaseDynamicResultModel>();
            return objFirebaseResponse;
        }

        private IActionResult ReturnShortLink(string fileName, string folderName)
        {
            return Json(new { success = false, data = folderName + "/" + fileName, message = "failure" });
        }

        private async Task<ApiResponse> SendMagicLinkApiCall(HttpClient client, HttpContent stringContent)
        {
            var objResult = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.SendMagicLinkInEmail, stringContent);
            var response2 = await objResult.Content.ReadAsAsync<ApiResponse>();
            return response2;
        }

        private void DataInitialization(string programId, string orgId, out int prgId, out int organizationId, out string folderName, out string sWebRootFolder)
        {
            prgId = !string.IsNullOrEmpty(programId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(programId)) : 0;
            organizationId = !string.IsNullOrEmpty(orgId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(orgId)) : 0;
            folderName = "/Import";
            sWebRootFolder = _env.WebRootPath + folderName;
        }

        private static void GetRowColFromExcelAndIntializeUser(ExcelPackage package, out ExcelWorksheet worksheet, out int rowCount, out int ColCount, out User userObj)
        {
            worksheet = package.Workbook.Worksheets[1];
            rowCount = worksheet.Dimension.Rows;
            ColCount = worksheet.Dimension.Columns;
            userObj = new User();
        }

        private void StringContentNRequestHeadersForMagicLink(HttpClient client, out string json, out HttpContent stringContent, List<int> userIds)
        {
            json = JsonConvert.SerializeObject(userIds);
            stringContent = new StringContent(json.ToString());
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
        }

        private static async Task<List<int>> SuccessMessageNUserIds(ExcelWorksheet worksheet, int ColCount, User userObj, int row, HttpResponseMessage result)
        {
            var response = await result.Content.ReadAsAsync<ApiResponse>();
            var resultId = Convert.ToString(response.Result);

            worksheet.Cells[row, (ColCount - 1)].Value = "Success";
            worksheet.Cells[row, ColCount].Value = string.Empty;

            userObj.Id = Convert.ToInt32(resultId);

            //Get short link from firebase and send in mail
            var userIds = new List<int>
                                    {
                                        userObj.Id
                                    };
            return userIds;
        }

        private async Task<HttpResponseMessage> CallingAddUserFromExcelAPI(HttpClient client, HttpContent stringContent)
        {
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

            var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUserFromExcel, stringContent);
            return result;
        }

        private static void ModalBindingForUserDetail(int prgId, int organizationId, ExcelWorksheet worksheet, User userObj, int row)
        {
            userObj.FirstName = worksheet.Cells[row, 1].Value.ToString().Trim();
            userObj.LastName = worksheet.Cells[row, 2].Value.ToString().Trim();
            userObj.Email = worksheet.Cells[row, 3].Value.ToString().Trim();
            userObj.PhoneNumber = worksheet.Cells[row, 4].Value.ToString().Trim();
            userObj.dateOfBirth = Convert.ToDateTime(worksheet.Cells[row, 5].Value.ToString().Trim());
            userObj.UserCode = worksheet.Cells[row, 6].Value != null ? worksheet.Cells[row, 6].Value.ToString().Trim() : string.Empty;
            userObj.secondaryEmail = worksheet.Cells[row, 7].Value != null ? worksheet.Cells[row, 7].Value.ToString().Trim() : string.Empty;
            userObj.Address = worksheet.Cells[row, 8].Value != null ? worksheet.Cells[row, 8].Value.ToString().Trim() : string.Empty;
            userObj.ProgramId = prgId;
            userObj.OrganisationId = organizationId;
        }

        /// <summary>
        /// This method is used to load all programs 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoadAllProgram()
        {
            var drawAllPrg = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                //Skip number of Rows count
                var startAllPrg = Request.Form["start"].FirstOrDefault();

                //Paging Length 10,20
                var lengthAllPrg = Request.Form["length"].FirstOrDefault();

                //Sort Column Name
                var sortColumnAllPrg = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                //Sort Column Direction(asc,desc)
                var sortColumnDirectionAllPrg = Request.Form["order[0][dir]"].FirstOrDefault();

                //Search Value from (Search box)
                var searchValueAllPrg = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50, 100)
                int pageSizeAllPrg = lengthAllPrg != null ? Convert.ToInt32(lengthAllPrg) : 0;

                int skipAllPrg = startAllPrg != null ? Convert.ToInt32(startAllPrg) : 0;

                int recordsTotalAllPrg = 0;
                List<ProgramListDto> allPrograms = new List<ProgramListDto>();

                using (var clientAllPrg = new HttpClient())
                {
                    clientAllPrg.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientAllPrg.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultAllPrg = clientAllPrg.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllProgramsList + "?isActive=" + true + "&isDeleted=" + false).Result;
                    if (resultAllPrg.IsSuccessStatusCode)
                    {
                        var responseAllPrg = await resultAllPrg.Content.ReadAsAsync<ApiResponse>();
                        if (responseAllPrg.StatusFlagNum == 1)
                        {
                            dynamic response1 = JsonConvert.DeserializeObject(responseAllPrg.Result.ToString());
                            allPrograms = response1.ToObject<List<ProgramListDto>>();
                            allPrograms.ForEach(x => { x.strProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()); x.EncProgName = Cryptography.EncryptPlainToCipher(x.ProgramName); x.OrganisationEncId = Cryptography.EncryptPlainToCipher(x.OrganisationId.ToString()); x.EncOrganisationName = Cryptography.EncryptPlainToCipher(x.OrganisationName.ToString()); });
                        }

                    }
                }
                // Getting all Organisation Programs data
                var allProgramData = (from tempallPrg in allPrograms select tempallPrg).AsEnumerable();

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnAllPrg) && string.IsNullOrEmpty(sortColumnDirectionAllPrg)))
                {
                    allProgramData = allProgramData.OrderBy(sortColumnAllPrg + " " + sortColumnDirectionAllPrg);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValueAllPrg))
                {
                    allProgramData = allProgramData.Where(o => o.ProgramName.ToLower().Trim().Contains(searchValueAllPrg.ToLower().Trim())
                    || o.ProgramCodeId.ToLower().Trim().Contains(searchValueAllPrg.ToLower().Trim())
                    || o.ProgramType.ToLower().Trim().Contains(searchValueAllPrg.ToLower().Trim())
                    || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), GetSuffix(o.DateAdded.Day.ToString())).ToLower().Trim().Contains(searchValueAllPrg.ToLower().Trim())
                    || (o.OrganisationSubTitle != null && o.OrganisationSubTitle.ToLower().Trim().Contains(searchValueAllPrg.ToLower().Trim()))).AsEnumerable();
                }
                //total number of rows counts
                recordsTotalAllPrg = allProgramData.Count();
                //Paging
                var dataAllPrg = allProgramData.Skip(skipAllPrg).Take(pageSizeAllPrg).ToList();
                return Json(new { draw = drawAllPrg, recordsFiltered = recordsTotalAllPrg, recordsTotal = recordsTotalAllPrg, data = dataAllPrg });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllProgram - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawAllPrg, recordsFiltered = 0, recordsTotal = 0, data = new List<ProgramListDto>() });
            }
        }
        /// <summary>
        /// This method is used to post program information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostProgramInformation(ProgramInfoModel model)
        {
            string resultId = "0";
            if (ModelState.IsValid)
            {
                try
                {
                    var dataProgramDetail = new ProgramInfoDto()
                    {
                        AccountHolderGroups = model.AccountHolderGroups,
                        AccountHolderUniqueId = model.AccountHolderUniqueId,
                        address = model.address,
                        city = model.city,
                        country = model.country,
                        createdBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value),
                        createdDate = DateTime.UtcNow,
                        customErrorMessaging = model.customErrorMessaging,
                        customInputMask = model.customInputMask,
                        customInstructions = model.customInstructions,
                        customName = model.customName,
                        id = !string.IsNullOrEmpty(model.ProgramEncId) && model.ProgramEncId != "0" ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.ProgramEncId)) : 0,
                        modifiedBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value),
                        modifiedDate = DateTime.UtcNow,
                        organisationId = !string.IsNullOrEmpty(model.OrganizationEncId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.OrganizationEncId)) : 0,
                        ProgramCodeId = model.ProgramCodeId,
                        name = model.name,
                        programCustomFields = model.programCustomFields,
                        ProgramTypeId = model.ProgramTypeId,
                        state = model.state,
                        timeZone = model.timeZone,
                        website = model.website,
                        zipcode = model.zipcode,
                        IsAllNotificationShow = model.IsAllNotificationShow,
                        IsRewardsShowInApp = model.IsRewardsShowInApp,
                        JPOS_IssuerId = model.JPOS_IssuerId
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataProgramDetail);
                        HttpContent stringContent = new StringContent(json.ToString());
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                        var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateProgramInfo, stringContent);

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
                    HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostProgramInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId, programName = Cryptography.EncryptPlainToCipher(model.name) });
        }
        /// <summary>
        /// This method is used to delete program.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="JPOS_IssuerId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteProgram(string Id, string JPOS_IssuerId)
        {
            int resultIdDelPrg = 0;
            try
            {
                using (var clientDelPrg = new HttpClient())
                {
                    var proDtoDel = new ProgramListDto()
                    {
                        ProgramId = !string.IsNullOrEmpty(Id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(Id)) : 0,
                        JPOS_IssuerId = JPOS_IssuerId
                    };
                    string jsonDelPrg = Newtonsoft.Json.JsonConvert.SerializeObject(proDtoDel);
                    HttpContent stringContentDelPrg = new StringContent(jsonDelPrg);
                    stringContentDelPrg.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelPrg.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultDelPrg = await clientDelPrg.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteProgram, stringContentDelPrg);
                    if (resultDelPrg.IsSuccessStatusCode)
                    {
                        var responseDelPrg = await resultDelPrg.Content.ReadAsAsync<ApiResponse>();
                        resultIdDelPrg = Convert.ToInt32(responseDelPrg.Result);
                    }
                }
                return Json(new { data = resultIdDelPrg, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (DeleteProgram - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdDelPrg, success = false });
            }
        }

        #endregion

        #region Plan
        /// <summary>
        /// This method is used to get the plan list from the system based on program.
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult PlanList(string poId, string ppId, string ppN, string poN)
        {
            return ViewComponent("PlanList", new { ppId, poId, ppN, poN });
        }
        /// <summary>
        /// This method is used to load plan data based on its id.
        /// </summary>
        /// <param name="ppId"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadPlanData(string ppId)
        {
            var drawPrgPlans = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                var primaryPrgIdPrgPlan = Cryptography.DecryptCipherToPlain(ppId);

                // Skip number of Rows count  
                var startPrgPlan = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var lengthPrgPlan = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumnPrgPlan = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirectionPrgPlan = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValuePrgPlan = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSizePrgPlan = lengthPrgPlan != null ? Convert.ToInt32(lengthPrgPlan) : 0;

                int skipPrgPlan = startPrgPlan != null ? Convert.ToInt32(startPrgPlan) : 0;

                int recordsTotalPrgPlan = 0;
                List<PlanListingDto> plans = new List<PlanListingDto>();
                using (var clientPrgPlans = new HttpClient())
                {
                    clientPrgPlans.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgPlans.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultPrgPlans = clientPrgPlans.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.PlanListing + "?programId=" + primaryPrgIdPrgPlan).Result;
                    if (resultPrgPlans.IsSuccessStatusCode)
                    {
                        var responsePrgPlans = await resultPrgPlans.Content.ReadAsAsync<apiModel.ApiResponse>();
                        if (responsePrgPlans.StatusFlagNum == 1)
                        {
                            dynamic responseDesPrgPlans = JsonConvert.DeserializeObject(responsePrgPlans.Result.ToString());
                            plans = responseDesPrgPlans.ToObject<List<PlanListingDto>>();
                            plans.ForEach(x => { x.StrId = Cryptography.EncryptPlainToCipher(x.Id.ToString()); x.Jpos_PlanEncId = !string.IsNullOrEmpty(x.Jpos_PlanId) ? Cryptography.EncryptPlainToCipher(x.Jpos_PlanId) : ""; });
                        }
                    }
                }
                // getting all Customer data  
                var plansData = (from tempdata in plans
                                 select tempdata);
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumnPrgPlan) && string.IsNullOrEmpty(sortColumnDirectionPrgPlan)))
                {
                    plansData = plansData.OrderBy(sortColumnPrgPlan + " " + sortColumnDirectionPrgPlan);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValuePrgPlan))
                {
                    plansData = plansData.Where(m => m.Name.ToLower().Trim().Contains(searchValuePrgPlan.ToLower().Trim())
                    || m.Accounts.ToLower().Trim().Contains(searchValuePrgPlan.ToLower().Trim())
                    || m.ClientId.ToLower().Trim().Contains(searchValuePrgPlan.ToLower().Trim())
                    || m.Description.ToLower().Trim().Contains(searchValuePrgPlan.ToLower().Trim())
                    || m.InternalId.ToLower().Trim().Contains(searchValuePrgPlan.ToLower().Trim())
                    || m.StartEnd.ToLower().Trim().Contains(searchValuePrgPlan.ToLower().Trim()));
                }

                //total number of rows counts   
                recordsTotalPrgPlan = plansData.Count();
                //Paging   
                var dataPrgPlans = plansData.Skip(skipPrgPlan).Take(pageSizePrgPlan).ToList();
                //Returning Json Data  
                return Json(new { draw = drawPrgPlans, recordsFiltered = recordsTotalPrgPlan, recordsTotal = recordsTotalPrgPlan, data = dataPrgPlans });

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadPlanData - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawPrgPlans, recordsFiltered = 0, recordsTotal = 0, data = new List<PlanListingDto>() });
            }
        }
        /// <summary>
        /// This method is used to change status of the plan in the datatable.
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePlanStatus(int planId, bool isActive)
        {
            int resultIdPlanStatus = 0;
            try
            {
                PlanListingDto adminStatus = new PlanListingDto()
                {
                    Id = planId,
                    Status = isActive
                };
                using (var clientPlanStatus = new HttpClient())
                {
                    string jsonPlanStatus = Newtonsoft.Json.JsonConvert.SerializeObject(adminStatus);

                    HttpContent stringContentPlanStatus = new StringContent(jsonPlanStatus);
                    stringContentPlanStatus.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientPlanStatus.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPlanStatus.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultPlanStatus = await clientPlanStatus.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UpdatePlanStatus, stringContentPlanStatus);
                    if (resultPlanStatus.IsSuccessStatusCode)
                    {
                        var responsePlanStatus = await resultPlanStatus.Content.ReadAsAsync<ApiResponse>();
                        if (responsePlanStatus.StatusFlagNum == 1)
                        {
                            resultIdPlanStatus = Convert.ToInt32(responsePlanStatus.Result);
                        }

                    }
                }
                return Json(new { data = resultIdPlanStatus, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (ChangePlanStatus - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdPlanStatus, success = false });
            }
        }
        /// <summary>
        /// This method is used to delete plan from the program it will be a soft deletion.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Jpos_PlanId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeletePlan(string ID, string Jpos_PlanId)
        {
            int resultIdDelPlan = 0;
            try
            {
                using (var clientDelPlan = new HttpClient())
                {
                    var orgDtoDelPlan = new PlanListingDto()
                    {
                        Id = !string.IsNullOrEmpty(ID) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(ID)) : 0,
                        Jpos_PlanId = !(string.IsNullOrEmpty(Jpos_PlanId)) ? Cryptography.DecryptCipherToPlain(Jpos_PlanId) : ""
                    };
                    string jsonDelPlan = Newtonsoft.Json.JsonConvert.SerializeObject(orgDtoDelPlan);

                    HttpContent stringContentDelPlan = new StringContent(jsonDelPlan);
                    stringContentDelPlan.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelPlan.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultDelPlan = await clientDelPlan.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeletePlan, stringContentDelPlan);
                    if (resultDelPlan.IsSuccessStatusCode)
                    {
                        var responseDelPlan = await resultDelPlan.Content.ReadAsAsync<apiModel.ApiResponse>();
                        if (responseDelPlan.StatusFlagNum == 1)
                        {
                            resultIdDelPlan = Convert.ToInt32(responseDelPlan.Result);
                        }
                    }
                }
                return Json(new { draw = resultIdDelPlan, success = true, message = "" });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (DeletePlan - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = resultIdDelPlan, success = false, message = "" });
            }
        }
        /// <summary>
        /// This method is used to clone the plan from the plan list.
        /// </summary>
        /// <param name="planId">PlanId</param>
        /// <param name="poId">OrganizationId</param>
        /// <param name="ppId">ProgramId</param>
        /// <param name="ppN">ProgramName</param>
        /// <returns>cloned Id of plan</returns>
        [HttpPost]
        public async Task<IActionResult> ClonePlan(string planId, string poId, string ppId, string ppN)
        {
            int resultId = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    var proDto = new PlanListingDto()
                    {
                        Id = !string.IsNullOrEmpty(planId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(planId)) : 0,
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(proDto);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ClonePlan, stringContent);

                }
                return RedirectToAction("PlanList", new { poId, ppId, ppN });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (ClonePlan - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultId, success = false, });
            }
        }
        /// <summary>
        /// This method is used to create plan from the paln detail form fields.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult CreatePlan(string id, string poId, string ppId, string ppN, string poN)
        {
            return ViewComponent("AddPlan", new { id, poId, ppId, ppN, poN });
        }
        /// <summary>
        /// This method isused to post the plan detail under the program.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostPlan(PlanModel model)
        {
            string resultId = "0";
            try
            {
                if (model.selectedProgramAccount.Any())
                {
                    model.planProgramAccount = new List<PlanProgramAccountModel>();
                    foreach (var item in model.selectedProgramAccount)
                    {
                        PlanProgramAccountModel objplanprogacc = new PlanProgramAccountModel();
                        objplanprogacc.planId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.id));
                        objplanprogacc.programAccountId = item;
                        model.planProgramAccount.Add(objplanprogacc);
                    }
                }

                var dataPlan = new apiModel.PlanViewModel()
                {
                    clientId = model.clientId,
                    description = !string.IsNullOrEmpty(model.description) ? model.description.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>") : null,
                    endDate = model.endDate,
                    endTime = model.endTime,
                    noOfFlexPoints = model.noOfFlexPoints,
                    noOfMealPasses = model.noOfMealPasses,
                    PlanProgramAccount = model.planProgramAccount,
                    planId = model.planId,
                    startDate = model.startDate,
                    id = !string.IsNullOrEmpty(model.id.ToString()) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.id.ToString())) : 0,
                    name = model.name,
                    programId = !string.IsNullOrEmpty(model.programId.ToString()) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.programId.ToString())) : 0,
                    startTime = model.startTime,
                    createdBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value),
                    modifiedBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value),
                    Jpos_PlanId = !string.IsNullOrEmpty(model.Jpos_PlanEncId) ? Cryptography.DecryptCipherToPlain(model.Jpos_PlanEncId) : ""
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataPlan);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdatePlanInfo, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToString(response.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostPlan - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultId });
        }
        #endregion

        #region Program Accounts
        /// <summary>
        /// This method is used to show the program account list
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult ProgramAccountList(string poId, string ppId, string ppN, string poN)
        {
            return ViewComponent("AccountList", new { ppId, poId, ppN, poN });
        }
        /// <summary>
        /// This method is used load program acccount data list.
        /// </summary>
        /// <param name="ppId"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadProgramAccountData(string ppId)
        {
            var drawPrgAccData = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                var primaryPrgIdPrgAccData = Cryptography.DecryptCipherToPlain(ppId);

                // Skip number of Rows count  
                var startPrgAccData = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var lengthPrgAccData = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumnPrgAccData = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirectionPrgAccData = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValuePrgAccData = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSizePrgAccData = lengthPrgAccData != null ? Convert.ToInt32(lengthPrgAccData) : 0;

                int skipPrgAccData = startPrgAccData != null ? Convert.ToInt32(startPrgAccData) : 0;

                int recordsTotalPrgAccData = 0;
                List<AccountListingDto> progAcc = new List<AccountListingDto>();
                using (var clientPrgAcc = new HttpClient())
                {
                    clientPrgAcc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgAcc.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultPrgAcc = clientPrgAcc.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramAccountListing + "?programId=" + primaryPrgIdPrgAccData).Result;
                    if (resultPrgAcc.IsSuccessStatusCode)
                    {
                        var responsePrgAcc = await resultPrgAcc.Content.ReadAsAsync<apiModel.ApiResponse>();
                        dynamic responseDesPrgAcc = JsonConvert.DeserializeObject(responsePrgAcc.Result.ToString());
                        progAcc = responseDesPrgAcc.ToObject<List<AccountListingDto>>();
                        progAcc.ForEach(x => { x.StrId = Cryptography.EncryptPlainToCipher(x.Id.ToString()); x.Jpos_ProgramEncAccountId = !string.IsNullOrEmpty(x.Jpos_ProgramAccountId) ? Cryptography.EncryptPlainToCipher(x.Jpos_ProgramAccountId) : ""; });
                    }
                }
                var proAccData = (from tempdata in progAcc
                                  select tempdata);
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumnPrgAccData) && string.IsNullOrEmpty(sortColumnDirectionPrgAccData)))
                {
                    proAccData = proAccData.OrderBy(sortColumnPrgAccData + " " + sortColumnDirectionPrgAccData);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValuePrgAccData))
                {
                    proAccData = proAccData.Where(m => m.AccountName.ToLower().Trim().Contains(searchValuePrgAccData.ToLower().Trim())
                    || m.AccountType.ToLower().Trim().Contains(searchValuePrgAccData.ToLower().Trim())
                    || m.ProgramAccountId.ToLower().Trim().Contains(searchValuePrgAccData.ToLower().Trim())
                    || m.PlanName.ToLower().Trim().Contains(searchValuePrgAccData.ToLower().Trim()));
                }

                //total number of rows counts   
                recordsTotalPrgAccData = proAccData.Count();
                //Paging   
                var dataPrgAcc = proAccData.Skip(skipPrgAccData).Take(pageSizePrgAccData).ToList();
                //Returning Json Data  
                return Json(new { draw = drawPrgAccData, recordsFiltered = recordsTotalPrgAccData, recordsTotal = recordsTotalPrgAccData, data = dataPrgAcc });

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadProgramAccountData - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawPrgAccData, recordsFiltered = 0, recordsTotal = 0, data = new List<AccountListingDto>() });
            }

        }
        /// <summary>
        /// This method is used to change account status from datatable.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeAccountStatus(int Id, bool isActive)
        {
            int resultIdChngAccountStatus = 0;
            try
            {
                AccountListingDto adminChangeStatus = new AccountListingDto()
                {
                    Id = Id,
                    Status = isActive
                };
                using (var clientChangeStatus = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(adminChangeStatus);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientChangeStatus.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientChangeStatus.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultChangeStatus = await clientChangeStatus.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UpdateProgramAccountStatus, stringContent);
                    if (resultChangeStatus.IsSuccessStatusCode)
                    {
                        var responseChangeStatus = await resultChangeStatus.Content.ReadAsAsync<ApiResponse>();
                        if (responseChangeStatus.StatusFlagNum == 1)
                        {
                            resultIdChngAccountStatus = Convert.ToInt32(responseChangeStatus.Result);
                        }
                    }
                }
                return Json(new { data = resultIdChngAccountStatus, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (ChangeAccountStatus - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdChngAccountStatus, success = false });
            }
        }
        /// <summary>
        /// This method is used to delete account.
        /// </summary>
        /// <param name="ID">ID for the program account to delete</param>
        /// <param name="Jpos_PrgAccountId">jpos id of the program id to delete</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string ID, string Jpos_PrgAccountId)
        {
            int resultIdDelAcc = 0;
            try
            {
                using (var clientDelAcc = new HttpClient())
                {
                    var dtoDelAcc = new AccountListingDto()
                    {
                        Id = !string.IsNullOrEmpty(ID) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(ID)) : 0,
                        Jpos_ProgramAccountId = !string.IsNullOrEmpty(Jpos_PrgAccountId) ? Cryptography.DecryptCipherToPlain(Jpos_PrgAccountId) : "",
                    };
                    string jsonDelAcc = Newtonsoft.Json.JsonConvert.SerializeObject(dtoDelAcc);

                    HttpContent stringContentDelAcc = new StringContent(jsonDelAcc);
                    stringContentDelAcc.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelAcc.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultdataDelAcc = await clientDelAcc.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteProgramAccount, stringContentDelAcc);
                    if (resultdataDelAcc.IsSuccessStatusCode)
                    {
                        var responseDelAcc = await resultdataDelAcc.Content.ReadAsAsync<apiModel.ApiResponse>();
                        if (responseDelAcc.StatusFlagNum == 1)
                        {
                            resultIdDelAcc = Convert.ToInt32(responseDelAcc.Result);
                        }
                    }
                }
                return Json(new { draw = resultIdDelAcc, success = true, message = "" });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (DeleteAccount - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = resultIdDelAcc, success = false, message = "" });
            }
        }
        /// <summary>
        /// This method is used to clone account from datatable account list.
        /// </summary>
        /// <param name="Id">Id of the program account to clone</param>
        /// <param name="poId">organization under which account is created</param>
        /// <param name="ppId">program id under which account is created</param>
        /// <param name="ppN">program name under which account is created</param>
        /// <returns>id of the cloned account</returns>
        [HttpPost]
        public async Task<IActionResult> CloneAccount(string Id, string poId, string ppId, string ppN)
        {
            int resultId = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    var dto = new AccountListingDto()
                    {
                        Id = !string.IsNullOrEmpty(Id) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(Id)) : 0,
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dto);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.CloneProgramAccount, stringContent);
                }
                return RedirectToAction("ProgramAccountList", new { poId, ppId, ppN });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (CloneAccount - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultId, success = false, });
            }
        }
        /// <summary>
        /// This method is used to create program account.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <returns></returns>
        public IActionResult CreateProgramAccount(string id, string poId, string ppId, string ppN, string poN)
        {
            return ViewComponent("AddAccount", new { id, poId, ppId, ppN, poN });
        }
        /// <summary>
        /// This method is used to create account merchant rules.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <param name="btId"></param>
        /// <param name="atId"></param>
        /// <param name="aId"></param>
        /// <returns></returns>
        public IActionResult CreateProgramMerchantAccountRules(string id, string poId, string ppId, string ppN, string poN, string btIdNatId, string aId)
        {
            return ViewComponent("AddAccountMerchantRules", new { id, poId, ppId, ppN, poN, btIdNatId, aId });
        }
        /// <summary>
        /// This method is used to post program account.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostProgramAccount(ProgramAccountModel model)
        {
            string resultId = "0";
            try
            {
                var dataProgramAccount = new apiModel.ProgramAccountViewModel()
                {
                    accountName = model.accountName,
                    accountTypeId = model.accountTypeId,
                    exchangePassValue = model.exchangePassValue,
                    exchangeResetDateOfMonth = model.exchangeResetDateOfMonth,
                    exchangeResetDay = model.exchangeResetDay,
                    exchangeResetPeriodType = model.exchangeResetPeriodType,
                    exchangeResetTime = model.exchangeResetTime,
                    flexEndDate = model.flexEndDate,
                    flexMaxSpendPerDay = model.flexMaxSpendPerDay,
                    flexMaxSpendPerMonth = model.flexMaxSpendPerMonth,
                    flexMaxSpendPerWeek = model.flexMaxSpendPerWeek,
                    intialBalanceCount = model.intialBalanceCount,
                    isPassExchangeEnabled = model.isPassExchangeEnabled,
                    isRollOver = model.isRollOver,
                    maxPassPerMonth = model.maxPassPerMonth,
                    maxPassPerWeek = model.maxPassPerWeek,
                    maxPassUsage = model.maxPassUsage,
                    passType = model.passType,
                    ProgramAccountId = model.ProgramAccountId,
                    resetDateOfMonth = model.resetDateOfMonth,
                    resetDay = model.resetDay,
                    resetPeriodType = model.resetPeriodType,
                    resetTime = model.resetTime,
                    vplMaxAddValueAmount = model.vplMaxAddValueAmount,
                    vplMaxBalance = model.vplMaxBalance,
                    vplMaxNumberOfTransaction = model.vplMaxNumberOfTransaction,
                    id = (!string.IsNullOrEmpty(model.id.ToString()) && model.id != "0") ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.id.ToString())) : 0,
                    programId = !string.IsNullOrEmpty(model.programId.ToString()) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.programId.ToString())) : 0,
                    Jpos_ProgramAccountId = !string.IsNullOrEmpty(model.Jpos_ProgramAccountEncId) ? Cryptography.DecryptCipherToPlain(model.Jpos_ProgramAccountEncId) : ""
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataProgramAccount);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateProgramAccountInfo, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
                        resultId = Convert.ToString(response.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostProgramAccount - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = resultId });
        }
        /// <summary>
        /// This method is used to post selected merchant for account merchant rules.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostSelectedMerchant(AccountMerchantRuleModel model)
        {
            string resultId = "0";
            try
            {
                foreach (var item in model.Merchants)
                {
                    model.AccountMerchantRuleAndDetail = new List<AccountMerchantRuleAndDetailModel>();
                    for (int i = 0; i < 4; i++)
                    {
                        AccountMerchantRuleAndDetailModel objamrd = new AccountMerchantRuleAndDetailModel();
                        objamrd.maxPassUsage = 0;
                        objamrd.maxPassValue = 0;
                        objamrd.mealPeriodId = i + 1;
                        objamrd.minPassValue = 0;
                        objamrd.transactionLimit = 0;
                        model.AccountMerchantRuleAndDetail.Add(objamrd);
                    }
                }
                var dataProgramAccount = new apiModel.AccountMerchantRuleViewModel()
                {
                    accountTypeId = model.accountTypeId,
                    programAccountId = !string.IsNullOrEmpty(model.programAccountId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.programAccountId)) : 0,
                    programId = !string.IsNullOrEmpty(model.programId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.programId)) : 0,
                    selectedMerchant = model.Merchants,
                    accountMerchantRuleAndDetails = _mapper.Map<List<AccountMerchantRuleAndDetailViewModel>>(model.AccountMerchantRuleAndDetail)
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataProgramAccount);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.CreateModifyAccountMerchantRules, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToString(response.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostSelectedMerchant - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultId });
        }
        /// <summary>
        /// This method is used to post selected merchant rules for account.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> PostSelectedMerchantRules(AccountMerchantRuleModel model)
        {
            string resultId = "0";

            try
            {

                var dataProgramAccount = _mapper.Map<List<AccountMerchantRuleAndDetailViewModel>>(model.AccountMerchantRuleAndDetail);

                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataProgramAccount);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ModifyAccountMerchantRuleDetails, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
                        if (response.StatusFlagNum == 1)
                        {
                            resultId = Convert.ToString(response.Result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostSelectedMerchant - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultId });
        }
        #endregion

        #region Account Holders
        /// <summary>
        /// This method is used to load account holders.
        /// </summary>
        /// <param name="prgId"></param>
        /// <param name="orgId"></param>
        /// <param name="planId"></param>
        /// <param name="currentPageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoadAccountHolders(string prgId, string orgId, string planId, int currentPageNumber)
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

                    var resultACH = clientACH.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAccountHolders + "?organisationId=" + organisationIdCHA + "&programId=" + programIdCHA + "&searchValue=" + searchValueACH + "&pageNumber=" + pageNumberCHA + "&pageSize=" + pageSizeACH + "&sortColumnName=" + sortColumnACH + "&sortOrderDirection=" + sortColumnDirectionACH + "&planId=" + (!string.IsNullOrEmpty(planId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(planId)) : 0)).Result;
                    if (resultACH.IsSuccessStatusCode)
                    {
                        var responseACH = await resultACH.Content.ReadAsAsync<ApiResponse>();
                        if (responseACH.StatusFlagNum == 1)
                        {
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseACH.Result.ToString());
                            userAccountHolder = responseDesACH.ToObject<List<AccountHolderDto>>();
                            userAccountHolder.ForEach(x => { x.UserEncId = Cryptography.EncryptPlainToCipher(x.Id.ToString()); x.Jpos_AccountEncId = !string.IsNullOrEmpty(x.Jpos_AccountHolderId) ? Cryptography.EncryptPlainToCipher(x.Jpos_AccountHolderId) : ""; });
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
                recordsTotalACH = userAccountHolder.Count > 0 ? userAccountHolder.FirstOrDefault().TotalCount : 0;


                return Json(new { draw = drawACH, recordsFiltered = recordsTotalACH, recordsTotal = recordsTotalACH, data = userAccountHolderData });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAccountHolders - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { draw = drawACH, recordsFiltered = 0, recordsTotal = 0, data = new List<AccountHolderDto>() });
            }
        }
        /// <summary>
        /// This method is used to delete account holder.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="prgId"></param>
        /// <param name="jpos_AccountHolderId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAccountHolder(string ID, string prgId, string jpos_AccountHolderId)
        {
            int resultIdDelAch = 0;
            try
            {
                using (var clientDelACH = new HttpClient())
                {
                    var orgDtoDelACH = new AccountHolderDto()
                    {
                        Id = !string.IsNullOrEmpty(ID) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(ID)) : 0,
                        Jpos_AccountEncId = jpos_AccountHolderId
                    };
                    string jsonDelACH = Newtonsoft.Json.JsonConvert.SerializeObject(orgDtoDelACH);

                    HttpContent stringContentDelACH = new StringContent(jsonDelACH.ToString());
                    stringContentDelACH.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelACH.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultDelACH = await clientDelACH.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteAccountHolder, stringContentDelACH);
                    if (resultDelACH.IsSuccessStatusCode)
                    {
                        var responseDelACH = await resultDelACH.Content.ReadAsAsync<ApiResponse>();
                        resultIdDelAch = Convert.ToInt32(responseDelACH.Result);
                    }
                }
                return Json(new { data = resultIdDelAch, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (DeleteAccountHolder - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdDelAch, success = false });
            }


        }
        /// <summary>
        /// This method is used to invite account holder.
        /// </summary>
        /// <param name="encIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InviteAccountHolder(List<string> encIds)
        {
            int resultIdInvACH = 0;
            try
            {
                using (var clientInvACH = new HttpClient())
                {
                    List<int> userIds = new List<int>();
                    foreach (var item in encIds)
                    {
                        userIds.Add(Convert.ToInt32(Cryptography.DecryptCipherToPlain(item)));
                    }
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(userIds);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientInvACH.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultInvACH = await clientInvACH.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.SendMagicLinkInEmail, stringContent);
                    if (resultInvACH.IsSuccessStatusCode)
                    {
                        var responseInvACH = await resultInvACH.Content.ReadAsAsync<ApiResponse>();
                        resultIdInvACH = Convert.ToInt32(responseInvACH.Result);
                    }
                }
                return Json(new { data = resultIdInvACH, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (InviteAccountHolder - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdInvACH, success = false });
            }
        }
        /// <summary>
        /// This method is used to post account holder information.
        /// </summary>
        /// <param name="model">Post data for account holder</param>
        /// <returns>Id of the account holder after creation.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAccountHolderInformation(AccountHolderModel model)
        {
            string resultId = "0";
            bool success = false;
            string message = "";
            if (ModelState.IsValid)
            {
                try
                {
                    bool? IsMobileRegistered;
                    int? InvitationStatus;
                    PostAccountHolderCheckForMobileNInvitation(model, out IsMobileRegistered, out InvitationStatus);
                    RegisterAccountHolderModel dataAccountHolderDetail = BindModalForAccountHolder(model, IsMobileRegistered, InvitationStatus);
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage result = await RegisterAccountHolder(dataAccountHolderDetail, client);
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<ApiResponse>();
                            success = Convert.ToBoolean(response.StatusFlagNum);
                            message = response.Message;
                            resultId = Convert.ToString(response.Result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostAccountHolderInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId, success = success, message = message });
        }

        private async Task<HttpResponseMessage> RegisterAccountHolder(RegisterAccountHolderModel dataAccountHolderDetail, HttpClient client)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataAccountHolderDetail);
            HttpContent stringContent = new StringContent(json.ToString());
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.RegisterAccountHolder, stringContent);
            return result;
        }

        private RegisterAccountHolderModel BindModalForAccountHolder(AccountHolderModel model, bool? IsMobileRegistered, int? InvitationStatus)
        {
            return new RegisterAccountHolderModel()
            {
                CustomFields = model.UserCustomJsonValue,
                AccountHolderUniqueId = model.AccountHolderID,
                DateOfBirth = model.DateOfBirth,
                FirstName = model.FirstName,
                GenderId = model.GenderId,
                CreatedBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value),
                Id = !string.IsNullOrEmpty(model.UserEncId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.UserEncId)) : 0,
                LastName = model.LastName,
                ModifiedBy = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value),
                OrganizationId = !string.IsNullOrEmpty(model.OrgEncId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.OrgEncId)) : 0,
                PhoneNumber = model.PhoneNumber,
                PlanIds = model.SelectedPlanIds,
                PrimaryEmail = model.Email,
                ProgramId = !string.IsNullOrEmpty(model.ProgEncId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.ProgEncId)) : 0,
                SecondaryEmail = model.SecondaryEmail,
                userEncId = model.UserEncId,
                UserImagePath = model.UserImagePath,
                IsMobileRegistered = IsMobileRegistered,
                InvitationStatus = InvitationStatus,
                Jpos_AccountHolderId = !string.IsNullOrEmpty(model.Jpos_EncUserID) ? Cryptography.DecryptCipherToPlain(model.Jpos_EncUserID) : ""
            };
        }

        private static void PostAccountHolderCheckForMobileNInvitation(AccountHolderModel model, out bool? IsMobileRegistered, out int? InvitationStatus)
        {
            IsMobileRegistered = null;
            InvitationStatus = null;
            if (!string.IsNullOrEmpty(model.UserEncId) && Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.UserEncId)) <= 0)
            {
                IsMobileRegistered = false;
                InvitationStatus = 1;
            }
        }
        #endregion

        #region Branding
        /// <summary>
        /// This method is used to create branding for the selected program and account.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <param name="bId"></param>
        /// <returns></returns>
        public IActionResult CreateBranding(string id, string poId, string ppId, string ppN, string poN, string bId)
        {
            return ViewComponent("AddBranding", new { id, poId, ppId, ppN, poN, bId });
        }
        /// <summary>
        /// This method is used to get the branding listing for the program.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <param name="bId"></param>
        /// <returns></returns>
        public IActionResult BrandingListing(string id, string poId, string ppId, string ppN, string poN, string bId)
        {
            return ViewComponent("BrandingList", new { id, poId, ppId, ppN, poN, bId });
        }
        /// <summary>
        /// This method is used to get the loyality override settings for the program.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="ppN"></param>
        /// <param name="poN"></param>
        /// <param name="bId"></param>
        /// <returns></returns>
        public IActionResult SiteOverrideSettingListing(string id, string poId, string ppId, string ppN, string poN, string bId)
        {
            return ViewComponent("SiteLevelOverrideSetting", new { id, poId, ppId, ppN, poN, bId });
        }
        /// <summary>
        /// This method is used to get the program account type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProgramAccountType(string id)
        {
            var BrandingAccountType = new BrandingDetailsWithAccountType();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetProgramAccountTypeById + "?id=" + id).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<ApiResponse>();
                        if (response.Result != null)
                        {
                            BrandingAccountType = JsonConvert.DeserializeObject<BrandingDetailsWithAccountType>(response.Result.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (GetProgramAccountType - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return Json(new { data = BrandingAccountType });
        }
        /// <summary>
        /// This method is used to post branding detail.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostBrandingDetails(ProgramBrandingModel model)
        {
            string resultId = "0";
            string message = string.Empty;

            try
            {
                var data = new apiModel.ProgramBrandingViewModel()
                {
                    accountName = model.accountName,
                    accountTypeId = model.accountTypeId.Value,
                    brandingColor = model.brandingColor,
                    programAccountID = model.programAccountID,
                    id = model.id,
                    programId = !string.IsNullOrEmpty(model.programId.ToString()) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.programId.ToString())) : 0,
                    cardNumber = model.cardNumber,
                    ImagePath = model.ImagePath
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateBrandingInfo, stringContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<apiModel.ApiResponse>();
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
        /// <summary>
        /// This method is used to delete program branding.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteBranding(string Id)
        {
            int resultIdDelBrand = 0;
            try
            {
                using (var clientDelBrand = new HttpClient())
                {
                    var brandDtoDel = new BrandingListingDto()
                    {
                        Id = Convert.ToInt32(Id)
                    };
                    string jsonDelBrand = Newtonsoft.Json.JsonConvert.SerializeObject(brandDtoDel);

                    HttpContent stringContentDelBrand = new StringContent(jsonDelBrand);
                    stringContentDelBrand.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    clientDelBrand.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultDelBrand = await clientDelBrand.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteBranding, stringContentDelBrand);
                    if (resultDelBrand.IsSuccessStatusCode)
                    {
                        var responseDelBrand = await resultDelBrand.Content.ReadAsAsync<ApiResponse>();
                        resultIdDelBrand = Convert.ToInt32(responseDelBrand.Result);
                    }
                }
                return Json(new { data = resultIdDelBrand, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (DeleteBranding - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultIdDelBrand, success = false });
            }
        }
        #endregion

        #region Cardholder Agreement
        /// <summary>
        /// This method is used to get card holder agreement by Id.
        /// </summary>
        /// <param name="programIdEnc"></param>
        /// <param name="cardHolderAgreementIdEnc"></param>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full")]
        [HttpGet]
        public async Task<IActionResult> GetCardHolderAgreementById(string programIdEnc, string cardHolderAgreementIdEnc)
        {
            CardholderAgreementDto cardholderAgreement = new CardholderAgreementDto();
            using (var clientCHAId = new HttpClient())
            {
                clientCHAId.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientCHAId.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                int programIdCHAId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(programIdEnc));
                int cardHolderAgreementId;
                if (!string.IsNullOrEmpty(cardHolderAgreementIdEnc))
                    cardHolderAgreementId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(cardHolderAgreementIdEnc));
                else
                    cardHolderAgreementId = 0;
                var resultCHAId = clientCHAId.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CardholderAgreementById + "?programId=" + programIdCHAId + "&cardholderAgreementId=" + cardHolderAgreementId).Result;

                if (resultCHAId.IsSuccessStatusCode)
                {
                    var responseCHAId = await resultCHAId.Content.ReadAsAsync<ApiResponse>();
                    dynamic responseDesCHAId = JsonConvert.DeserializeObject(responseCHAId.Result.ToString());
                    cardholderAgreement = responseDesCHAId.ToObject<CardholderAgreementDto>();
                }
            }
            return Json(new { data = cardholderAgreement });
        }
        /// <summary>
        /// This method is used to get the user agreement history versions.
        /// </summary>
        /// <param name="programIdEnc"></param>
        /// <param name="cardHolderAgreementIdEnc"></param>
        /// <returns></returns>
        [Authorize(Roles = "Super Admin, Organization Full, Program Full")]
        [HttpGet]
        public async Task<IActionResult> GetUserAgreementHistoryVersions(string programIdEnc, string cardHolderAgreementIdEnc)
        {
            List<UserAgreementHistoryDto> cardholderAgreementVersion = new List<UserAgreementHistoryDto>();
            using (var clientCHAVer = new HttpClient())
            {
                clientCHAVer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientCHAVer.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                int programIdCHAVer = Convert.ToInt32(Cryptography.DecryptCipherToPlain(programIdEnc));
                int cardHolderAgreementIdCHAVer;
                if (!string.IsNullOrEmpty(cardHolderAgreementIdEnc))
                    cardHolderAgreementIdCHAVer = Convert.ToInt32(Cryptography.DecryptCipherToPlain(cardHolderAgreementIdEnc));
                else
                    cardHolderAgreementIdCHAVer = 0;
                var resultCHAVer = clientCHAVer.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UserAgreementHistoryVersions + "?programId=" + programIdCHAVer + "&cardholderAgreementId=" + cardHolderAgreementIdCHAVer).Result;
                if (resultCHAVer.IsSuccessStatusCode)
                {
                    var responseCHAVer = await resultCHAVer.Content.ReadAsAsync<ApiResponse>();
                    dynamic responseDesCHAVer = JsonConvert.DeserializeObject(responseCHAVer.Result.ToString());
                    cardholderAgreementVersion = responseDesCHAVer.ToObject<List<UserAgreementHistoryDto>>();
                    cardholderAgreementVersion.ForEach(x => x.DateAcceptedString = String.Format(x.DateAccepted.ToString("MMMM - dd{0} - yyyy"), GetSuffix(x.DateAccepted.Day.ToString())).Trim());
                }
            }
            return Json(new { data = cardholderAgreementVersion });
        }
        /// <summary>
        /// This method is used to post card holder agreement detail information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostCardHolderAgreementDetailInformation(CardHolderAgreeementModel model)
        {
            string resultIdCHAPost = "0";
            string messageCHAPost = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var dataCardHolderAgreementDetail = new CardholderAgreementDto
                    {
                        CardHolderAgreementId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.CardHolderAgreementId)),
                        cardHoldrAgreementContent = model.CardHolderAgreementContent,
                        ProgramId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(model.ProgramIdEnc)),
                        versionNo = model.VersionNo
                    };
                    using (var clientCHAPost = new HttpClient())
                    {
                        string jsonCHAPost = Newtonsoft.Json.JsonConvert.SerializeObject(dataCardHolderAgreementDetail);
                        HttpContent stringContentCHAPost = new StringContent(jsonCHAPost);
                        stringContentCHAPost.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        clientCHAPost.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        clientCHAPost.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                        var resultCHAPost = await clientCHAPost.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddCardHolderAgreement, stringContentCHAPost);
                        if (resultCHAPost.IsSuccessStatusCode)
                        {
                            var responseCHAPost = await resultCHAPost.Content.ReadAsAsync<ApiResponse>();
                            resultIdCHAPost = Convert.ToString(responseCHAPost.Result);
                            messageCHAPost = Convert.ToString(responseCHAPost.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostCardHolderAgreementDetailInformation - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultIdCHAPost, messageCHAPost });
        }


        #endregion

        #region Private Method
        /// <summary>
        /// This method is used to get the suffix of the date in nth term.
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private string GetSuffix(string day)
        {
            string suffix = "th";

            if (int.Parse(day) < 11 || int.Parse(day) > 20)
            {
                var dayArray = day.ToCharArray();
                day = dayArray[dayArray.Length - 1].ToString();
                switch (day)
                {
                    case "1":
                        suffix = "st";
                        break;
                    case "2":
                        suffix = "nd";
                        break;
                    case "3":
                        suffix = "rd";
                        break;
                }
            }

            return suffix;
        }
        #endregion

        #region Export Excel

        #region  Export Organisation Program
        /// <summary>
        /// This method is used to export the organisation program excel sheet.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrganisationProgramExportExcel(string searchValue)
        {
            var fileNameOrgPrgmExport = "Program List.xlsx";

            //Save the file to server temp folder
            List<ProgramListDto> organisationProgramsExport = new List<ProgramListDto>();
            using (var clientOrgPrgExport = new HttpClient())
            {
                clientOrgPrgExport.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientOrgPrgExport.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var resultOrgPrgExport = clientOrgPrgExport.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllProgramsList + "?isActive=" + true + "&isDeleted=" + false).Result;
                if (resultOrgPrgExport.IsSuccessStatusCode)
                {
                    var responseOrgPrgExport = await resultOrgPrgExport.Content.ReadAsAsync<ApiResponse>();
                    if (responseOrgPrgExport.StatusFlagNum == 1)
                    {
                        dynamic responseDesOrgPrg = JsonConvert.DeserializeObject(responseOrgPrgExport.Result.ToString());
                        organisationProgramsExport = responseDesOrgPrg.ToObject<List<ProgramListDto>>();
                        organisationProgramsExport.ForEach(x => { x.strProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()); x.EncProgName = Cryptography.EncryptPlainToCipher(x.ProgramName); x.OrganisationEncId = Cryptography.EncryptPlainToCipher(x.OrganisationId.ToString()); x.EncOrganisationName = Cryptography.EncryptPlainToCipher(x.OrganisationName.ToString()); });
                    }
                }
            }
            // Getting all Organisation Programs data
            var orgProgramDataExport = (from tempOrgPrg in organisationProgramsExport select tempOrgPrg).AsEnumerable();
            //Sorting
            var sortColumnOrgPrgExport = "DateAdded";
            var sortColumnDirectionOrgPrgExport = "desc";
            if (!(string.IsNullOrEmpty(sortColumnOrgPrgExport) && string.IsNullOrEmpty(sortColumnDirectionOrgPrgExport)))
            {
                orgProgramDataExport = orgProgramDataExport.OrderBy(sortColumnOrgPrgExport + " " + sortColumnDirectionOrgPrgExport);
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                orgProgramDataExport = orgProgramDataExport.Where(o => o.ProgramName.ToLower().Trim().Contains(searchValue.ToLower().Trim()) || o.ProgramCodeId.ToLower().Trim().Contains(searchValue.ToLower().Trim()) || o.ProgramType.ToLower().Trim().Contains(searchValue.ToLower().Trim()) || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), GetSuffix(o.DateAdded.Day.ToString())).ToLower().Trim().Contains(searchValue.ToLower().Trim()) || (o.OrganisationSubTitle != null && o.OrganisationSubTitle.ToLower().Trim().Contains(searchValue.ToLower().Trim()))).AsEnumerable();
            }
            ExcelPackage excelOrgPrgExport = new ExcelPackage();
            var workSheetOrgPrgExport = excelOrgPrgExport.Workbook.Worksheets.Add("Sheet1");
            workSheetOrgPrgExport.TabColor = System.Drawing.Color.Black;
            workSheetOrgPrgExport.DefaultRowHeight = 12;
            //Header of table  
            workSheetOrgPrgExport.Row(1).Height = 20;
            workSheetOrgPrgExport.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetOrgPrgExport.Row(1).Style.Font.Bold = true;
            workSheetOrgPrgExport.Cells[1, 1].Value = "Program Name";
            workSheetOrgPrgExport.Cells[1, 2].Value = "Program Id";
            workSheetOrgPrgExport.Cells[1, 3].Value = "Date Added";
            workSheetOrgPrgExport.Cells[1, 4].Value = "Program Types";
            workSheetOrgPrgExport.Cells[1, 5].Value = "Organization";
            //Body of table  

            int recordIndex = 2;
            foreach (var prog in orgProgramDataExport)
            {
                workSheetOrgPrgExport.Cells[recordIndex, 1].Value = prog.ProgramName;
                workSheetOrgPrgExport.Cells[recordIndex, 2].Value = prog.ProgramCodeId;
                workSheetOrgPrgExport.Cells[recordIndex, 3].Value = prog.DateAdded.ToShortDateString();
                workSheetOrgPrgExport.Cells[recordIndex, 4].Value = prog.ProgramType;
                workSheetOrgPrgExport.Cells[recordIndex, 5].Value = prog.OrganisationSubTitle;
                recordIndex++;
            }
            workSheetOrgPrgExport.Column(1).AutoFit();
            workSheetOrgPrgExport.Column(2).AutoFit();
            workSheetOrgPrgExport.Column(3).AutoFit();
            workSheetOrgPrgExport.Column(4).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelOrgPrgExport.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameOrgPrgmExport });
        }
        /// <summary>
        /// This method is used to export admin program excel sheet.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AdminProgramExportExcel(string searchValue)
        {
            var fileNameAdminProgramExport = "Program List.xlsx";
            //Save the file to server temp folder
            List<ProgramListDto> adminPrograms = new List<ProgramListDto>();
            using (var clientAdminPrograms = new HttpClient())
            {
                clientAdminPrograms.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientAdminPrograms.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var resultAdminProgram = clientAdminPrograms.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllProgramsForPrgAdminList + "?isActive=" + true + "&isDeleted=" + false).Result;
                if (resultAdminProgram.IsSuccessStatusCode)
                {
                    var responseAdminPrg = await resultAdminProgram.Content.ReadAsAsync<ApiResponse>();
                    if (responseAdminPrg.StatusFlagNum == 1)
                    {
                        dynamic responseDesAdminPrg = JsonConvert.DeserializeObject(responseAdminPrg.Result.ToString());
                        adminPrograms = responseDesAdminPrg.ToObject<List<ProgramListDto>>();
                        adminPrograms.ForEach(x => { x.strProgramId = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()); x.EncProgName = Cryptography.EncryptPlainToCipher(x.ProgramName); x.OrganisationEncId = Cryptography.EncryptPlainToCipher(x.OrganisationId.ToString()); x.EncOrganisationName = Cryptography.EncryptPlainToCipher(x.OrganisationName.ToString()); });
                    }
                }
            }
            // Getting all Organisation Programs data
            var adminProgramData = (from tempDataAdminPrg in adminPrograms select tempDataAdminPrg).AsEnumerable();
            //Sorting
            var sortColumnAdminPrg = "DateAdded";
            var sortColumnDirectionAdminPrg = "desc";
            if (!(string.IsNullOrEmpty(sortColumnAdminPrg) && string.IsNullOrEmpty(sortColumnDirectionAdminPrg)))
            {
                adminProgramData = adminProgramData.OrderBy(sortColumnAdminPrg + " " + sortColumnDirectionAdminPrg);
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                adminProgramData = adminProgramData.Where(o => o.ProgramName.ToLower().Trim().Contains(searchValue.ToLower().Trim()) || o.ProgramCodeId.ToLower().Trim().Contains(searchValue.ToLower().Trim()) || o.ProgramType.ToLower().Trim().Contains(searchValue.ToLower().Trim()) || String.Format(o.DateAdded.ToString("MMMM - dd{0} - yyyy"), GetSuffix(o.DateAdded.Day.ToString())).ToLower().Trim().Contains(searchValue.ToLower().Trim()) || (o.OrganisationSubTitle != null && o.OrganisationSubTitle.ToLower().Trim().Contains(searchValue.ToLower().Trim()))).AsEnumerable();
            }
            ExcelPackage excelAdminPrg = new ExcelPackage();
            var workSheetAdminPrg = excelAdminPrg.Workbook.Worksheets.Add("Sheet1");
            workSheetAdminPrg.TabColor = System.Drawing.Color.Black;
            workSheetAdminPrg.DefaultRowHeight = 12;
            //Header of table  
            workSheetAdminPrg.Row(1).Height = 20;
            workSheetAdminPrg.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAdminPrg.Row(1).Style.Font.Bold = true;
            workSheetAdminPrg.Cells[1, 1].Value = "Program Name";
            workSheetAdminPrg.Cells[1, 2].Value = "Program Id";
            workSheetAdminPrg.Cells[1, 3].Value = "Date Added";
            workSheetAdminPrg.Cells[1, 4].Value = "Program Types";
            workSheetAdminPrg.Cells[1, 5].Value = "Organization";
            //Body of table  

            int recordIndex = 2;
            foreach (var prog in adminProgramData)
            {
                workSheetAdminPrg.Cells[recordIndex, 1].Value = prog.ProgramName;
                workSheetAdminPrg.Cells[recordIndex, 2].Value = prog.ProgramCodeId;
                workSheetAdminPrg.Cells[recordIndex, 3].Value = prog.DateAdded.ToShortDateString();
                workSheetAdminPrg.Cells[recordIndex, 4].Value = prog.ProgramType;
                workSheetAdminPrg.Cells[recordIndex, 5].Value = prog.OrganisationName;
                recordIndex++;
            }
            workSheetAdminPrg.Column(1).AutoFit();
            workSheetAdminPrg.Column(2).AutoFit();
            workSheetAdminPrg.Column(3).AutoFit();
            workSheetAdminPrg.Column(4).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelAdminPrg.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameAdminProgramExport });
        }
        #endregion

        #region Export Merchant Listing
        /// <summary>
        /// This method is used to export admin merchant in excel.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="ppId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AdminMerchantExportExcel(string searchValue, string ppId)
        {
            var userIAdminMerchant = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

            var fileNameAdminMerchant = "Merchant List.xlsx";
            List<MerchantDto> adminMerchant = new List<MerchantDto>();
            using (var clientAdminMerchant = new HttpClient())
            {
                clientAdminMerchant.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientAdminMerchant.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var resultAdminMerchant = clientAdminMerchant.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAllMerchantsByAdmin + "?userId=" + userIAdminMerchant).Result;
                if (resultAdminMerchant.IsSuccessStatusCode)
                {
                    var responseAdminMerchant = await resultAdminMerchant.Content.ReadAsAsync<apiModel.ApiResponse>();
                    dynamic responseDescAdminMerchant = JsonConvert.DeserializeObject(responseAdminMerchant.Result.ToString());
                    adminMerchant = responseDescAdminMerchant.ToObject<List<MerchantDto>>();

                    adminMerchant.ForEach(x => x.Id = Cryptography.EncryptPlainToCipher(x.Id.ToString()));
                    adminMerchant.ForEach(x => x.AccountType = !string.IsNullOrEmpty(x.AccountType) ? x.AccountType.Replace("lt;br /&gt;", ", ").Replace("&", "").Trim(new char[] { ',', ' ' }) : "");
                }
            }
            // Getting all Organisation Programs data
            var dataAdminMerchant = (from temp in adminMerchant select temp).AsEnumerable();
            //Search  
            var sortColumnAdminMerchant = "DateAdded";
            var sortColumnDirectionAdminMerchant = "desc";
            dataAdminMerchant = dataAdminMerchant.OrderBy(sortColumnAdminMerchant + " " + sortColumnDirectionAdminMerchant);
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataAdminMerchant = dataAdminMerchant.Where(m => m.MerchantName.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                   || (m.Id != null && m.Id.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.Location != null && m.Location.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.AccountType != null && m.AccountType.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.LastTransaction != null && m.LastTransaction.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.DateAdded.ToString("dd MM yyyy").ToLower().Trim().Contains(searchValue.ToLower().Trim())));
            }
            ExcelPackage excelAdminMerchant = new ExcelPackage();
            var workSheetAdminMerchant = excelAdminMerchant.Workbook.Worksheets.Add("Sheet1");
            workSheetAdminMerchant.TabColor = System.Drawing.Color.Black;
            workSheetAdminMerchant.DefaultRowHeight = 12;
            //Header of table  
            workSheetAdminMerchant.Row(1).Height = 20;
            workSheetAdminMerchant.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheetAdminMerchant.Row(1).Style.Font.Bold = true;
            workSheetAdminMerchant.Cells[1, 1].Value = "ID";
            workSheetAdminMerchant.Cells[1, 2].Value = "MERCHANT NAME";
            workSheetAdminMerchant.Cells[1, 3].Value = "LOCATION";
            workSheetAdminMerchant.Cells[1, 4].Value = "LAST TRANSACTION";
            workSheetAdminMerchant.Cells[1, 5].Value = "DATE ADDED";
            workSheetAdminMerchant.Cells[1, 6].Value = "ACCOUNT TYPE";
            //Body of table  

            int recordIndex = 2;
            foreach (var d in dataAdminMerchant)
            {
                workSheetAdminMerchant.Cells[recordIndex, 1].Value = d.MerchantId;
                workSheetAdminMerchant.Cells[recordIndex, 2].Value = d.MerchantName;
                workSheetAdminMerchant.Cells[recordIndex, 3].Value = d.Location;
                workSheetAdminMerchant.Cells[recordIndex, 4].Value = d.LastTransaction;
                workSheetAdminMerchant.Cells[recordIndex, 5].Value = d.DateAdded.ToShortDateString();
                workSheetAdminMerchant.Cells[recordIndex, 6].Value = d.AccountType;
                recordIndex++;
            }
            workSheetAdminMerchant.Column(1).AutoFit();
            workSheetAdminMerchant.Column(2).AutoFit();
            workSheetAdminMerchant.Column(3).AutoFit();
            workSheetAdminMerchant.Column(4).AutoFit();
            workSheetAdminMerchant.Column(5).AutoFit();
            workSheetAdminMerchant.Column(6).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelAdminMerchant.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameAdminMerchant });
        }
        /// <summary>
        /// This method is used to export merchant in excel.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="ppId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MerchantExportExcel(string searchValue, string ppId)
        {
            var primaryPrgIdMerchantExport = Cryptography.DecryptCipherToPlain(ppId);
            var fileNameMerchantExpt = "Merchant List.xlsx";
            List<MerchantDto> merchantExport = new List<MerchantDto>();
            using (var clientMerchantExp = new HttpClient())
            {
                clientMerchantExp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientMerchantExp.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var resultMerchantExp = clientMerchantExp.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.MerchantListWithTransaction + "?programId=" + primaryPrgIdMerchantExport).Result;
                if (resultMerchantExp.IsSuccessStatusCode)
                {
                    var responseMerchantExp = await resultMerchantExp.Content.ReadAsAsync<apiModel.ApiResponse>();
                    dynamic responseDesMerchantExp = JsonConvert.DeserializeObject(responseMerchantExp.Result.ToString());
                    merchantExport = responseDesMerchantExp.ToObject<List<MerchantDto>>();
                    merchantExport.ForEach(x => x.Id = Cryptography.EncryptPlainToCipher(x.Id.ToString()));
                    merchantExport.ForEach(x => x.AccountType = !string.IsNullOrEmpty(x.AccountType) ? x.AccountType.Replace("lt;br /&gt;", ", ").Replace("&", "").Trim(new char[] { ',', ' ' }) : "");
                }
            }
            // Getting all Organisation Programs data
            var dataMerchantExp = (from temp in merchantExport select temp).AsEnumerable();
            //Search  
            var sortColumnMerchantExp = "DateAdded";
            var sortColumnDirectionMerchantExp = "desc";
            //Sort
            dataMerchantExp = dataMerchantExp.OrderBy(sortColumnMerchantExp + " " + sortColumnDirectionMerchantExp);
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataMerchantExp = dataMerchantExp.Where(m => m.MerchantName.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                   || (m.Id != null && m.Id.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.Location != null && m.Location.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.AccountType != null && m.AccountType.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.LastTransaction != null && m.LastTransaction.ToLower().Trim().Contains(searchValue.ToLower().Trim()))
                   || (m.DateAdded.ToString("dd MM yyyy").ToLower().Trim().Contains(searchValue.ToLower().Trim())));
            }
            ExcelPackage excelMerchantExp = new ExcelPackage();
            var workSheetMerchantExp = excelMerchantExp.Workbook.Worksheets.Add("Sheet1");
            workSheetMerchantExp.TabColor = System.Drawing.Color.Black;
            workSheetMerchantExp.DefaultRowHeight = 12;
            //Header of table  
            workSheetMerchantExp.Row(1).Height = 20;
            workSheetMerchantExp.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheetMerchantExp.Row(1).Style.Font.Bold = true;
            workSheetMerchantExp.Cells[1, 1].Value = "ID";
            workSheetMerchantExp.Cells[1, 2].Value = "MERCHANT NAME";
            workSheetMerchantExp.Cells[1, 3].Value = "LOCATION";
            workSheetMerchantExp.Cells[1, 4].Value = "LAST TRANSACTION";
            workSheetMerchantExp.Cells[1, 5].Value = "DATE ADDED";
            workSheetMerchantExp.Cells[1, 6].Value = "ACCOUNT TYPE";
            //Body of table  

            int recordIndex = 2;
            foreach (var d in dataMerchantExp)
            {
                workSheetMerchantExp.Cells[recordIndex, 1].Value = d.MerchantId;
                workSheetMerchantExp.Cells[recordIndex, 2].Value = d.MerchantName;
                workSheetMerchantExp.Cells[recordIndex, 3].Value = d.Location;
                workSheetMerchantExp.Cells[recordIndex, 4].Value = d.LastTransaction;
                workSheetMerchantExp.Cells[recordIndex, 5].Value = d.DateAdded.ToShortDateString();
                workSheetMerchantExp.Cells[recordIndex, 6].Value = d.AccountType;
                recordIndex++;
            }
            workSheetMerchantExp.Column(1).AutoFit();
            workSheetMerchantExp.Column(2).AutoFit();
            workSheetMerchantExp.Column(3).AutoFit();
            workSheetMerchantExp.Column(4).AutoFit();
            workSheetMerchantExp.Column(5).AutoFit();
            workSheetMerchantExp.Column(6).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelMerchantExp.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameMerchantExpt });
        }
        #endregion

        #region Export Plan Listing
        /// <summary>
        /// This method is used to export excel for plan.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="ppId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PlanExportExcel(string searchValue, string ppId)
        {
            var primaryPrgIdPlanExp = Cryptography.DecryptCipherToPlain(ppId);
            var fileNamePlanExp = "Plan List.xlsx";
            List<PlanListingDto> plansExp = new List<PlanListingDto>();
            using (var clientPlanExp = new HttpClient())
            {
                clientPlanExp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientPlanExp.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var resultPlanExp = clientPlanExp.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.PlanListing + "?programId=" + primaryPrgIdPlanExp).Result;
                if (resultPlanExp.IsSuccessStatusCode)
                {
                    var responsePlanExp = await resultPlanExp.Content.ReadAsAsync<apiModel.ApiResponse>();
                    dynamic responseDesPlanExp = JsonConvert.DeserializeObject(responsePlanExp.Result.ToString());
                    plansExp = responseDesPlanExp.ToObject<List<PlanListingDto>>();
                    plansExp.ForEach(x => x.StrId = Cryptography.EncryptPlainToCipher(x.Id.ToString()));
                    plansExp.ForEach(x => x.StartEnd = !string.IsNullOrEmpty(x.StartEnd) ? x.StartEnd.Replace("<br />", " - ") : "");
                    plansExp.ForEach(x => x.Accounts = !string.IsNullOrEmpty(x.Accounts) ? x.Accounts.Replace("lt;br /&gt;", ", ").Replace("&", "").Trim(new char[] { ',', ' ' }) : "");
                }
            }
            // Getting all Organisation Programs data
            var dataPlanExp = (from temp in plansExp select temp).AsEnumerable();

            var sortColumn = "ClientId";
            var sortColumnDirection = "desc";
            //Sorting  
            dataPlanExp = dataPlanExp.OrderBy(sortColumn + " " + sortColumnDirection);

            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataPlanExp = dataPlanExp.Where(m => m.Name.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.Accounts.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.ClientId.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.Description.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.InternalId.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.StartEnd.ToLower().Trim().Contains(searchValue.ToLower().Trim()));
            }
            ExcelPackage excelPlanExp = new ExcelPackage();
            var workSheetPlanExp = excelPlanExp.Workbook.Worksheets.Add("Sheet1");
            workSheetPlanExp.TabColor = System.Drawing.Color.Black;
            workSheetPlanExp.DefaultRowHeight = 12;
            //Header of table  
            workSheetPlanExp.Row(1).Height = 20;
            workSheetPlanExp.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetPlanExp.Row(1).Style.Font.Bold = true;
            workSheetPlanExp.Cells[1, 1].Value = "CLIENT ID";
            workSheetPlanExp.Cells[1, 2].Value = "INTERNAL ID";
            workSheetPlanExp.Cells[1, 3].Value = "PLAN NAME";
            workSheetPlanExp.Cells[1, 4].Value = "DESCRIPTION";
            workSheetPlanExp.Cells[1, 5].Value = "START - END";
            workSheetPlanExp.Cells[1, 6].Value = "ACCOUNTS";
            //Body of table  

            int recordIndex = 2;
            foreach (var d in dataPlanExp)
            {
                workSheetPlanExp.Cells[recordIndex, 1].Value = d.ClientId;
                workSheetPlanExp.Cells[recordIndex, 2].Value = d.InternalId;
                workSheetPlanExp.Cells[recordIndex, 3].Value = d.Name;
                workSheetPlanExp.Cells[recordIndex, 4].Value = d.Description;
                workSheetPlanExp.Cells[recordIndex, 5].Value = d.StartEnd;
                workSheetPlanExp.Cells[recordIndex, 6].Value = d.Accounts;
                recordIndex++;
            }
            workSheetPlanExp.Column(1).AutoFit();
            workSheetPlanExp.Column(2).AutoFit();
            workSheetPlanExp.Column(3).AutoFit();
            workSheetPlanExp.Column(4).AutoFit();
            workSheetPlanExp.Column(5).AutoFit();
            workSheetPlanExp.Column(6).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelPlanExp.GetAsByteArray());

            //Return the Excel file name
            return Json(new { fileName = fileNamePlanExp });
        }
        #endregion

        #region Export Account Holder Listing
        /// <summary>
        /// This method is used to export account holder excel sheet.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="poId"></param>
        /// <param name="ppId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AccountHolderExportExcel(string searchValue, string poId, string ppId, string planId)
        {
            var primaryPrgIdACHExport = Cryptography.DecryptCipherToPlain(ppId);
            var fileNameACHExport = "Account Holder List.xlsx";
            List<AccountHolderDto> userAccountHolderExport = new List<AccountHolderDto>();
            var organisationIdACHExport = !string.IsNullOrEmpty(poId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(poId)) : 0;
            using (var clientACHexport = new HttpClient())
            {
                clientACHexport.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientACHexport.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                var resultAchExport = clientACHexport.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetAccountHolders + "?organisationId=" + organisationIdACHExport + "&programId=" + primaryPrgIdACHExport + "&searchValue=" + searchValue + "&pageNumber=" + 0 + "&pageSize=" + 0 + "&sortColumnName=DateAdded&sortOrderDirection=desc&planId=" + (!string.IsNullOrEmpty(planId) ? Convert.ToInt32(Cryptography.DecryptCipherToPlain(planId)) : 0)).Result;
                if (resultAchExport.IsSuccessStatusCode)
                {
                    var responseAchExport = await resultAchExport.Content.ReadAsAsync<ApiResponse>();
                    if (responseAchExport.StatusFlagNum == 1)
                    {
                        dynamic responseDescAchExport = JsonConvert.DeserializeObject(responseAchExport.Result.ToString());
                        userAccountHolderExport = responseDescAchExport.ToObject<List<AccountHolderDto>>();
                        userAccountHolderExport.ForEach(x => { x.UserEncId = Cryptography.EncryptPlainToCipher(x.Id.ToString()); });
                    }
                }
            }
            // Getting all Organisation Programs data
            var dataAchExport = (from temp in userAccountHolderExport select temp).AsEnumerable();
            ExcelPackage excelAchExport = new ExcelPackage();
            var workSheetAchExport = excelAchExport.Workbook.Worksheets.Add("Sheet1");
            workSheetAchExport.TabColor = System.Drawing.Color.Black;
            workSheetAchExport.DefaultRowHeight = 12;
            //Header of table  
            workSheetAchExport.Row(1).Height = 20;
            workSheetAchExport.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAchExport.Row(1).Style.Font.Bold = true;
            workSheetAchExport.Cells[1, 1].Value = "STUDENT ID";
            workSheetAchExport.Cells[1, 2].Value = "NAME";
            workSheetAchExport.Cells[1, 3].Value = "EMAIL";
            workSheetAchExport.Cells[1, 4].Value = "DATE ADDED";
            workSheetAchExport.Cells[1, 5].Value = "PLAN NAME";
            workSheetAchExport.Cells[1, 6].Value = "Status";
            //Body of table  

            int recordIndex = 2;
            foreach (var d in dataAchExport)
            {
                workSheetAchExport.Cells[recordIndex, 1].Value = d.AccountHolderID;
                workSheetAchExport.Cells[recordIndex, 2].Value = d.Name;
                workSheetAchExport.Cells[recordIndex, 3].Value = d.Email;
                workSheetAchExport.Cells[recordIndex, 4].Value = d.DateAdded.Value.ToShortDateString();
                workSheetAchExport.Cells[recordIndex, 5].Value = d.PlanName;
                if (d.Status == 3)
                {
                    workSheetAchExport.Cells[recordIndex, 6].Value = "Not signed up";
                }
                else if (d.Status == 2)
                {
                    workSheetAchExport.Cells[recordIndex, 6].Value = "Signed up but not linked to the program";
                }
                else { workSheetAchExport.Cells[recordIndex, 6].Value = "Signed up and linked to the program"; }
                recordIndex++;
            }
            workSheetAchExport.Column(1).AutoFit();
            workSheetAchExport.Column(2).AutoFit();
            workSheetAchExport.Column(3).AutoFit();
            workSheetAchExport.Column(4).AutoFit();
            workSheetAchExport.Column(5).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelAchExport.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameACHExport });
        }
        #endregion

        #region Export Account Listing
        /// <summary>
        /// This method is used to export excel sheet for account.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="ppId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AccountExportExcel(string searchValue, string ppId)
        {
            var primaryPrgIdAccExp = Cryptography.DecryptCipherToPlain(ppId);
            var fileNameAccExport = "Account List.xlsx";
            List<AccountListingDto> progAccExp = new List<AccountListingDto>();
            using (var clientAccExport = new HttpClient())
            {
                clientAccExport.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientAccExport.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var resultAccExp = clientAccExport.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ProgramAccountListing + "?programId=" + primaryPrgIdAccExp).Result;
                if (resultAccExp.IsSuccessStatusCode)
                {
                    var responseAccExp = await resultAccExp.Content.ReadAsAsync<apiModel.ApiResponse>();
                    dynamic responseDesAccExp = JsonConvert.DeserializeObject(responseAccExp.Result.ToString());
                    progAccExp = responseDesAccExp.ToObject<List<AccountListingDto>>();
                    progAccExp.ForEach(x => x.StrId = Cryptography.EncryptPlainToCipher(x.Id.ToString()));
                    progAccExp.ForEach(x => x.PlanName = !string.IsNullOrEmpty(x.PlanName) ? x.PlanName.Replace("lt;br /&gt;", ", ").Replace("&", "").Trim(new char[] { ',', ' ' }) : "");

                }
            }
            // Getting all Organisation Programs data
            var dataAccExp = (from temp in progAccExp select temp).AsEnumerable();

            var sortColumnAccExp = "ProgramAccountId";
            var sortColumnDirectionAccExp = "desc";
            //Sorting  
            dataAccExp = dataAccExp.OrderBy(sortColumnAccExp + " " + sortColumnDirectionAccExp);

            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataAccExp = dataAccExp.Where(m => m.AccountName.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.AccountType.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.ProgramAccountId.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || m.PlanName.ToLower().Trim().Contains(searchValue.ToLower().Trim()));
            }
            ExcelPackage excelAccExp = new ExcelPackage();
            var workSheetAccExp = excelAccExp.Workbook.Worksheets.Add("Sheet1");
            workSheetAccExp.TabColor = System.Drawing.Color.Black;
            workSheetAccExp.DefaultRowHeight = 12;
            //Header of table  
            workSheetAccExp.Row(1).Height = 20;
            workSheetAccExp.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAccExp.Row(1).Style.Font.Bold = true;
            workSheetAccExp.Cells[1, 1].Value = "ID";
            workSheetAccExp.Cells[1, 2].Value = "ACCOUNT NAME";
            workSheetAccExp.Cells[1, 3].Value = "ACCOUNT TYPE";
            workSheetAccExp.Cells[1, 4].Value = "PLANS ASSOCIATED";
            //Body of table  
            int recordIndex = 2;
            foreach (var d in dataAccExp)
            {
                workSheetAccExp.Cells[recordIndex, 1].Value = d.ProgramAccountId;
                workSheetAccExp.Cells[recordIndex, 2].Value = d.AccountName;
                workSheetAccExp.Cells[recordIndex, 3].Value = d.AccountType;
                workSheetAccExp.Cells[recordIndex, 4].Value = d.PlanName;
                recordIndex++;
            }
            workSheetAccExp.Column(1).AutoFit();
            workSheetAccExp.Column(2).AutoFit();
            workSheetAccExp.Column(3).AutoFit();
            workSheetAccExp.Column(4).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelAccExp.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameAccExport });
        }
        #endregion

        #region Export Program Transaction Listing
        /// <summary>
        /// This method is used to program transaction export excel sheet.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="ppId"></param>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ProgramTransactionExportExcel(string searchValue, string ppId, string dateMonth = "")
        {
            var primaryPrgIdTranExp = Cryptography.DecryptCipherToPlain(ppId);
            var fileNameTranExp = "Transaction List.xlsx";
            List<TransactionViewDto> transactionsExp = new List<TransactionViewDto>();
            using (var clientTranExp = new HttpClient())
            {
                clientTranExp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientTranExp.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                var resultTranExp = clientTranExp.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.Transactions + "?programId=" + primaryPrgIdTranExp + "&dateMonth=" + dateMonth).Result;
                if (resultTranExp.IsSuccessStatusCode)
                {
                    var responseTranExp = await resultTranExp.Content.ReadAsAsync<ApiResponse>();
                    if (responseTranExp.Result != null)
                    {
                        transactionsExp = JsonConvert.DeserializeObject<List<TransactionViewDto>>(responseTranExp.Result.ToString());
                    }
                }
            }
            // Getting all Organisation Programs data
            var dataTranExp = (from temp in transactionsExp select temp).AsEnumerable();
            //Sorting
            var sortColumn = "MerchantName";
            var sortColumnDirection = "desc";
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                dataTranExp = dataTranExp.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataTranExp = dataTranExp.Where(o => Convert.ToString(o.Amount).ToLower().Trim().Contains(searchValue.ToLower().Trim()) || o.AccountType.ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || o.TransactionDate.ToString().ToLower().Trim().Contains(searchValue.ToLower().Trim())
                || o.MerchantName.ToLower().Trim().Contains(searchValue.ToLower().Trim()));
            }
            ExcelPackage excelTranExp = new ExcelPackage();
            var workSheetTranExp = excelTranExp.Workbook.Worksheets.Add("Sheet1");
            workSheetTranExp.TabColor = System.Drawing.Color.Black;
            workSheetTranExp.DefaultRowHeight = 12;
            //Header of table  
            workSheetTranExp.Row(1).Height = 20;
            workSheetTranExp.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetTranExp.Row(1).Style.Font.Bold = true;
            workSheetTranExp.Cells[1, 1].Value = "DATE";
            workSheetTranExp.Cells[1, 2].Value = "TIME";
            workSheetTranExp.Cells[1, 3].Value = "ACCOUNT";
            workSheetTranExp.Cells[1, 4].Value = "MERCHANT NAME";
            workSheetTranExp.Cells[1, 5].Value = "AMOUNT";
            //Body of table  

            int recordIndex = 2;
            foreach (var d in dataTranExp)
            {
                workSheetTranExp.Cells[recordIndex, 1].Value = d.TransactionDate.ToShortDateString();
                workSheetTranExp.Cells[recordIndex, 2].Value = d.TransactionDate.ToString("hh:mm:ss tt");
                workSheetTranExp.Cells[recordIndex, 3].Value = d.AccountType;
                workSheetTranExp.Cells[recordIndex, 4].Value = d.MerchantName;
                workSheetTranExp.Cells[recordIndex, 5].Value = d.Amount;
                recordIndex++;
            }
            workSheetTranExp.Column(1).AutoFit();
            workSheetTranExp.Column(2).AutoFit();
            workSheetTranExp.Column(3).AutoFit();
            workSheetTranExp.Column(4).AutoFit();
            workSheetTranExp.Column(5).AutoFit();
            HttpContext.Session.Set("DownloadExcel_OrgProg", excelTranExp.GetAsByteArray());
            //Return the Excel file name
            return Json(new { fileName = fileNameTranExp });
        }
        #endregion
        /// <summary>
        /// This method is used to download the excel sheet after exporting data from program.
        /// </summary>
        /// <param name="fileName">Program Data to download</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Download(string fileName)
        {
            byte[] dataPrgContent;
            if (HttpContext.Session.TryGetValue("DownloadExcel_OrgProg", out dataPrgContent))
            {
                return File(dataPrgContent, "application/octet-stream", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> PostSiteLevelOverrideSettings(SiteLevelOverrideSettingViewModel model)
        {
            string resultId = "0";
            string message = string.Empty;

            try
            {
                var data = new SiteLevelOverrideSettingViewModel()
                {
                    id = model.id,
                    programId = model.programId,
                    siteLevelBitePayRatio = model.siteLevelBitePayRatio,
                    siteLevelDcbFlexRatio = model.siteLevelDcbFlexRatio,
                    siteLevelUserStatusRegularRatio = model.siteLevelUserStatusRegularRatio,
                    siteLevelUserStatusVipRatio = model.siteLevelUserStatusVipRatio,
                    createdDate = DateTime.Now,
                    modifiedDate = DateTime.Now,
                    FirstTransactionBonus=model.FirstTransactionBonus
                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.AddUpdateSiteLevelOverrideSettings, stringContent);

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
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (PostSiteLevelSettings - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }

            return Json(new { data = resultId, message = message });
        }
    }
}