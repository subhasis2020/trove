using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;


namespace Foundry.ScheduledJobs
{
    public static class DbFunction
    {
        // private static readonly ILog logger = LogManager.GetLogger("DbFuntion call");
        public static string[] GetUserNamePassword()
        {
            var dtResult = GetAdminUserandPwd();
            if (dtResult.Rows.Count > 0)
            {
                if (dtResult.Rows[0].ItemArray.Length == 2)
                {
                    string[] resultArray = new string[] { Convert.ToString("nana.kim@yopmail.com"), "Test@123" };
                    return resultArray;
                }
            }
            return null;
        }

        private static DataTable GetAdminUserandPwd()
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            DataTable result = new DataTable();
            try
            {
                string sql = @"Select top 1 U.UserName,U.passwordHash from [User] U inner join UserRole UR on U.Id=UR.UserId and UR.RoleId=1 and U.isdeleted=0 and U.isactive=1";
                using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("FoundryDb")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                //  logger.Info(ex);
            }
            return null;
        }
    }
}
