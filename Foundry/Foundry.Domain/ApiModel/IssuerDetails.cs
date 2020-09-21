using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Issuerprop
    {
        public string website { get; set; }
        public string contactEmail { get; set; }
        public string contactName { get; set; }
        public string programName { get; set; }
        public string contactNumber { get; set; }
        public string programColor { get; set; }
    }

    public class Issuer
    {
        public string id { get; set; }
        public string institutionId { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string tz { get; set; }
        public string destinationStation { get; set; }
        public string programName { get; set; }
        public string programColor { get; set; }
        public string organizationId { get; set; }
        public string organizationName { get; set; }
        //public Issuerprop issuerprop { get; set; }
        public Dictionary<string, string> issuerprop { get; set; }
    }

    public class IssuerDetails
    {
        public bool success { get; set; }
        public Issuer issuer { get; set; }
    }
    public class Issuers
    {
        public bool success { get; set; }
        public List<Issuer> issuer { get; set; }
    }

}
