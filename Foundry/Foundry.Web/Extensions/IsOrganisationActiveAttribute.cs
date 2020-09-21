using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Web
{
    [Authorize]
    public class IsOrganisationActiveAttribute : IAsyncActionFilter
    {
      
        public IsOrganisationActiveAttribute()
        {
           
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "AccessToken".ToLower().Trim()).Value);
            }

           await next();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // our code after action executes
        }
    }
}
