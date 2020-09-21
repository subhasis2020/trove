using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ActionFilters
{
    public class GenerateAntiforgeryTokenCookieForAjaxAttribute : ActionFilterAttribute
    {
        private IAntiforgery _antiForgery;
        public GenerateAntiforgeryTokenCookieForAjaxAttribute(IAntiforgery antiForgery)
        {
            _antiForgery = antiForgery;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // We can send the request token as a JavaScript-readable cookie, 
            // and Angular will use it by default.
            var tokens = _antiForgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Append(
                "XSRF-TOKEN",
                tokens.RequestToken,
                new CookieOptions { HttpOnly = false });
        }
    }
}
