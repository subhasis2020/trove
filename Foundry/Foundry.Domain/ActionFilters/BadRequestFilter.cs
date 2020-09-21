using Foundry.Domain.ApiModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ActionFilters
{
    public class BadRequestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //store request in data base
                context.Result = new BadRequestObjectResult(new ApiBadRequestResponse(context.ModelState, "Bad request error."));
            }
        }
    }
}
