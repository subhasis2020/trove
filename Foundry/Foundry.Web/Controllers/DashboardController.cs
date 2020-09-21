using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class is used to contain classes for dashboard.
    /// </summary>
    [ServiceFilter(typeof(CustomActionAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class DashboardController : Controller
    {
        /// <summary>
        /// This method is the default action method for the controller.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method is used to return the dashboard view to the user.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}