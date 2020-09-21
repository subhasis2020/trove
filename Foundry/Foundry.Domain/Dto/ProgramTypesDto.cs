using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramTypesDto
    {
        public int Id { get; set; }
        public string ProgramTypeName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
