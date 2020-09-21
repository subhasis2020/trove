using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class OrganisationMerchantTerminalModel
    {
        public int id { get; set; }
        public string terminalId { get; set; }
        public string terminalName { get; set; }
        public int? terminalType { get; set; }
        public int organisationId { get; set; }
        public string Jpos_TerminalId { get; set; }
        public string Jpos_TerminalEncId { get; set; }
    }
}
