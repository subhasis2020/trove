using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class JPOSBiteBalanceApiModel
    {
        public bool success { get; set; }
        public List<Account> accounts { get; set; }

        

    }
    public class Account
    {
        public string name { get; set; }
        public double balance { get; set; }
    }
}
