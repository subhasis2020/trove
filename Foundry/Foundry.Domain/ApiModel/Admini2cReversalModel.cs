using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
  public class Admini2cReversalModel
    {
        public decimal dsamount { get; set; }
        public int userId { get; set; }
        public string dstxnid { get; set; }
        public DateTime date { get; set; }

    }
}
