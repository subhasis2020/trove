using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class AdminMerchantsViewComponent : ViewComponent
    {
        public AdminMerchantsViewComponent()
        {
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.FromResult(0);
            return View();
        }
    }
}
