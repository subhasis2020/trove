using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
namespace Foundry.Services
{
   public  interface ITranlog
    {
         IEnumerable<TranlogViewModel> gettranslog(string id);
        IEnumerable<TranlogViewModel> getUserLoyaltyRewardTransactions(string id, int pagenumber, int pagelength);
        IEnumerable<TranlogViewModel> getUserBitePayTransactions(string id, int pagenumber, int pagelength);

    }
}
