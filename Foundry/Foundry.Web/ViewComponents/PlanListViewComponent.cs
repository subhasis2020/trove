using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Domain.Dto;

namespace Foundry.Web.ViewComponents
{
    public class PlanListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string poId, string ppId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgId = poId;
            ViewBag.PrimaryProgId = ppId;
            ViewBag.PrimaryProgName = ppN;
            ViewBag.PrimaryOrgName = poN;
            await Task.FromResult(0);
            return View();
        }
    }
}
