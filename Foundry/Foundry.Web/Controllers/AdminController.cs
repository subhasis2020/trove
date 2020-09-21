using System;
using System.Linq;
using Foundry.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static Foundry.Domain.Constants;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class is used to conatin all the admin related methods and actions
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(CustomActionAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Thid constructor is used to inject the services.
        /// </summary>
        /// <param name="configuration"></param>
        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// This method is used to get the default action from this controller.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method is used to redirect the user to specific page based on its role.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole(Roles.SuperAdmin) || User.IsInRole(Roles.OrganizationFull) || User.IsInRole(Roles.OrganizationReporting))
            {
                ViewBag.UId = Cryptography.EncryptPlainToCipher(Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim() == "sub".ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()).Value));
                ViewBag.RId = Cryptography.EncryptPlainToCipher(User.Claims.FirstOrDefault(x => x.Type.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()).Value.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim());
                return View("FoundryDashboard");
            }
            else if (User.IsInRole(Roles.ProgramFull))
            {
                return RedirectToAction("Program", "Program");
            }
            else if (User.IsInRole(Roles.MerchantFull))
            {
                return RedirectToAction("Merchants", "Merchant");
            }
            else if (User.IsInRole(Roles.ProgramReporting) || User.IsInRole(Roles.MerchantReporting))
            {
                var result = Cryptography.EncryptPlainToCipher(Convert.ToString(HttpContext.Connection.RemoteIpAddress));
                var ReportURL = string.Concat(_configuration["ReportURL"], "dashboard.aspx?auth=", result, "|", @Cryptography.EncryptPlainToCipher(Convert.ToString(User.Claims.FirstOrDefault(x => x.Type.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim() == "sub".ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()).Value)), "|", @Cryptography.EncryptPlainToCipher(@User.Claims.FirstOrDefault(x => x.Type.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()).Value.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim()));
                return Redirect(ReportURL);
            }
            return View();
        }

        /// <summary>
        /// This is the action which returns view for the superadmin dashboard
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult FoundryDashboard()
        {
            return View();
        }
    }
}