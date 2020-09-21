using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class TransactionsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppN, string poN)
        {
            await Task.FromResult(0);
            ViewBag.CurrentPeriod = DateTime.Now.ToString("MMMM yyyy");
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            var monthsBetweenDates = Enumerable.Range(0, 12)
                                       .Select(i => DateTime.Now.AddMonths(-i))
                                       .OrderByDescending(e => e.Year).Skip(1)
                                       .AsEnumerable();

            ViewBag.MonthYear = monthsBetweenDates.Select(e => e.ToString("MMMM yyyy")).ToArray();
            return View();
        }
    }
}
