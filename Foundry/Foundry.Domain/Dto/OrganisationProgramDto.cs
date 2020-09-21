using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class OrganisationProgramDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramCodeId { get; set; }
        public DateTime DateAdded { get; set; }
        public string ProgramType { get; set; }
        public int OrganisationId { get; set; }
        public string strProgramId { get; set; }
        public string EncProgName { get; set; }
        public bool IsPrimaryAssociation { get; set; }

        public int AccountListCount { get; set; }
    }

    public class OrganisationProgramDBDto
    {
        public int id { get; set; }
        public int organisationId { get; set; }
        public int? programId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
