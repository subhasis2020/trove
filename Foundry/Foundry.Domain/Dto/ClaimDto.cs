using Newtonsoft.Json;

namespace Foundry.Domain.Dto
{
    public class ClaimDto
    {
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
        [JsonProperty(PropertyName = "claimType")]
        public string ClaimType { get; set; }
        [JsonProperty(PropertyName = "claimValue")]
        public string ClaimValue { get; set; }
    }
}
