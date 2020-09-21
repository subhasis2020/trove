using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Api.Attributes
{
    /// <summary>
    /// This attribute is checking the organisation is active or not before calling the API.
    /// </summary>
    [Authorize]
    public class IsOrganisationActiveAttribute : IAsyncActionFilter
    {
        private readonly IOrganisation _organisation;
        /// <summary>
        /// This constructor is used to inject the dependancy for specified services.
        /// </summary>
        /// <param name="organisation"></param>
        public IsOrganisationActiveAttribute(IOrganisation organisation)
        {
            _organisation = organisation;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userRoleCheckForAction = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower().Trim());
            if (userRoleCheckForAction != null && userRoleCheckForAction.Value.ToLower().Trim() == "Basic User".ToLower().Trim())
            {
                var organisationIdClaim = Convert.ToInt32(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "organistionId".ToLower().Trim()).Value);
                var orgDetail = await _organisation.GetOrganisationDetailsById(organisationIdClaim);
                if (!orgDetail.isActive.Value && !orgDetail.isDeleted.Value)
                {
                    context.Result = new OkObjectResult(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, "Your organisation does not exist.", null, 0, "", false, false));
                    return;
                }
                if (!orgDetail.isActive.Value && orgDetail.isDeleted.Value)
                {
                    context.Result = new OkObjectResult(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, "Organisation is inactive.", null, 0, "", false, false));
                    return;
                }
            }
             await next();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // our code after action executes
        }
    }
}
