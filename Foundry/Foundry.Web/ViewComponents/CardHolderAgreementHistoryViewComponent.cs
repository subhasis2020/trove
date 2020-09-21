using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class CardHolderAgreementHistoryViewComponent : ViewComponent
    {
        public CardHolderAgreementHistoryViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string poId, string ppN, string poN)
        {
            ViewBag.PrimaryOrgName = poN;
            ViewBag.PrimaryProgName = ppN;
            await Task.FromResult(0);
            return View();
        }
    }
}
