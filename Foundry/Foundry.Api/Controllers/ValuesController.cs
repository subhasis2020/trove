using System.Collections.Generic;
using System.Threading.Tasks;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.LogService;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILoggerManager _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ValuesController(ILoggerManager logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This is default.
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _logger.LogInfo("Here is info message from our values controller.");
            _logger.LogDebug("Here is debug message from our values controller.");
            _logger.LogWarn("Here is warn message from our values controller.");
            _logger.LogError("Here is error message from our values controller.");

            return new string[] { "value1", "value2" };
        }
        /// <summary>
        /// This is default.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// This is default.
        /// </summary>
        /// <param name="value"></param>
        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            /*Empty sample API.*/
        }

        /// <summary>
        /// This is default.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            /*Empty sample API.*/
        }

        /// <summary>
        /// This is default.
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            /*Empty sample API.*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("GetJson")]
        [HttpGet]
        public async Task<IActionResult> GetDummyStatusJson() {
            await Task.FromResult(0);
            object chkResponse = @"[""test"" = 123]";
          //  return Ok(chkResponse);
            return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, JsonConvert.SerializeObject(chkResponse)));           
        } 
    }
}
