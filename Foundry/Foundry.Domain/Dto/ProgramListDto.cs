using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramListDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramCodeId { get; set; }
        public DateTime DateAdded { get; set; }
        public string ProgramType { get; set; }
        public int OrganisationId { get; set; }
        public string strProgramId { get; set; }
        public string EncProgName { get; set; }
        public string OrganisationEncId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationSubTitle { get; set; }
        public string EncOrganisationName { get; set; }
        public int OrgPrgLinkCount { get; set; }
        public string JPOS_IssuerId { get; set; }
    }
}
