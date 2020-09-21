using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel.PartnerApiModel
{
    public class DisplayBenefactorModel
    {
        public int BenefactorUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RelationshipName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
