using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Api.Extensions
{
    public class SwaggerAuthorizationFilter : IDocumentFilter
    {
        private IServiceProvider _provider;

        public SwaggerAuthorizationFilter(IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            this._provider = provider;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var http = this._provider.GetRequiredService<IHttpContextAccessor>();
            var authorizedIds = new[] { "00000000-1111-2222-1111-000000000000" };   // All the authorized user id's.
                                                                                    // When using this in a real application, you should store these safely using appsettings or some other method.
            var userId = http.HttpContext.User.Claims.Where(x => x.Type == "jti").Select(x => x.Value).FirstOrDefault();
            var show = http.HttpContext.User.Identity.IsAuthenticated && authorizedIds.Contains(userId);

            if (!show)
            {
                #region Hide method endpoints 
                var descriptions = context.ApiDescriptionsGroups.Items.SelectMany(group => group.Items);

                foreach (var description in descriptions)
                {
                    // Expose login so users can login through Swagger. 
                    if (description.HttpMethod == "POST" && description.RelativePath == "v1/users/login")
                        continue;

                    var route = "/" + description.RelativePath.TrimEnd('/');
                    var path = swaggerDoc.Paths[route];

                    // remove method or entire path (if there are no more methods in this path)
                    switch (description.HttpMethod)
                    {
                        case "DELETE": path.Delete = null; break;
                        case "GET": path.Get = null; break;
                        case "HEAD": path.Head = null; break;
                        case "OPTIONS": path.Options = null; break;
                        case "PATCH": path.Patch = null; break;
                        case "POST": path.Post = null; break;
                        case "PUT": path.Put = null; break;
                        default: throw new ArgumentOutOfRangeException("Method name not mapped to operation");
                    }

                    if (path.Delete == null && path.Get == null &&
                        path.Head == null && path.Options == null &&
                        path.Patch == null && path.Post == null && path.Put == null)
                        swaggerDoc.Paths.Remove(route);
                }
                #endregion

                #region Hide models 
              //  var loginRequest = "MySolution.Common.Models.LoginRequest";
               // var loginRequestModel = context.SchemaRegistry.Definitions[loginRequest];
              //  var booleanApiResult = "MySolution.Common.Results.BooleanApiResult";
              //  var booleanApiResultModel = context.SchemaRegistry.Definitions[booleanApiResult];

                //swaggerDoc.Definitions.Clear();

                //if (loginRequestModel != null)
                //{
                //    swaggerDoc.Definitions.Add(loginRequest, loginRequestModel);
                //}
                //if (booleanApiResultModel != null)
                //{
                //    swaggerDoc.Definitions.Add(booleanApiResult, booleanApiResultModel);
                //}
                #endregion
            }
        }
    }
}
