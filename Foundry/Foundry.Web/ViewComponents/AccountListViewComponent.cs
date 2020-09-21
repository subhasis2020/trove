using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class AccountListViewComponent : ViewComponent
    {
        public AccountListViewComponent()
        {
        }
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
