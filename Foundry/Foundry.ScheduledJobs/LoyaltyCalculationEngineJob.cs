using log4net;
using Npgsql;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Foundry.Domain.ApiModel;
using System.Data.SqlClient;
using System.Data;
using Foundry.Services;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Foundry.ScheduledJobs
{
    [DisallowConcurrentExecution]
    public class LoyaltyCalculationEngineJob  : IJob
    {
         private static readonly ILog logger = LogManager.GetLogger("LoyaltyCalculationEngineJob");
        // private static string path = @"D:\admin\MyTest.txt";
        private static string path = "";
        private string consumerId = "";
        private string version = "";
        private string url = "";
        private string n = "";
        //private readonly IUserRepository _userRepository = new IUserRepository() ;
        // private readonly ISharedJPOSService _sharedJPOSService;
        //public LoyaltyCalculationEngineJob(ISharedJPOSService sharedJPOSService)
        // {
        //     _sharedJPOSService = sharedJPOSService;
        // }


        public Task Execute(IJobExecutionContext context)
        {
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();

            path = ConfigurationData.GetValue<string>("BiteLogOutputDirectory") + "\\" + DateTime.Now.ToString("yyyy-MM-dd")+ ".txt";

        Console.WriteLine(DateTime.Now.ToString());
            logger.Info("Error while getting Running  ScheduleOfferNRewards API ");
            create();

            gettranlog(); 

            return Task.CompletedTask;
        }


        private void gettranlog()
        {
            string maxtranid = GetMaxTranlogid();

            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();


            // Specify connection options and open an connection
            NpgsqlConnection conn = new NpgsqlConnection(Configuration.GetConnectionString("PostgreSqlDb"));

            conn.Open();


             NpgsqlCommand cmd = new NpgsqlCommand("SELECT  t.id, c.realid, t.dstxnid,  t.loyalty, t.dsamount, t.dsbalance, t.irc,  t.itc, t.date ,t.Pcode from public.tranlog t inner join public.cardholder c on t.cardholder = c.id where t.irc = '0000' and t.loyalty = '1' and t.voidcount < '1' and t.reversalcount < '1' AND t.itc in  ('sale' ,'refund' ,'capture')  AND  t.id >" + maxtranid + " order by t.id ", conn);
          ///  NpgsqlCommand cmd = new NpgsqlCommand("SELECT  t.id, c.realid, t.dstxnid,  t.loyalty, t.dsamount, t.dsbalance, t.irc,  t.itc, t.date, t.Pcode from public.tranlog t inner join public.cardholder c on t.cardholder = c.id where t.irc = '0000' and t.loyalty = '1' AND t.itc in  ('sale' ,'refund' ,'capture')  AND  t.id >" + maxtranid + " order by t.id ", conn);


            // Execute a query
            NpgsqlDataReader dr = cmd.ExecuteReader();
            var detail = new List<TranlogViewModel>();
            // Read all rows 
            while (dr.Read())
            {
                TranlogViewModel model = new TranlogViewModel();
                model.id = Convert.ToInt64(dr["id"]);
                model.realid = dr["realid"].ToString();
                model.loyalty = dr["loyalty"].ToString();
                if (!(dr["dsbalance"] is DBNull))
                    model.dsbalance = Convert.ToDecimal(dr["dsbalance"]);
                else
                    model.dsbalance = 0;
                if (!(dr["dsamount"] is DBNull))
                    model.dsamount = Convert.ToDecimal(dr["dsamount"]);
                else
                    model.dsamount = 0;

                model.irc = dr["irc"].ToString();
                model.itc = dr["itc"].ToString();
                model.date = Convert.ToDateTime(dr["date"]);
                var usertype = "";

                if (dr["Pcode"].ToString() == "credit")
                {
                     usertype = GetUserType(dr["realid"].ToString());
                }
                else
                {
                    usertype = "regular";
                }
                 using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(dr["id"].ToString() +" : " + dr["realid"].ToString());
                    sw.WriteLine(usertype +" : "+dr["Pcode"].ToString());
                }
                InserAndCalculateUserLoyaltyPoints(model.realid, dr["dstxnid"].ToString(), model.dsamount, model.date.ToString(), model.id, usertype);
                detail.Add(model);
            }
           
            // Close connection
            conn.Close();
            conn.Dispose();
        }

        private  string InserAndCalculateUserLoyaltyPoints(string realid,string transactionId, decimal transactionAmount, string transactionDate, long TranlogID, string usertype)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"dbo.InserAndCalculateUserLoyaltyPoints";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@PartnerUserId", realid));
                        cmd.Parameters.Add(new SqlParameter("@transactionId", transactionId));
                        cmd.Parameters.Add(new SqlParameter("@transactionAmount", transactionAmount));
                        cmd.Parameters.Add(new SqlParameter("@transactionDate", transactionDate));
                        cmd.Parameters.Add(new SqlParameter("@TranlogID", TranlogID));
                        cmd.Parameters.Add(new SqlParameter("@usertype", usertype));
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        string pointsEarned = result.Rows[0][0].ToString();
                        string isThresholdReached = result.Rows[0][1].ToString();
                        string totalPoints = result.Rows[0][2].ToString();
                        string globalReward = result.Rows[0][3].ToString();
                        string loyalityThreshhold = result.Rows[0][4].ToString();
                        string PercentProgress = result.Rows[0][5].ToString();
                        string leftOverPoints = result.Rows[0][6].ToString();
                        string isFirsttran = result.Rows[0][7].ToString();
                        int UserID =Convert.ToInt16( result.Rows[0][8].ToString());
                        //sendbitePointsCreditedNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, PercentProgress+'%');
                        string isactive = GetBitePushUrl(ConstantsBitenotification.Is2ndUrlActiveForBite);

                        if (isFirsttran == "True")
                        {

                            sendbite1stTranNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, globalReward, leftOverPoints, UserID , ConstantsBitenotification.SodexoBiteBaseUrl);
                            if(isactive=="1")
                            {
                        
                               sendbite1stTranNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, globalReward, leftOverPoints, UserID, ConstantsBitenotification.SecondSodexoBiteBaseUrl);
                            }

                        }

                        if (isThresholdReached == "True")
                        {
                            sendJposWlletsCredit(realid, globalReward);
                            sendbiteRewardIssuedNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, globalReward, leftOverPoints, UserID, ConstantsBitenotification.SodexoBiteBaseUrl);
                            if (isactive == "1")
                            {
                                sendbiteRewardIssuedNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, globalReward, leftOverPoints, UserID, ConstantsBitenotification.SecondSodexoBiteBaseUrl);
                            }
                        }
                        else
                        {
                            sendbitePointsCreditedNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, PercentProgress + '%', UserID, ConstantsBitenotification.SodexoBiteBaseUrl);
                            if (isactive == "1")
                            {
                                sendbitePointsCreditedNotificaion(realid, transactionAmount.ToString(), pointsEarned, totalPoints, loyalityThreshhold, PercentProgress + '%', UserID, ConstantsBitenotification.SecondSodexoBiteBaseUrl);
                            }
                        }
                        return result.Rows[0][0].ToString();
                        

                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error  : InserAndCalculateUserLoyaltyPoints :" + ex.Message.ToString());
                }
            }
            return "0";
        }

        private void sendJposWlletsCredit(string id, string globalReward)
        {

            GetjposGeneralSetting();
            string ApiUrl = url + "wallets/"+ id + "/credit";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
            request.Method = "POST";
            request.Headers.Add("version", version);
            request.Headers.Add("consumer-id", consumerId);
            request.Headers.Add("nonce", n);
            request.ContentType = "application/json";
 
            string json;
            

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                //json = "{ \"UserObjectId\": \"" + id + "\", \"Date\": \"" + DateTime.Now.ToString("yyyy-MM-dd") + "\", \"Time\": \"" + DateTime.Now.ToString("HH:mm:ss")
                //   + "\", \"RewardCredited\": \"" + globalReward + "\", \"PreviousPointsBalance\": \"" + PointsBalance + "\", \"PointsDebited\": \"" + PointsThreshold + "\", \"PointsBalance\": \"" + leftOverPoints + "\"   } ";
                json = "{  \"amount\": " + globalReward + ", \"currency\": \"840\",\"description\": \"loyalty\",\"rrn\": \"548715406860\", \"type\": \"loyalty\" } ";


                streamWriter.Write(json);
            }
            string JposRequestCreate ;
            JposRequestCreate ="Headers:{version:" + version + ",consumer-id:" + consumerId + ",nonce:" + n + "},URL:{" + ApiUrl + "}Body:{" + json + "}" ;
            HttpWebResponse response = null;

            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var rescode = response.StatusCode.ToString();
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Jpos :" + rescode + "  " + responseString);
                    }
                    InsertJPOSCallLog(ApiUrl, JposRequestCreate, responseString, true);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error : sendJposWlletsCredit :" + ex.Message.ToString());
                    InsertJPOSCallLog(ApiUrl, JposRequestCreate,  ex.Message.ToString(), false);
                }
            }
            //            return access_token;
        }


        private void sendbiteRewardIssuedNotificaion(string id, string amount, string PointsAdded, string PointsBalance, string PointsThreshold, string globalReward ,string leftOverPoints, int UserID,string Key)
        {

            // IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            // string[] getUserNamePwd = DbFunction.GetUserNamePassword();
            // string ApiUrl = ConfigurationData.GetValue<string>("BiteNotifictionBasicUrl") + ConstantsBitenotification.RewardIssued;
            string ApiUrl = GetBitePushUrl(Key) + ConstantsBitenotification.RewardIssued;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
            request.Method = "POST";
            request.ContentType = "application/json";

            string json;


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                json = "{ \"UserObjectId\": \"" + id + "\", \"Date\": \"" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "\", \"Time\": \"" + DateTime.UtcNow.ToString("HH:mm:ss")
                   + "\", \"RewardCredited\": \"" + globalReward + "\", \"PreviousPointsBalance\": \"" + PointsBalance + "\", \"PointsDebited\": \"" + PointsThreshold + "\", \"PointsBalance\": \"" + leftOverPoints + "\"   } ";

                streamWriter.Write(json);
            }
            HttpWebResponse response = null;


            try
            { using (response = (HttpWebResponse)request.GetResponse())
                {

                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var rescode = response.StatusCode.ToString();
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Notification :" + rescode);
                    }
                    InsertPartnerNotificationsLog("RewardIssued", ApiUrl, json, responseString, rescode, UserID);
                }
            }


            //try
            //{
            //    response = (HttpWebResponse)request.GetResponse();
            //    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //    var rescode = response.StatusCode.ToString();
            //    using (StreamWriter sw = File.AppendText(path))
            //    {
            //        sw.WriteLine("Notification :" + rescode);
            //    }
            //    InsertPartnerNotificationsLog("RewardIssued", ApiUrl, json, responseString, rescode, UserID);

            //}
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error :sendbiteRewardIssuedNotificaion :" + ex.Message.ToString());
                    InsertPartnerNotificationsLog("RewardIssued", ApiUrl, json, ex.Message.ToString(), "Error", UserID);
                }
            }
        
        }


        private void sendbite1stTranNotificaion(string id, string amount, string PointsAdded, string PointsBalance, string PointsThreshold, string globalReward, string leftOverPoints, int UserID,string Key)
        {

            // IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            // string[] getUserNamePwd = DbFunction.GetUserNamePassword();
            // string ApiUrl = ConfigurationData.GetValue<string>("BiteNotifictionBasicUrl") + ConstantsBitenotification.RewardIssued;
            string ApiUrl = GetBitePushUrl(Key) + ConstantsBitenotification.FirstTransactionBonus;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
            request.Method = "POST";
            request.ContentType = "application/json";

            string json;


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                json = "{ \"UserObjectId\": \"" + id + "\", \"Date\": \"" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "\", \"Time\": \"" + DateTime.UtcNow.ToString("HH:mm:ss")
                   + "\", \"RewardCredited\": \"" + globalReward + "\", \"PreviousPointsBalance\": \"" + PointsBalance + "\", \"PointsDebited\": \"" + PointsThreshold + "\", \"PointsBalance\": \"" + leftOverPoints + "\"   } ";

                streamWriter.Write(json);
            }
            HttpWebResponse response = null;

            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var rescode = response.StatusCode.ToString();
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Notification :" + rescode);
                    }
                    InsertPartnerNotificationsLog("1stTransactionBonus", ApiUrl, json, responseString, rescode, UserID);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error :sendbiteRewardIssuedNotificaion :" + ex.Message.ToString());
                    InsertPartnerNotificationsLog("1stTransactionBonus", ApiUrl, json, ex.Message.ToString(), "Error", UserID);
                }
            }
            //            return access_token;
        }



        private string InsertJPOSCallLog(string entity, string requestJPOSContent, string responseJPOSContent,  bool Status)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"INSERT INTO dbo.JPOSCallLog ( entity , httpMethod , requestJPOSContent , responseJPOSContent , responseStatus , requestDateTime , responseDateTime ) VALUES  ( @entity , @httpMethod , @requestJPOSContent ,@responseJPOSContent ,@responseStatus ,GETUTCDATE() ,GETUTCDATE())";
              //  string sql = @"INSERT INTO dbo.JPOSCallLog ( entity , httpMethod , requestJPOSContent , responseJPOSContent  , requestDateTime , responseDateTime ) VALUES  ( @entity , 'Post' , @requestJPOSContent ,@responseJPOSContent  ,GETDATE() ,GETDATE())";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@entity", entity));
                        cmd.Parameters.Add(new SqlParameter("@httpMethod", "Post"));
                        cmd.Parameters.Add(new SqlParameter("@requestJPOSContent", requestJPOSContent));
                        cmd.Parameters.Add(new SqlParameter("@responseJPOSContent", responseJPOSContent));
                        cmd.Parameters.Add(new SqlParameter("@responseStatus", Status));
                    
              
                        cmd.ExecuteNonQuery();
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        //dataAdapter.Fill(result);
                        //return result.Rows[0][0].ToString();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error :InsertJPOSCallLog :" + ex.Message.ToString());
                }
            }
            return "0";
        }





        private void sendbitePointsCreditedNotificaion( string  id , string amount, string PointsAdded, string PointsBalance ,string PointsThreshold, string PercentProgress, int UserID,string Key)
        {
           
            //IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            string[] getUserNamePwd = DbFunction.GetUserNamePassword();
            // string ApiUrl = ConfigurationData.GetValue<string>("BiteNotifictionBasicUrl") + ConstantsBitenotification.PointsCredited;
            string ApiUrl = GetBitePushUrl(Key) + ConstantsBitenotification.PointsCredited;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
            request.Method = "POST";
            request.ContentType = "application/json";

            string json;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                json = "{ \"UserObjectId\": \"" + id + "\", \"TransactionAmount\": \"" + amount + "\", \"PointsAdded\": \"" + PointsAdded
                   + "\", \"PointsBalance\": \"" + PointsBalance + "\", \"PointsThreshold\": \"" + PointsThreshold + "\", \"PercentProgress\": \"" + PercentProgress + "\"   } ";

                streamWriter.Write(json);
            }
            HttpWebResponse response = null;
           
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var rescode = response.StatusCode.ToString();
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Notification :" + rescode);
                    }
                    InsertPartnerNotificationsLog("PointsCredited", ApiUrl, json, responseString, rescode, UserID);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error : sendbitePointsCreditedNotificaion :" + ex.Message.ToString());
                    InsertPartnerNotificationsLog("PointsCredited", ApiUrl, json, ex.Message.ToString(), "Error" , UserID);
                }
            }
            //            return access_token;
        }

        private  string InsertPartnerNotificationsLog( string ApiName , string ApiUrl , string Request, string Response , string Status,int UserID)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"INSERT INTO dbo.PartnerNotificationsLog (UserId ,ApiName ,ApiUrl ,Request ,Response ,Status ,CreatedDate ,UpdatedDate) VALUES  ( @UserId,@ApiName ,@ApiUrl ,@Request, @Response ,@Status  ,GETUTCDATE() , GETUTCDATE()  )";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                       
                        cmd.CommandType = CommandType.Text;
                        
                        cmd.Parameters.Add(new SqlParameter("@UserId", UserID));
                        cmd.Parameters.Add(new SqlParameter("@ApiName", ApiName));
                        cmd.Parameters.Add(new SqlParameter("@ApiUrl", ApiUrl));
                        cmd.Parameters.Add(new SqlParameter("@Request", Request));
                        cmd.Parameters.Add(new SqlParameter("@Response", Response));
                        cmd.Parameters.Add(new SqlParameter("@Status", Status));
                        
                        cmd.ExecuteNonQuery();
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        //dataAdapter.Fill(result);
                        //return result.Rows[0][0].ToString();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error : InsertPartnerNotificationsLog:" + ex.Message.ToString());
                }
            }
            return "0";
        }

        private static string GetUserType( string realid)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"dbo.GetSodexhoUserTypebyPartnerUserId";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@PartnerId", 1));
                        cmd.Parameters.Add(new SqlParameter("@PartnerUserId", realid));
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        return result.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error :GetUserType:" + ex.Message.ToString());
                }
            }
            return "0";
        }


        private static string GetMaxTranlogid()
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"SELECT ISNULL( MAX(TranlogID),0) id FROM dbo.UserLoyaltyPointsHistory";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        return result.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error : GetMaxTranlogid :" + ex.Message.ToString());
                }
            }
            return "0";
        }



        private static string GetBitePushUrl( string  key)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                key = key.Trim();
                string sql = @"SELECT Value FROM  dbo.GeneralSetting WHERE keyGroup ='SodexoBiteWebHooks' AND KeyName='"+ key + "'";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        return result.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error :GetBitePushUrl :" + ex.Message.ToString());
                }
            }
            return "0";
        }


        private  string GetjposGeneralSetting()
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.GeneralSetting WHERE keyGroup='JPOS_Staging'";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        for (int i=0; i< result.Rows.Count;i++)
                        {
                            switch (result.Rows[i][1].ToString())
                            {
                                case JPOSConstants.JPOS_Version:
                                    version = result.Rows[i][2].ToString();
                                    break;
                                case JPOSConstants.JPOS_ConsumerId:
                                    consumerId = result.Rows[i][2].ToString();
                                    break;
                                case JPOSConstants.JPOS_HostURL:
                                    url = result.Rows[i][2].ToString();
                                    break;
                                case JPOSConstants.JPOS_N:
                                    n = result.Rows[i][2].ToString();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error : GetjposGeneralSetting :" + ex.Message.ToString());
                }
            }
            return "0";
        }


        private void create()
        {
            
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString());
            }

            
        }

    }
}
