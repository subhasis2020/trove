using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
   public interface IBinDataService
    {
        Task InsertUpdateBinData(DataTable binFilelist);
    }



   
}
