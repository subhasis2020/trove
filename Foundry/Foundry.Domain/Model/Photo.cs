using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Photo
    {
        public int Id { get; set; }
        public string PhotoPath { get; set; }
        public int? EntityId { get; set; }
        public int? PhotoType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
