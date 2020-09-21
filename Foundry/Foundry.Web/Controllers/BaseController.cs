using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Foundry.Domain;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Foundry.Domain.Constants;

namespace Foundry.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        //public readonly int UserId;
        //public BaseController()
        //{
        //    UserId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower().Trim() == "sub".ToLower().Trim()).Value);

        //}        
    }
}