using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class BusinessTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
