using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Text;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Foundry.ScheduledJobs
{
    [DisallowConcurrentExecution]
    public class ReportJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("hello");
            // Button1_Click();

            Start();
            return Task.CompletedTask;
        }

        private void Start()
        {

            if(CheckEmailSend("Daily"))
            {
                DailyLoyaltyActivity();
            }
            if (CheckEmailSend("Month_End_Loyalty_Summary"))
            {
                MonthEndLoyaltySummary();
            }
        }

        private  bool CheckEmailSend (string key)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = "";
                if (key == "Daily")
                {
                     sql = @"SELECT * FROM schedulerReportLog WHERE ReportType='Daily_Loyalty_Activity' AND CONVERT( DATE ,SendDate) = CONVERT( DATE ,GETUTCDATE())";
                }
                else if(key == "Month_End_Loyalty_Summary")
                {
                    sql = @"SELECT * FROM schedulerReportLog WHERE ReportType='Month_End_Loyalty_Summary' AND DATEPART( MONTH,SendDate ) = DATEPART(MONTH,GETUTCDATE())";
                }

                    using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        // return result.Rows[0][0].ToString();
                        if (result.Rows.Count > 0)
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return true;
        }


        private static string GetdatafromGeneralSetting(string key)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                key = key.Trim();
                string sql = @"SELECT Value FROM  dbo.GeneralSetting WHERE keyGroup ='schedulerReport' AND KeyName='" + key + "'";
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
              
            }
            return "0";
        }




        public async Task<bool> SendEmail(string ToEmail, string subject, string bodyHtml, string ccEmail, string bccEmail,string filePath,string filename)
        {
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            try
            {
                var message = new SendGridMessage();

                message.SetFrom(new EmailAddress(ConfigurationData.GetValue<string>("SendGridSettings:SenderEmail"), ConfigurationData.GetValue<string>("SendGridSettings:fromName")));
                var lstRecipients = new List<EmailAddress>();
                if (!string.IsNullOrEmpty(ccEmail))
                {
                    var lstCC = ccEmail.Trim(new char[] { ' ', '.', ',' }).Split(',').ToList();
                    foreach (var item in lstCC)
                    {
                        lstRecipients.Add(new EmailAddress() { Email = item });
                    }
                    message.AddCcs(lstRecipients);
                }

                lstRecipients = new List<EmailAddress>();
                if (!string.IsNullOrEmpty(bccEmail))
                {
                    var lstBCC = bccEmail.Trim(new char[] { ' ', '.', ',' }).Split(',').ToList();
                    foreach (var item in lstBCC)
                    {
                        lstRecipients.Add(new EmailAddress() { Email = item });
                    }
                    message.AddBccs(lstRecipients);
                }

                lstRecipients = new List<EmailAddress>();
                if (!string.IsNullOrEmpty(ToEmail))
                {
                    var lstBCC = ToEmail.Trim(new char[] { ' ', '.', ',' }).Split(',').ToList();
                    foreach (var item in lstBCC)
                    {
                        lstRecipients.Add(new EmailAddress() { Email = item });
                    }
                    message.AddTos(lstRecipients);
                }


                message.SetSubject(subject);
                //message.AddTo(new EmailAddress(ToEmail));
                message.HtmlContent = bodyHtml;

                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(file); //Attaching File to Mail 

                byte[] byteData = Encoding.ASCII.GetBytes(File.ReadAllText(filePath));
                message.Attachments = new List<SendGrid.Helpers.Mail.Attachment>
            {
                new SendGrid.Helpers.Mail.Attachment
                {
                    Content = Convert.ToBase64String(byteData),
                    Filename = filename,
                    Type = "text/csv",
                    Disposition = "attachment"
                }
            };
                
               



                await SendMailDispatch(message);

                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task SendMailDispatch(SendGridMessage message)
        {
            try
            {
                IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();

                var client = new SendGridClient(ConfigurationData.GetValue<string>("SendGridSettings:Key"));
                await client.SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }










        protected async void DailyLoyaltyActivity()
        {


            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            // Specify connection options and open an connection
            NpgsqlConnection conn = new NpgsqlConnection(Configuration.GetConnectionString("PostgreSqlDb"));
            conn.Open();
            NpgsqlDataAdapter cmd = new NpgsqlDataAdapter("SELECT date_trunc('day',  t.date) as Date , c.issuer as CostCenterNumber, sum( case when t.redeem='1' then 0 else t.loyaltyamount end) as LoyaltyEarned , sum( case when t.redeem='1' then t.loyaltyamount else 0 end) as LoyaltyRedeemed from public.tranlog t inner join public.cardholder c on t.cardholder = c.id inner join public.issuers i on  i.id = c.issuer where loyaltyamount is not null and date_trunc('day',  t.date) = date_trunc('day',  ( NOW() - INTERVAL '1 DAY')) group by  date_trunc('day',  t.date), c.issuer", conn);
            DataSet ds = new DataSet();

            cmd.Fill(ds, "Data");
            DataTable dt = ds.Tables["Data"];
            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            string filename = "Daily_Loyalty_Activity_" + yesterday.ToString("yyyy-MM-dd") + ".csv";
           string  path = Configuration.GetValue<string>("BiteLogOutputDirectory") + "\\Daily_Loyalty_Activity_" + yesterday.ToString("yyyy-MM-dd") + ".csv";
           CreateCSVFile(dt, path);

         bool send = await SendEmail(GetdatafromGeneralSetting("schedulerReportEmail"), "Daily Loyaly Activity", "<h2>Dear Sir/Madam</h2><p>Please find the attached file for Daily Loyalty Activity </p><p>Thanks</p>", "", "", path,filename);
            if (send)
            {
                schedulerReportLog("Daily_Loyalty_Activity", DataTableToJSONWithJSONNet(dt));
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        protected async void MonthEndLoyaltySummary()
        {


            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            // Specify connection options and open an connection
            NpgsqlConnection conn = new NpgsqlConnection(Configuration.GetConnectionString("PostgreSqlDb"));
            conn.Open();
            NpgsqlDataAdapter cmd = new NpgsqlDataAdapter("SELECT TO_CHAR(date_trunc('month',  t.date), 'Month') as Month , c.issuer as CostCenterNumber, sum( case when t.redeem='1' then 0 else t.loyaltyamount end) as LoyaltyEarned , sum( case when t.redeem='1' then t.loyaltyamount else 0 end) as LoyaltyRedeemed from public.tranlog t inner join public.cardholder c on t.cardholder = c.id inner join public.issuers i on  i.id = c.issuer where loyaltyamount is not null and date_trunc('month',  t.date) = date_trunc('month',  ( NOW() - INTERVAL '1' month)) group by  date_trunc('month',  t.date), c.issuer", conn);
            DataSet ds = new DataSet();

            cmd.Fill(ds, "Data");
            DataTable dt = ds.Tables["Data"];
            var today = DateTime.UtcNow;
            var yesterday = today.AddMonths(-1);

            string filename = "Month_End_Loyaly_Activity_" + yesterday.ToString("MMMM") + ".csv";
            string path = Configuration.GetValue<string>("BiteLogOutputDirectory") + "\\Month_End_Loyaly_Activity_" + yesterday.ToString("MMMM") + ".csv";
            CreateCSVFile(dt, path);

            bool send = await SendEmail(GetdatafromGeneralSetting("schedulerReportEmail"), "Month End Loyalty Summary", "<h2>Dear Sir/Madam</h2><p>Please find the attached file for Month Loyalty Activity Summary </p><p>Thanks</p>", "", "", path, filename);
            if (send)
            {
                schedulerReportLog("Month_End_Loyalty_Summary" , DataTableToJSONWithJSONNet(dt));
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }


        public void CreateCSVFile(DataTable dt, string strFilePath)
        {
            // Create the CSV file to which grid data will be exported.

            StreamWriter sw = new StreamWriter(strFilePath, false);

            // First we will write the headers.

            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < iColCount - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);

            // Now write all the rows.
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < iColCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        sw.Write(dr[i].ToString());
                    }
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }


        private string schedulerReportLog(string ReportType,string contain)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"INSERT INTO dbo.schedulerReportLog( ReportType, SendDate, contain ) VALUES  ( @ReportType, GETUTCDATE(), @contain)";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add(new SqlParameter("@ReportType", ReportType));
                        cmd.Parameters.Add(new SqlParameter("@contain", contain));
                        cmd.ExecuteNonQuery();
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        //dataAdapter.Fill(result);
                        //return result.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            
            }
            return "0";
        }






    }
}
