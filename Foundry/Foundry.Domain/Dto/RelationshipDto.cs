using System;

namespace Foundry.Domain.Dto
{
    public class RelationshipDto
    {
        public int id { get; set; }
        public string RelationName { get; set; }
        public string description { get; set; }
        public int? displayOrder { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
    }
}
