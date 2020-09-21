using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    [Serializable]
    [DataContract]
    public class ApiResponse
    {
        [DataMember(Name = "result")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; }
        [DataMember(Name = "statusCode")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int StatusCode { get; }
        [DataMember(Name = "status")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Status { get; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "message")]
        public string Message { get; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "statusFlagNum")]
        public int StatusFlagNum { get; }
        [DataMember(Name = "customResult")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomResult { get; set; } = "";
        [DataMember(Name = "customResult1")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomResult1 { get; set; } = "";
        [DataMember(Name = "sessionStatus")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool SessionStatus { get; }
        [DataMember(Name = "organisationStatus")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool OrganisationStatus { get; set; } = true;
        public ApiResponse(int statusCode, bool succeeded, string message = null, object result = null, int statusFlagNum = 0, string customData = "", bool sessionStatus = false, bool organisationStatus = true, string customeResult1 = "")
        {
            StatusCode = statusCode;
            Status = succeeded;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Result = result;
            StatusFlagNum = statusFlagNum;
            CustomResult = customData;
            SessionStatus = sessionStatus;
            OrganisationStatus = organisationStatus;
            CustomResult1 = customeResult1;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return "Resource not found";
                case 500:
                    return "An unhandled error occurred";
                default:
                    return null;
            }
        }
    }

    public class ApiOkResponse : ApiResponse
    {
        public ApiOkResponse(object result, string message = "Data is successfully returned.")
     : base(200, true, message)
        {

        }
    }

    public class ApiBadRequestResponse : ApiResponse
    {
        public object result { get; }

        public ApiBadRequestResponse(ModelStateDictionary modelState, string message = "Bad Request Error.")
            : base(400, false, message)
        {
            result = modelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage).ToArray();
        }
    }

    public class ResultCheck
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string userName { get; set; }
        public object sessionId { get; set; }
        public string phoneNumber { get; set; }
        public object address { get; set; }
        public string imagePath { get; set; }
        public bool emailConfirmed { get; set; }
        public bool phoneNumberConfirmed { get; set; }
        public int programCode { get; set; }
        public string programImagePath { get; set; }
        public object programName { get; set; }
    }
}
