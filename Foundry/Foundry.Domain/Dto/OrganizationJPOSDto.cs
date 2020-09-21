using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class OrganizationJPOSDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string organizationId { get; set; }
        public bool active { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string province { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zip { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string country { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactTitle { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactEmail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string website { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string facebook { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string twitter { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string skype { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string description { get; set; }
    }

    public class JPOSResponse {
        public bool success { get; set; }
        public int id { get; set; }
        public string RC { get; set; }
    }
}
