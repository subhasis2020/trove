using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Foundry.Web.Models;

namespace Foundry.Web.Controllers
{
    /// <summary>
    /// This class is the default class.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// This is the default method for the home class.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }               
        /// <summary>
        /// This method is given for privacy setting but this is not used.
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// This method is used to contain the method for error.
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }       
        /// <summary>
        /// This method is used to import the functionality
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Import()
        {
            ViewBag.Url = "/Import/demo1.xlsx";
            return View("Import");
        }

    }
}
