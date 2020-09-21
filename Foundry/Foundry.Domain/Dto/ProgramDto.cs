using System;
using System.Collections.Generic;

namespace Foundry.Domain.Dto
{
    public class ProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganisationId { get; set; }
        public string LogoPath { get; set; }
        public string ColorCode { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsProgramLinkedWithUser { get; set; }
    }

    public class ProgramDrpDto
    {
        public int id { get; set; }
        public string name { get; set; }

    }
}
