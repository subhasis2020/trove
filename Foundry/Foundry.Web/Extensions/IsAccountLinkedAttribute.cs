using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Web
{
    public class IsAccountLinkedAttribute : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly string Arguments;

        public IsAccountLinkedAttribute(string argument, IConfiguration configuration)
        {
            this.Arguments = argument;
            this._configuration = configuration;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim() == "Benefactor".ToLower(CultureInfo.InvariantCulture).Trim())
            {
                object param;
                var islink = false;
                if (context.ActionArguments.TryGetValue(Arguments, out param)) { 
                    islink = Convert.ToBoolean(param);
                }

                var benefactorId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sub".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.SetBearerToken(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "AccessToken".ToLower(CultureInfo.InvariantCulture).Trim()).Value);

                    var result = client.GetAsync(_configuration["ServiceAPIURL"] + ApiConstants.LinkedUsers + "?benefactorId=" + Convert.ToInt32(benefactorId)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var response = result.Content.ReadAsAsync<ApiResponse>();
                        if (response.Result.StatusFlagNum == 0 && !islink)
                        {
                            context.HttpContext.Response.Redirect(_configuration["WebURL"] + "Benefactor/Dashboard?linked=0");
                        }

                    }

                }
            }
            await next();
        }
    }
}
