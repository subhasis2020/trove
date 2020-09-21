using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel.PartnerApiModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
   public class BinDataService : IBinDataService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public BinDataService(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }


        public async Task InsertUpdateBinData(DataTable binFilelist)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {

                        await  sqlConnection.QueryAsync(SQLQueryConstants.InsertUpdateBinData, new {
                            BinFile  = binFilelist.AsTableValuedParameter("BinFile")
                    }

                        , commandType: CommandType.StoredProcedure); 
                }
                catch (Exception ec)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }


    }
}
