using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
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
using static Foundry.Domain.Constants;
using sys = Microsoft.AspNetCore.Mvc.Rendering;

namespace Foundry.Web.ViewComponents
{
    public class MerchantTransactionViewComponent : ViewComponent
    {
        public MerchantTransactionViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppId, string poN, string ppN)
        {
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            MerchnatTransactionModel orgAdmins = new MerchnatTransactionModel();
            ViewBag.CurrentPeriod = DateTime.Now.ToString("MMMM yyyy");

            var monthsBetweenDates = Enumerable.Range(0, 12)
                                       .Select(i => DateTime.Now.AddMonths(-i))
                                       .OrderByDescending(e => e.Year).Skip(1)
                                       .AsEnumerable();

            ViewBag.MonthYear = monthsBetweenDates.Select(e => e.ToString("MMMM yyyy")).ToArray();
            ViewBag.OrganisationId = id;
            await Task.FromResult(0);
            return View(orgAdmins);
        }

    }
}
