using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Foundry.Domain;
using response = Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Newtonsoft.Json;
using IdentityModel.Client;
using static Foundry.Domain.Constants;
using Microsoft.Extensions.Configuration;
using ElmahCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Globalization;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This is the class which is having all the methods for a user flow like login, forgot password, reset etc.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// This is the constructor for the Account Controller to configure the interfaces which needs to be used.
        /// </summary>
        /// <param name="configuration"></param>
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method is used to get the user check and redirecting it to dashboard if the cookie exists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uid"></param>
        /// <param name="pid"></param>
        /// <param name="ptype"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string id, string uid, string pid, string ptype)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage result = CheckUserInactivity(id, uid, pid, ptype, client);
                        if (result.IsSuccessStatusCode)
                        {
                            var resultCheck = await result.Content.ReadAsAsync<response.ApiResponse>();
                            dynamic invitationDetail = InactiveUserResponseDeserailize(resultCheck);
                            if (!string.IsNullOrEmpty(ptype))
                            {
                                if (resultCheck.StatusFlagNum == 1)
                                {
                                    return IvitationEmailConfirmedCheck(id, invitationDetail);
                                }
                            }
                            else
                            {
                                return RedirectToBenefactor(id, resultCheck, invitationDetail);
                            }
                        }
                    }
                }
                else
                {
                    return RefactorRedirectForAuthentiacatedUser();
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (Index) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View("Login");
        }

        private IActionResult RedirectToBenefactor(string id, response.ApiResponse resultCheck, dynamic invitationDetail)
        {
            if (resultCheck.StatusFlagNum == 1)
            {
                return CheckUserForInactivityNRedirection(invitationDetail);
            }
            else
            {
                return RedirectionToCreatePassword(id, invitationDetail);
            }
        }

        private HttpResponseMessage CheckUserInactivity(string id, string uid, string pid, string ptype, HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.UserInactiveCheck + "?id=" + id + "&uid=" + uid + "&pid=" + pid + (!string.IsNullOrEmpty(ptype) ? "&ptype=" + ptype : "")).Result;
            return result;
        }

        private static dynamic InactiveUserResponseDeserailize(response.ApiResponse resultCheck)
        {
            dynamic response1 = JsonConvert.DeserializeObject(resultCheck.Result.ToString());
            var invitationDetail = response1.ToObject<InvitationDto>();
            return invitationDetail;
        }

        private IActionResult RedirectionToCreatePassword(string id, dynamic invitationDetail)
        {
            return RedirectToAction("CreatePassword", "Account", new { id, userid = invitationDetail.CreatedBy });
        }

        private IActionResult RefactorRedirectForAuthentiacatedUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return AuthenticatedUserRediect();
            }
            return View("Login");
        }

        private IActionResult CheckUserForInactivityNRedirection(dynamic invitationDetail)
        {
            if (User.Identity.IsAuthenticated)
            {
                return CheckBenefactorRoleNother(invitationDetail);
            }
            else { return RedirectToAction("Login", new { islink = true, id = invitationDetail.CreatedBy }); }
        }

        private IActionResult AuthenticatedUserRediect()
        {
            if (User.IsInRole(Roles.Benefactor))
            {
                return RedirectToAction("Dashboard", "Benefactor");
            }
            else if (User.IsInRole(Roles.SuperAdmin) || User.IsInRole(Roles.MerchantFull) ||
                                    User.IsInRole(Roles.MerchantReporting) || User.IsInRole(Roles.OrganizationFull) ||
                                    User.IsInRole(Roles.OrganizationReporting) || User.IsInRole(Roles.ProgramFull) ||
                                    User.IsInRole(Roles.ProgramReporting))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("UserReloadAmout", "Benefactor");
            }
        }

        private IActionResult CheckBenefactorRoleNother(dynamic invitationDetail)
        {
            if (User.IsInRole(Roles.Benefactor))
            {
                return RedirectToAction("LinkedAccounts", "Benefactor", new { islink = true, id = invitationDetail.CreatedBy });
            }
            else if (User.IsInRole(Roles.SuperAdmin) || User.IsInRole(Roles.MerchantFull) ||
                User.IsInRole(Roles.MerchantReporting) || User.IsInRole(Roles.OrganizationFull) ||
                User.IsInRole(Roles.OrganizationReporting) || User.IsInRole(Roles.ProgramFull) ||
                User.IsInRole(Roles.ProgramReporting))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("UserReloadAmout", "Benefactor");
            }
        }

        private IActionResult IvitationEmailConfirmedCheck(string id, dynamic invitationDetail)
        {
            if (!invitationDetail?.EmailConfirmed)
                return RedirectToAction("CreatePassword", "Account", new { id, userid = invitationDetail?.CreatedBy });
            else { return RedirectToAction("Login", new { islink = true, id = invitationDetail?.CreatedBy }); }
        }

        /// <summary>
        /// This method is used to get the Login View.
        /// </summary>
        /// <param name="islink"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Login(bool islink, int? id, string returnUrl = "")
        {
            LoginModel model = new LoginModel();
            try
            {
                if (TempData["Message"] != null && TempData["Status"] != null)
                {
                    ViewBag.StatusResponse = TempData["Status"];
                    ViewBag.MessageResponse = TempData["Message"];
                    TempData["Message"] = null;
                    TempData["Status"] = null;
                }
                model.Id = id.HasValue ? id.Value : 0;
                model.IsLink = islink;
                model.ReturnURL = returnUrl;
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (Login - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View(model);
        }

        /// <summary>
        /// This method is used to post the login detail to validate the user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var client = new HttpClient();
                    DiscoveryResponse disco = await client.GetDiscoveryDocumentAsync(_configuration["ServiceAPIURL"]);
                    if (disco.IsError)
                    {
                        return DiscoveryErrorAction(disco);
                    }

                    // request token
                    TokenResponse tokenResponse = await GetAuthToken(model, client);

                    if (tokenResponse.IsError)
                    {
                        return TokenErrorAction();
                    }
                    UserInfoResponse userInfo = await GetUserInfoAfterAuthToken(client, disco, tokenResponse);
                    var claims = new List<Claim>();
                    var userActiveCheck = userInfo.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "isuseractive".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                    if (!string.IsNullOrEmpty(userActiveCheck) && !Convert.ToBoolean(userActiveCheck))
                    {
                        return UserInactiveAction();
                    }

                    GetClaims(userInfo, claims);
                    ClaimsIdentity identity = AddCustomClaims(tokenResponse, claims);

                    SettingUserLoginPrincipals(identity);
                    if (!string.IsNullOrEmpty(model.ReturnURL))
                    {
                        return Redirect(model.ReturnURL);
                    }
                    else
                    {
                        if (claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Basic User".ToLower(CultureInfo.InvariantCulture).Trim())
                        {
                            return BasicUserLoginAction();
                        }
                        else if (claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Benefactor".ToLower(CultureInfo.InvariantCulture).Trim())
                        {
                            return BenefactorLoginAction(model);
                        }
                        else if (claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Super Admin".ToLower(CultureInfo.InvariantCulture).Trim()
                            || claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Organization Full".ToLower(CultureInfo.InvariantCulture).Trim()
                            || claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Organization Reporting".ToLower(CultureInfo.InvariantCulture).Trim()
                            || claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Merchant Full".ToLower(CultureInfo.InvariantCulture).Trim()
                            || claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Merchant Reporting".ToLower(CultureInfo.InvariantCulture).Trim()
                            || claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Program Full".ToLower(CultureInfo.InvariantCulture).Trim()
                            || claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Program Reporting".ToLower(CultureInfo.InvariantCulture).Trim())
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else if (claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Super Admin".ToLower(CultureInfo.InvariantCulture).Trim())
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (Login - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }

        private IActionResult DiscoveryErrorAction(DiscoveryResponse disco)
        {
            TempData["Message"] = disco.Error;
            TempData["Status"] = false;
            return RedirectToAction("Login");
        }

        private IActionResult BasicUserLoginAction()
        {
            return RedirectToAction("UserReloadAmout", "Benefactor");
        }

        private IActionResult BenefactorLoginAction(LoginModel model)
        {
            if (model.Id > 0 && model.IsLink.HasValue && model.IsLink.Value)
            {
                return RedirectToAction("LinkedAccounts", "Benefactor", new { islink = model.IsLink.Value, id = model.Id });
            }
            return RedirectToAction("Dashboard", "Benefactor");
        }

        private IActionResult UserInactiveAction()
        {
            TempData["Message"] = MessagesConstants.UserAccountDeactivated.Replace("{supportemail}", _configuration["SupportEmail"]);
            TempData["Status"] = false;
            return RedirectToAction("Login");
        }

        private IActionResult TokenErrorAction()
        {
            TempData["Message"] = "Username or password is incorrect.";
            TempData["Status"] = false;
            return RedirectToAction("Login");
        }

        private void SettingUserLoginPrincipals(ClaimsIdentity identity)
        {
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties();
            props.IsPersistent = true;

            HttpContext.SignInAsync(
          CookieAuthenticationDefaults.
AuthenticationScheme,
          principal,
          props).Wait();
        }

        private static ClaimsIdentity AddCustomClaims(TokenResponse tokenResponse, List<Claim> claims)
        {
            claims.Add(new Claim("AccessToken", tokenResponse.AccessToken));
            claims.Add(new Claim("TokenExpiresIn", DateTime.UtcNow.AddSeconds(Convert.ToInt32(tokenResponse.ExpiresIn.ToString())).ToString()));

            claims.Add(new Claim("TokenRefreshIdentity", tokenResponse.RefreshToken));
            var identity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.
            AuthenticationScheme);
            return identity;
        }

        private static void GetClaims(UserInfoResponse userInfo, List<Claim> claims)
        {
            foreach (var item in userInfo.Claims)
            {
                claims.Add(new Claim(item.Type, item.Value));
            }
        }

        private static async Task<UserInfoResponse> GetUserInfoAfterAuthToken(HttpClient client, DiscoveryResponse disco, TokenResponse tokenResponse)
        {
            return await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = tokenResponse.AccessToken
            });
        }

        private async Task<TokenResponse> GetAuthToken(LoginModel model, HttpClient client)
        {
            return await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = _configuration["ServiceAPIURL"] + ApiConstants.GenerateUserToken,

                ClientId = "ro.angular",
                ClientSecret = "secret",
                Scope = "openid offline_access",
                GrantType = "password",
                UserName = model.EmailAddress.Trim(),
                Password = model.Password.Trim(),

            });
        }

        /// <summary>
        /// This method is used to get the ForgotPassword View.
        /// </summary>
        /// <returns></returns>
        public IActionResult ForgotPassword()
        {
            if (TempData["Message"] != null && TempData["Status"] != null)
            {
                ViewBag.StatusResponse = TempData["Status"];
                ViewBag.MessageResponse = TempData["Message"];
                TempData["Message"] = null;
                TempData["Status"] = null;
            }
            ForgotPasswordModel model = new ForgotPasswordModel();
            return View(model);
        }

        /// <summary>
        /// This method is used to post the Forgot Password detail.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var forgotDto = new response.ForgotPasswordModel
                    {
                        EmailAddress = model.EmailAddress.Trim()
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(forgotDto);
                        HttpContent stringContent = new StringContent(json);
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ForgotUserPassword, stringContent);
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<response.ApiResponse>();
                            TempData["Message"] = response.Message;
                            TempData["Status"] = response.StatusFlagNum;
                        }
                    }
                    return RedirectToAction("ForgotPassword");
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (ForgotPassword - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }

        /// <summary>
        /// This method is used to check the reset password view and validate it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ResetPasswordCheck(string id)
        {
            try
            {
                var resetDto = new ResetPasswordCheckDto
                {
                    EmailAddress = Cryptography.DecryptCipherToPlain(id.Trim()),

                };
                using (var client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(resetDto);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.CheckUseResetPasswordLink, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<response.ApiResponse>();
                        if (response.StatusFlagNum == 0)
                        {
                            TempData["Message"] = response.Message;
                            TempData["Status"] = response.StatusFlagNum;

                            return RedirectToAction("ForgotPassword");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (ResetPasswordCheck - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return RedirectToAction("ResetPassword", new { id });
        }

        /// <summary>
        /// This method is used to reset the password post data and getting the response.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ResetPassword(string id)
        {
            ResetPasswordModel model = new ResetPasswordModel { Id = id.Trim() };
            try
            {
                bool invite = false;
                if (TempData["Message"] != null && TempData["Status"] != null)
                {
                    ViewBag.StatusResponse = TempData["Status"];
                    ViewBag.MessageResponse = TempData["Message"];
                    TempData["Message"] = null;
                    TempData["Status"] = null;
                    ViewBag.Redirect = TempData["Redirect"];
                    TempData["Redirect"] = null;
                }
                invite = Convert.ToBoolean(TempData["invite"]);
                TempData["invite"] = null;
                if (invite) { model.LabelForPage = "Create Password"; model.invite = true; }
                else { model.LabelForPage = "Reset Password"; model.invite = false; }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (ResetPassword - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View(model);
        }
        /// <summary>
        /// This method is used to get the create password page for creating a new password for new users.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public IActionResult CreatePassword(string id, int UserId)
        {
            ResetPasswordModel model = new ResetPasswordModel { Id = id.Trim(), UserId = UserId, IsLink = true };
            try
            {
                if (TempData["Message"] != null && TempData["Status"] != null)
                {
                    ViewBag.StatusResponse = TempData["Status"];
                    ViewBag.MessageResponse = TempData["Message"];
                    TempData["Message"] = null;
                    TempData["Status"] = null;
                    ViewBag.Redirect = TempData["Redirect"];
                    TempData["Redirect"] = null;
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (CreatePassword - Get) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View(model);
        }

        /// <summary>
        /// This method is used to post the create password detail and create a new password for the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePassword(ResetPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resetCreatePwdDto = new response.ResetPasswordModel
                    {
                        EmailAddress = Cryptography.DecryptCipherToPlain(model.Id.Trim()),
                        Password = model.Password.Trim(),
                        UserId = model.UserId
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(resetCreatePwdDto);

                        await ResetUserPassword(client, json);

                    }

                    return RedirectToAction("CreatePassword", new { UserId = model.UserId });
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (CreatePassword - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }

        private async Task ResetUserPassword(HttpClient client, string json)
        {
            HttpContent stringContent = new StringContent(json.ToString());
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.ResetUserPassword, stringContent);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsAsync<response.ApiResponse>();
                TempData["Message"] = response.Message;
                TempData["Status"] = response.StatusFlagNum;
                TempData["Redirect"] = true;
            }
        }

        /// <summary>
        /// This method is used to post the detail of the user for reset password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resetDto = new response.ResetPasswordModel()
                    {
                        EmailAddress = Cryptography.DecryptCipherToPlain(model.Id.Trim()),
                        Password = model.Password.Trim()
                    };
                    using (var client = new HttpClient())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(resetDto);
                        await ResetUserPassword(client, json);

                    }
                    TempData["invite"] = model.invite;
                    return RedirectToAction("ResetPassword");
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (ResetPassword - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }

        /// <summary>
        /// This method is used to return the View when the user is denied to access a particular page.
        /// </summary>
        /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// This method is used to redirect the user to specific action on clicking the back button.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AccessDeniedUser(int id = 0)
        {
            if (User.IsInRole(Constants.Roles.BasicUser))
            {
                return RedirectToAction("ReloadRequest", "Benefactor");
            }
            return RedirectToAction("Dashboard", "Benefactor");
        }

        /// <summary>
        /// This method is used to signout the user from the web.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("login");
        }
        /// <summary>
        /// This method is used to upload an image of the user in s3.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            string resultId = "";

            if (ModelState.IsValid)
            {
                try
                {

                    using (var client = new HttpClient())
                    {

                        byte[] data;
                        using (var br = new BinaryReader(file.OpenReadStream()))
                            data = br.ReadBytes((int)file.OpenReadStream().Length);

                        ByteArrayContent bytes = new ByteArrayContent(data);

                        MultipartFormDataContent multiContent = new MultipartFormDataContent();

                        multiContent.Add(bytes, "file", file.FileName);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                        var result = client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.UploadImage, multiContent).Result;


                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<response.ApiResponse>();
                            if (response.StatusFlagNum == 1)
                            {
                                resultId = response.Result.ToString();

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Account (UploadImage - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultId });
        }

        /// <summary>
        /// This method is used to remove an image from s3
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imgPath"></param>
        /// <param name="userPhotoEntityType"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveImage(string userId, string imgPath, string userPhotoEntityType)
        {
            int resultId = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    var photoDto = new PhotosDto()
                    {
                        entityId = Convert.ToInt32(userId),
                        photoType = Convert.ToInt32(Constants.PhotoType.UserProfile),
                        photoPath = imgPath
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(photoDto);

                    HttpContent stringContent = new StringContent(json.ToString());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                    var result = await client.PostAsync(_configuration["ServiceAPIURL"] + ApiConstants.DeleteImage, stringContent);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsAsync<Foundry.Domain.ApiModel.ApiResponse>();
                        resultId = Convert.ToInt32(response.Result);
                    }
                }
                return Json(new { data = resultId, success = true });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Organisation (RemoveImage - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = resultId, success = false });
            }


        }

        /// <summary>
        /// This method is used to get the digitally signed URL from S3
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> GetPresignedURLImage(string fileName)
        {
            string resultPath = "";

            if (ModelState.IsValid)
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                        var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.S3ImageForFileName + "?fileName=" + fileName).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsAsync<response.ApiResponse>();
                            resultPath = response.Result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("Web: Account (UploadImage - POST) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                }
            }
            return Json(new { data = resultPath });
        }

        /// <summary>
        /// This method is used to get the card holder agreement by Mobile app.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ViewCardHolderAgreement(string id)
        {
            CardHolderAgreeementModel oModel = new CardHolderAgreeementModel();
            var programId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(id));
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CardholderAgreementByIdUnauth + "?programId=" + programId + "&cardholderAgreementId=" + 0).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsAsync<response.ApiResponse>();
                    dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
                    oModel = response1.ToObject<CardHolderAgreeementModel>();
                }
            }

            return View(oModel);
        }

        /// <summary>
        /// This method is used to get the Add money page to Mobile app.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TransferMoney()
        {
            Microsoft.Extensions.Primitives.StringValues authorizationToken;
            Request.Headers.TryGetValue("Authorization", out authorizationToken);

            Microsoft.Extensions.Primitives.StringValues expires_in;
            Request.Headers.TryGetValue("expires_in", out expires_in);

            Microsoft.Extensions.Primitives.StringValues refresh_token;
            Request.Headers.TryGetValue("refresh_token", out refresh_token);


            var AccessToken = authorizationToken.FirstOrDefault();
            var expiry_in = expires_in.FirstOrDefault();
            var refresh_Access_Token = refresh_token.FirstOrDefault();

            if (AccessToken != null)
            {

                AccessToken = AccessToken.Replace("Bearer ", "");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync(_configuration["ServiceAPIURL"]);
                if (disco.IsError)
                {
                    TempData["Message"] = disco.Error;
                    TempData["Status"] = false;
                    return RedirectToAction("Login");
                }

                var userInfo = await client.GetUserInfoAsync(new UserInfoRequest()
                {
                    Address = disco.UserInfoEndpoint,
                    Token = AccessToken
                });
                var claims = new List<Claim>();
                var userActiveCheck = userInfo.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "isuseractive".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                if (!string.IsNullOrEmpty(userActiveCheck) && !Convert.ToBoolean(userActiveCheck))
                {
                    TempData["Message"] = MessagesConstants.UserAccountDeactivated.Replace("{supportemail}", _configuration["SupportEmail"]);
                    TempData["Status"] = false;
                    return RedirectToAction("Login");
                }

                foreach (var item in userInfo.Claims)
                {
                    claims.Add(new Claim(item.Type, item.Value));
                }
                claims.Add(new Claim("AccessToken", AccessToken));
                claims.Add(new Claim("TokenExpiresIn", DateTime.UtcNow.AddSeconds(Convert.ToInt32(expiry_in.ToString())).ToString()));//

                claims.Add(new Claim("TokenRefreshIdentity", refresh_Access_Token));
                var identity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.
                AuthenticationScheme);


                var principal = new ClaimsPrincipal(identity);

                var props = new AuthenticationProperties();
                props.IsPersistent = true;

                HttpContext.SignInAsync(
              CookieAuthenticationDefaults.
    AuthenticationScheme,
              principal,
              props).Wait();
                return RedirectToAction("UserReloadAmout", "Benefactor");
            }
            else { return RedirectToAction("Login"); }
        }

        /// <summary>
        /// This method is used to get the activate card view.
        /// </summary>
        /// <returns></returns>
        [Route("activatecard")]
        public IActionResult ActivateCard()
        {
            CardActivationModel model = new CardActivationModel();
            return View(model);
        }
        /// <summary>
        /// This method is used to post the activate card detail to validate and getting the response.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ActivateCard(CardActivationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ActivateCard + "?accesscode=" + model.AccessCode).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            await result.Content.ReadAsAsync<Foundry.Domain.ApiModel.ApiResponse>();
                        }
                    }

                    return RedirectToAction("ActivateCard");
                }
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Account (ActivateCard - Post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
            }
            return View();
        }
        public IActionResult UserLoyaltyTrackingTransaction(int id)
        {
            return ViewComponent("UserLoyaltyTrackingTransaction", new { id = id });
        }
  
        public async Task<IActionResult> LoadLoyaltyTrackingTransactions1(int id)
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
                List<UserLoyaltyPointsHistoryDto> userAccountHolder = new List<UserLoyaltyPointsHistoryDto>();

                  var pageNumberCHA = (Convert.ToInt32(startACH) / pageSizeACH) + 1;
            
                using (var clientACH = new HttpClient())
                {
                    clientACH.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientACH.SetBearerToken(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

                    var resultACH = clientACH.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserLoyaltyTrackingTransactions + "?userId=" + id + "&pageNumber=" + pageNumberCHA + "&pageSize=" + pageSizeACH + "&sortColumnName=" + sortColumnACH + "&sortOrderDirection=" + sortColumnDirectionACH ).Result;
                    if (resultACH.IsSuccessStatusCode)
                    {
                        var responseACH = await resultACH.Content.ReadAsAsync<response.ApiResponse>();
                        if (responseACH.StatusFlagNum == 1)
                        {
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseACH.Result.ToString());
                            userAccountHolder = responseDesACH.ToObject<List<UserLoyaltyPointsHistoryDto>>();
                         //   userAccountHolder.ForEach(x => { x.UserEncId = Cryptography.EncryptPlainToCipher(x.Id.ToString()); x.Jpos_AccountEncId = !string.IsNullOrEmpty(x.Jpos_AccountHolderId) ? Cryptography.EncryptPlainToCipher(x.Jpos_AccountHolderId) : ""; });
                        }
                    }
                }
                // Getting all Account holder Programs data
                var userAccountHolderData = (from tempUserAccountHolder in userAccountHolder select tempUserAccountHolder);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumnACH) && string.IsNullOrEmpty(sortColumnDirectionACH)))
                {
                  //  userAccountHolderData = userAccountHolderData.OrderBy(sortColumnACH + " " + sortColumnDirectionACH);
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
        public async Task<IActionResult> LoadLoyaltyTrackingTransactions(int id)
        {
            var drawAllPrg = 1;
            int TotalRecords = 0;
           // int pageSizeTransactionLst = lengthMerchantLst != null ? Convert.ToInt32(lengthMerchantLst) : 0;

           // int skipTransactionLst = startMerchantLst != null ? Convert.ToInt32(startMerchantLst) : 0;
            List<UserLoyaltyPointsHistoryDto> transactions = new List<UserLoyaltyPointsHistoryDto>();
            try
            {
                //Skip number of Rows count
                var startAllPrg = 0;

                //Paging Length 10,20
                var lengthAllPrg =10;
                //Paging Size (10, 20, 50, 100)
                int pageSizeAllPrg = lengthAllPrg != null ? Convert.ToInt32(lengthAllPrg) : 0;

                int skipAllPrg = startAllPrg != null ? Convert.ToInt32(startAllPrg) : 0;
                using (var clientPrgTransaction = new HttpClient())
                {
                   
                    clientPrgTransaction.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgTransaction.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultTransaction = clientPrgTransaction.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserLoyaltyTrackingTransactions + "?userId=" +id).Result;
                    if (resultTransaction.IsSuccessStatusCode)
                    {
                        var responsePrgTransaction = await resultTransaction.Content.ReadAsAsync<response.ApiResponse>();
                        if (responsePrgTransaction.Result != null)
                        {
                            transactions = JsonConvert.DeserializeObject<List<UserLoyaltyPointsHistoryDto>>(responsePrgTransaction.Result.ToString());
                        }

                    }
                }
                //total number of rows counts   
                TotalRecords = transactions.Count();
                var dataAllPrg = transactions.Skip(skipAllPrg).Take(pageSizeAllPrg).ToList();
                return Json(new { draw = drawAllPrg, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = dataAllPrg });
               // return Json(new { data = transactions});
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllTransactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = transactions });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }
        public async Task<IActionResult> LoadLoyaltyRewardTransactions(int id)
        {
            var drawACH = HttpContext.Request.Form["draw"].FirstOrDefault();
            //Skip number of Rows count
            var startACH = Request.Form["start"].FirstOrDefault();

            //Paging Length 10,20
            var length = Request.Form["length"].FirstOrDefault();

            //Sort Column Name
            var sortColumnACH = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

            //Sort Column Direction(asc,desc)
            var sortColumnDirectionACH = Request.Form["order[0][dir]"].FirstOrDefault();

            //Search Value from (Search box)
            var searchValueACH = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10, 20, 50, 100)
            int pageSizeACH = length != null ? Convert.ToInt32(length) : 0;

            int recordsTotalACH = 0;
            List<response.TranlogViewModel> transactions = new List<response.TranlogViewModel>();
            var pageNumberCHA = (Convert.ToInt32(startACH) / pageSizeACH) + 1;
            try
            {
                using (var clientPrgTransaction = new HttpClient())
                {

                    clientPrgTransaction.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgTransaction.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultTransaction = clientPrgTransaction.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserLoyaltyRewardTransactions + "?userId=" + id + "&pagenumber=" + pageNumberCHA + "&pagelength=" + length).Result;
                    if (resultTransaction.IsSuccessStatusCode)
                    {
                        var responseTransaction = await resultTransaction.Content.ReadAsAsync<response.ApiResponse>();
                        if (responseTransaction.Result != null)
                        {
                          //  transactions = JsonConvert.DeserializeObject<List<response.TranlogViewModel>>(responseTransaction.Result.ToString());
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseTransaction.Result.ToString());
                            transactions = responseDesACH.ToObject<List<response.TranlogViewModel>>();
                          
                        }

                    }
                }
                //total number of rows counts 
                recordsTotalACH = transactions.Count > 0 ? transactions.FirstOrDefault().TotalCount : 0;
                return Json(new { draw = drawACH, recordsFiltered = recordsTotalACH, recordsTotal = recordsTotalACH, data = transactions });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllTransactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = transactions });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }

       
        [HttpPost]
        [Authorize(Roles = "Super Admin, Organization Full")]
        public async Task<IActionResult> AdminReloadAmount(Foundry.Web.Models.AdminCreditFundsModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultcredit = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.CreditFundsi2cByAdmin + "?creditUserId=" + model.ReloadUserId + "&amount=" + model.ReloadAmount ).Result;
                    if (!resultcredit.IsSuccessStatusCode)
                    {
                        return Json(new { Status = false, responseCode = "auto-refunded" });
                    }


                }
                //  return PartialView("ManageAccountHolderViewComponent");


                return Json(new { Status = true });
                //string bal = await getUserbalance(model.ReloadUserId.ToString());
                
                //return Json(new { Status = true, balance = bal });

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Acount (AdminReloadAmount - post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { Status = false, responseCode = "auto-refunded" });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }

        

        [HttpPost]
        [Authorize(Roles = "Super Admin, Organization Full")]
        public async Task<IActionResult> AdminDebitFundsi2c(Foundry.Web.Models.AdminCreditFundsModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultcredit = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.DebitFundsi2cByAdmin + "?creditUserId=" + model.ReloadUserId + "&amount=" + model.ReloadAmount).Result;
                    if (!resultcredit.IsSuccessStatusCode)
                    {
                        return Json(new { Status = false, responseCode = "Error" });
                    }


                }


             //   string bal = await getUserbalance(model.ReloadUserId.ToString());


                return Json(new { Status = true});

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Acount (AdminDebitFundsi2c - post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { Status = false, responseCode = "auto-refunded" });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }
        //private async Task<string>  getUserbalance( string UserId)
        //{
        //    var jposusrbalance = new Foundry.Domain.ApiModel.JPOSBiteBalanceApiModel();
        //    string usrBitePayBalance = "0";
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);

        //        var result1 = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetJPOSBiteUserBalance + "?userId=" + UserId).Result;
        //        if (result1.IsSuccessStatusCode)
        //        {
        //            var response = await result1.Content.ReadAsAsync<Foundry.Domain.ApiModel.ApiResponse>();
        //            dynamic response1 = JsonConvert.DeserializeObject(response.Result.ToString());
        //            jposusrbalance = response1.ToObject<Foundry.Domain.ApiModel.JPOSBiteBalanceApiModel>();
        //            if (jposusrbalance.success != false)
        //            {
        //                IEnumerable<double> objbitepay = from s in jposusrbalance.accounts.Where(x => x.name == "bite pay")
        //                                                 select s.balance;
        //                usrBitePayBalance = objbitepay.FirstOrDefault().ToString();
        //            }
        //            else
        //            {
        //                usrBitePayBalance = "0";
        //            }
        //        }
        //    }
        //        return usrBitePayBalance;
        //}




        [HttpPost]
        [Authorize(Roles = "Super Admin, Organization Full")]
        public async Task<IActionResult> AdminReversal(response.Admini2cReversalModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultcredit = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.ReversalI2CTranByAdmin + "?dsamount=" + model.dsamount + "&userId=" + model.userId + "&dstxnid=" + model.dstxnid + "&date=" + model.date).Result;
                    if (!resultcredit.IsSuccessStatusCode)
                    {
                        return Json(new { Status = false, responseCode = "Error" });
                    }


                }
                
                return Json(new { Status = true });

            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Acount (AdminDebitFundsi2c - post) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { Status = false, responseCode = "auto-refunded" });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }



        public async Task<IActionResult> LoadBenefectors(int id)
        {
            var drawACH = HttpContext.Request.Form["draw"].FirstOrDefault();
            //Skip number of Rows count
            var startACH = Request.Form["start"].FirstOrDefault();

            //Paging Length 10,20
            var length = Request.Form["length"].FirstOrDefault();

            //Sort Column Name
            var sortColumnACH = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

            //Sort Column Direction(asc,desc)
            var sortColumnDirectionACH = Request.Form["order[0][dir]"].FirstOrDefault();

            //Search Value from (Search box)
            var searchValueACH = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10, 20, 50, 100)
            int pageSizeACH = length != null ? Convert.ToInt32(length) : 0;

            int recordsTotalACH = 0;
            List<LinkedUsersDto> benefactors = new List<LinkedUsersDto>();
            var pageNumberCHA = (Convert.ToInt32(startACH) / pageSizeACH) + 1;
            try
            {
                using (var httpclient = new HttpClient())
                {

                    httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpclient.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var benefactorsDetails = httpclient.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.BenefactorDetails + "?benefactorId=" + id).Result;
                    if (benefactorsDetails.IsSuccessStatusCode)
                    {
                        var responseTransaction = await benefactorsDetails.Content.ReadAsAsync<response.ApiResponse>();
                        if (responseTransaction.Result != null)
                        {
                            // transactions = JsonConvert.DeserializeObject<List<response.TranlogViewModel>>(responseTransaction.Result.ToString());
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseTransaction.Result.ToString());
                            benefactors = responseDesACH.ToObject<List<LinkedUsersDto>>();
                        }

                    }
                }
                //total number of rows counts
                recordsTotalACH = benefactors.Count;
                return Json(new { draw = drawACH, recordsFiltered = recordsTotalACH, recordsTotal = recordsTotalACH, data = benefactors });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllTransactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = benefactors });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }







        public async Task<IActionResult> LoadBitePayTransactions(int id)
        {
            var drawACH = HttpContext.Request.Form["draw"].FirstOrDefault();
            //Skip number of Rows count
            var startACH = Request.Form["start"].FirstOrDefault();

            //Paging Length 10,20
            var length = Request.Form["length"].FirstOrDefault();

            //Sort Column Name
            var sortColumnACH = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

            //Sort Column Direction(asc,desc)
            var sortColumnDirectionACH = Request.Form["order[0][dir]"].FirstOrDefault();

            //Search Value from (Search box)
            var searchValueACH = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10, 20, 50, 100)
            int pageSizeACH = length != null ? Convert.ToInt32(length) : 0;

            int recordsTotalACH = 0;
            List<response.TranlogViewModel> transactions = new List<response.TranlogViewModel>();
            var pageNumberCHA = (Convert.ToInt32(startACH) / pageSizeACH) + 1;
            try
            {
                using (var clientPrgTransaction = new HttpClient())
                {

                    clientPrgTransaction.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientPrgTransaction.SetBearerToken(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
                    var resultTransaction = clientPrgTransaction.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.GetUserBitePayTransactions + "?userId=" + id + "&pagenumber=" + pageNumberCHA + "&pagelength=" +length).Result;
                    if (resultTransaction.IsSuccessStatusCode)
                    {
                        var responseTransaction = await resultTransaction.Content.ReadAsAsync<response.ApiResponse>();
                        if (responseTransaction.Result != null)
                        {
                            // transactions = JsonConvert.DeserializeObject<List<response.TranlogViewModel>>(responseTransaction.Result.ToString());
                            dynamic responseDesACH = JsonConvert.DeserializeObject(responseTransaction.Result.ToString());
                            transactions = responseDesACH.ToObject<List<response.TranlogViewModel>>();
                        }

                    }
                }
                recordsTotalACH = transactions.Count > 0 ? transactions.FirstOrDefault().TotalCount : 0;
                return Json(new { draw = drawACH, recordsFiltered = recordsTotalACH, recordsTotal = recordsTotalACH, data = transactions });
             
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("Web: Program (LoadAllTransactions - GET) := ", ex.Message, " Stack Trace : " + ex.StackTrace + " Inner Exception : ", ex.InnerException)));
                return Json(new { data = transactions });
                // return Json(new { draw = drawPrgTransactions, recordsFiltered = 0, recordsTotal = 0, data = new List<TransactionViewDto>() });
            }
        }

    }
}