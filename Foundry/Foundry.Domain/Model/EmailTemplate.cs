using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class EmailTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Ccemail { get; set; }
        public string Bccemail { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
