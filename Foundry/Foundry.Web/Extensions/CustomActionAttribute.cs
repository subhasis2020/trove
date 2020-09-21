using Foundry.Domain;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Web
{
    [Authorize]
    public class CustomActionAttribute : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;
        public CustomActionAttribute(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var tokenExpiration = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "TokenExpiresIn".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
            var refreshTokenIdentity = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "TokenRefreshIdentity".ToLower(CultureInfo.InvariantCulture).Trim()).Value;

            if (DateTime.UtcNow > Convert.ToDateTime(tokenExpiration))
            {
                using (var client = new HttpClient())
                {// request token

                    var disco = await client.GetDiscoveryDocumentAsync(_configuration["ServiceAPIURL"]);
                    if (disco.IsError) { new InvalidOperationException("GetDiscoveryDocumentAsync method is giving error"); }
                    var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        Address = _configuration["ServiceAPIURL"] + ApiConstants.GenerateUserToken,
                        GrantType = "refresh_token",
                        ClientId = "ro.angular",
                        ClientSecret = "secret",
                        RefreshToken = refreshTokenIdentity
                    });
                    if (response.IsError)
                    {
                        await context.HttpContext.SignOutAsync();
                    }
                    else
                    {
                        // create a new identity from the old one
                        var identity = new ClaimsIdentity(context.HttpContext.User.Identity);

                        // update claim value
                        identity.RemoveClaim(identity.FindFirst("AccessToken"));
                        identity.AddClaim(new Claim("AccessToken", response.AccessToken));
                        identity.RemoveClaim(identity.FindFirst("TokenExpiresIn"));
                        identity.AddClaim(new Claim("TokenExpiresIn", DateTime.UtcNow.AddSeconds(Convert.ToInt32(response.ExpiresIn.ToString())).ToString()));
                        identity.RemoveClaim(identity.FindFirst("TokenRefreshIdentity"));
                        identity.AddClaim(new Claim("TokenRefreshIdentity", response.RefreshToken.ToString()));
                        var principal = new ClaimsPrincipal(identity);

                        var props = new AuthenticationProperties();
                        props.IsPersistent = true;

                        context.HttpContext.SignInAsync(
                      CookieAuthenticationDefaults.
          AuthenticationScheme,
                      principal,
                      props).Wait();
                    }
                }
            }

            await next();
        }


    }
}
