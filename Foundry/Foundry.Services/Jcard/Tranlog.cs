using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using Foundry.Domain.Dto;
using Foundry.Domain;
using Dapper;
using Npgsql;

namespace Foundry.Services
{
   public class Tranlog : FoundryRepositoryBase<OrgLoyalityGlobalSettings>, ITranlog
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public Tranlog(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)  : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;

        }

     

        public IEnumerable<TranlogViewModel> gettranslog(string id)
        {
            
            // Specify connection options and open an connection
            NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSqlDb"));
            conn.Open();
          //  id = "0b592fc3-7829-4e39-b48b-e0c71cf75665";
            // Define a query
            // NpgsqlCommand cmd = new NpgsqlCommand("Select t.id, c.realid,t.loyalty,t.dsamount,t.irc,t.itc,t.date from public.tranlog t inner join public.cardholder c on t.cardholder = c.id where c.realId= "+id +"and t.date="+DateTime.Now, conn);
             NpgsqlCommand cmd = new NpgsqlCommand("select *,2000 - (sumamt ) total from (Select sum(abs(t.dsamount)) sumamt, c.realid from public.tranlog t inner join public.cardholder c on t.cardholder  = c.id where Date(t.date)= current_date  and c.realid=" + "'" + id + "'"+ " group by c.realid)b", conn);
           // NpgsqlCommand cmd = new NpgsqlCommand("select *,2000 - (sumamt) total from (Select sum(abs(t.dsamount)) sumamt, c.realid from public.tranlog t inner join public.cardholder c on t.cardholder  = c.id where  Date(t.date)= current_date group by c.realid)b join(select realid, dsbalance from tranlog join (Select max(t.id) mid, c.realid from public.tranlog t inner join public.cardholder c on t.cardholder  = c.id where  Date(t.date)= current_date group by c.realid)a on a.mid=id)c on c.realid=b.realid where c.realid=" + "'" +id +"'",conn);
            // Execute a query
            NpgsqlDataReader dr = cmd.ExecuteReader();
            var detail = new List<TranlogViewModel>();
            // Read all rows 
            while (dr.Read())
            {
                TranlogViewModel model = new TranlogViewModel();
                model.realid = dr["realid"].ToString();
                model.loyalty = dr["sumamt"].ToString();
               // if (!(dr["dsbalance"] is DBNull))
                   // model.dsbalance = Convert.ToDecimal(dr["dsbalance"]);
              //  else
                  //  model.dsbalance = 0;
                //if (!(dr["dsamount"] is DBNull))
                //    model.dsamount = Convert.ToDecimal(dr["dsamount"]);
                //else
                //    model.dsamount = 0;
                             
              //  model.irc = dr["irc"].ToString();
                model.total = dr["total"].ToString();
              //  model.date = Convert.ToDateTime(dr["date"]);
                detail.Add(model);
            }    

            // Close connection
            conn.Close();
            return detail ;
        }

        public IEnumerable<TranlogViewModel> getUserLoyaltyRewardTransactions(string id,int pagenumber, int pagelength)
        {
            int offset = 0;
            if (pagenumber != 1)
            {
                offset = (pagenumber - 1) * pagelength;
            }
            // Specify connection options and open an connection
            NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSqlDb"));
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select COUNT(*) OVER() AS TotalCount, t.id, c.realid,t.loyalty,t.dsamount,t.dsbalance,t.irc,t.itc,t.date ,CASE WHEN t.itc='wcredit' THEN t.loyaltyamount ELSE 0 end as CreditAmount, CASE WHEN t.itc<>'wcredit' THEN t.loyaltyamount ELSE 0 end DebitAmount from public.tranlog t inner join public.cardholder c on t.cardholder = c.id where t.loyaltyamount is not null and   c.realId=" + "'" + id + "'" + " order by id desc OFFSET " + offset +" LIMIT " + pagelength, conn);
            // Execute a query
            NpgsqlDataReader dr = cmd.ExecuteReader();
            var detail = new List<TranlogViewModel>();
            // Read all rows 
            while (dr.Read())
            {
                TranlogViewModel model = new TranlogViewModel();
                model.realid = dr["realid"].ToString();            
                if (!(dr["dsbalance"] is DBNull))
                    model.dsbalance = Convert.ToDecimal(dr["dsbalance"]);
                else
                    model.dsbalance = 0;
                if (!(dr["dsamount"] is DBNull))
                    model.dsamount = Convert.ToDecimal(dr["dsamount"]);
                else
                    model.dsamount = 0;

                model.id =Convert.ToInt64( dr["id"]);
                model.TotalCount = Convert.ToInt32(dr["TotalCount"]);
                model.date = Convert.ToDateTime(dr["date"]);

                model.CreditAmount = Convert.ToDecimal(dr["CreditAmount"]);
                model.DebitAmount = Convert.ToDecimal(dr["DebitAmount"]);
               

                detail.Add(model);
            }

            // Close connection
            conn.Close();
            return detail;
        }

        public IEnumerable<TranlogViewModel> getUserBitePayTransactions(string id, int pagenumber,int pagelength)
        {
            int offset = 0;
            if(pagenumber!=1)
            {
                offset=(pagenumber -1)*pagelength;
            }
            // Specify connection options and open an connection
            NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSqlDb"));
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select COUNT(*) OVER() AS TotalCount, t.id, c.realid,t.loyalty,t.dsamount,t.dsbalance,t.irc,t.itc,t.date , t.dstxnid,t.ds,t.dsrc,t.dsresponse from public.tranlog t inner join public.cardholder c on t.cardholder = c.id where c.realId=" + "'" + id + "'" + " order by id desc OFFSET " + offset + " LIMIT " + pagelength, conn);
            // Execute a query
            NpgsqlDataReader dr = cmd.ExecuteReader();
            var detail = new List<TranlogViewModel>();
            // Read all rows 
            while (dr.Read())
            {
                TranlogViewModel model = new TranlogViewModel();
                model.realid = dr["realid"].ToString();
                if (!(dr["dsbalance"] is DBNull))
                    model.dsbalance = Convert.ToDecimal(dr["dsbalance"]);
                else
                    model.dsbalance = 0;
                if (!(dr["dsamount"] is DBNull))
                    model.dsamount = Convert.ToDecimal(dr["dsamount"]);
                else
                    model.dsamount = 0;

                model.id = Convert.ToInt64(dr["id"]);
                model.TotalCount = Convert.ToInt32(dr["TotalCount"]);
                model.date = Convert.ToDateTime(dr["date"]);
                model.dstxnid = dr["dstxnid"].ToString();
                model.ds = dr["ds"].ToString();
                model.dsrc = dr["dsrc"].ToString();
                model.dsresponse = dr["dsresponse"].ToString();
                detail.Add(model);
            }

            // Close connection
            conn.Close();
            return detail;
        }

    }
}
