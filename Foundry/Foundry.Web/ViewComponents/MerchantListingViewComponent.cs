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

namespace Foundry.Web.ViewComponents
{
    public class MerchantListingViewComponent : ViewComponent
    {

        public MerchantListingViewComponent()
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