using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class GeneralSetting
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string Value { get; set; }
        public string KeyGroup { get; set; }
    }
}
