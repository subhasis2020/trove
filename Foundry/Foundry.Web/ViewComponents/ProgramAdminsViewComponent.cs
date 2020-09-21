using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.ViewComponents
{
    public class ProgramAdminsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
          await  Task.FromResult(0);
            return View();
        }
    }
}
