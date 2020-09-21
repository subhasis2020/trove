using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
  public  interface IFiservMethods
    {
        Task<string> fiservReverseTransaction(decimal Amount, string orderId);
    }
}
