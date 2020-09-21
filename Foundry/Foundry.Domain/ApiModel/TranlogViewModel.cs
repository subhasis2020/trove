using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
   public class TranlogViewModel
    {
        public long id { get; set; }
        public string loyalty { get; set; }
        public string realid { get; set; }
        public decimal dsamount { get; set; }
        public decimal dsbalance { get; set; }
        public string irc { get; set; }
        public string itc { get; set; }
        public DateTime date { get; set; }
        public string total { get; set; }
        public int TotalCount { get; set; }
        public string dstxnid { get; set; }
        public string ds { get; set; }
        public string dsrc { get; set; }
        public string dsresponse { get; set; }
        public  decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }

    }
}
