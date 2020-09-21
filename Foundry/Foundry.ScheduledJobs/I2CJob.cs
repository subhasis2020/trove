//using Foundry.Api.Models;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Foundry.ScheduledJobs
{
    [DisallowConcurrentExecution]
    public class I2CJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger("I2CJobScheduler");
        public Task Execute(IJobExecutionContext context)
        {
            logger.Info("Setting Push Notification");
            SendPushNotification();
            GetAlli2cUserAccountDetails();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Generate Token 
        /// </summary>
        /// <returns></returns>
        private string GenerateToken()
        {
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            string[] getUserNamePwd = DbFunction.GetUserNamePassword();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ConfigurationData.GetValue<string>("BasicUrl") + Constants.GenerateUserToken);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var postData = "username=" + (getUserNamePwd[0] != null ? getUserNamePwd[0] : null);
            postData += "&password=" + (getUserNamePwd[1] != null ? getUserNamePwd[1] : null);
            postData += "&scope=openid offline_access";
            postData += "&client_id=ro.angular";
            postData += "&grant_type=password";
            postData += "&client_secret=secret";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            HttpWebResponse response = null;
            var access_token = "";
            var refresh_token = "";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject objects = JObject.Parse(responseString);
                foreach (var kv in objects)
                {
                    if (kv.Key == "access_token")
                    {
                        access_token = kv.Value.ToString();
                    }
                    if (kv.Key == "refresh_token")
                    {
                        refresh_token = kv.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Info("Error while getting file from GenerateToken  " + ex);
            }
            return access_token;
        }

        /// <summary>
        /// get Alli2cUserAccountDetail
        /// </summary>
        private void GetAlli2cUserAccountDetails()
        {
            var fileUrl = "";
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var basicUrl = ConfigurationData.GetValue<string>("BasicUrl");
            var url = basicUrl + Constants.GetAlli2cUserAccountDetail;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.PreAuthenticate = true;
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", "Bearer " + GenerateToken());
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var jsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var lstI2CBank2CardTrasfer = JsonConvert.DeserializeObject<List<i2cBank2CardTransfer>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.Info("Error while getting file from GetAlli2cUserAccountDetails  " + ex);

            }
        }

        /// <summary>
        /// get Transection History Table records
        /// </summary>
        private void GetTransectionHistoryDetails(string refId, string dateFrom, string dateTo)
        {
            var fileUrl = "";
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var basicUrl = ConfigurationData.GetValue<string>("BasicUrl");
            var url = basicUrl + Constants.GetTransactionHistory + "?refId=" + refId + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.PreAuthenticate = true;
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", "Bearer " + GenerateToken());
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                fileUrl = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                logger.Info("Error while getting file from GetTransectionHistoryDetails  " + ex);
            }
        }
        /// <summary>
        /// get Transection History Table records
        /// </summary>
        /// Need to add send model with POST Request 
        private void UpdateUserTransactionBalance(int Id, UserTransactionInfo userTransactionInfoModel)
        {
            var fileUrl = "";
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var basicUrl = ConfigurationData.GetValue<string>("BasicUrl");
            var url = basicUrl + Constants.UpdateUserTransactionBalance;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", "Bearer " + GenerateToken());
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                fileUrl = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                logger.Info("Error while getting  from UpdateUserTransactionBalance  " + ex);
            }
        }

        /// <summary>
        /// Send Push Notification
        /// </summary>
        private async void SendPushNotification()
        {
            //PushNotifications objPushNotifications = new PushNotifications();
            //await objPushNotifications.AndroidPushNotification("0911a3fe2743c554", "Hello Test Message", "Aman -Test Notification", "", "", "", "AAAAge0fUa4:APA91bGiL9I7smSF3ViyEV0viFeArfDTmkL2L31R2kRm269thQpuko1KXW-aol9HO_gaQ2fsLD3Q2K_Pf0lDfl8kluBneZ4TQn0lCOiZM7sjqUgT3GQZLrUvFa8BGOX4Pyx5sUZwXvFS");
        }

        /// <summary>
        /// get Alli2cBank2CardTransfer
        /// </summary>
        private void GetAlli2cBank2CardTransfer()
        {
            var fileUrl = "";
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var basicUrl = ConfigurationData.GetValue<string>("BasicUrl");
            var url = basicUrl + Constants.Geti2cBank2CardTransfer;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.PreAuthenticate = true;
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", "Bearer " + GenerateToken());
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var jsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var lstI2CBank2CardTrasfer = JsonConvert.DeserializeObject<List<i2cBank2CardTransfer>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.Info("Error while getting from GetAlli2cBank2CardTransfer  " + ex);
            }
        }

        /// <summary>
        /// get UpdateI2CAccountDetail
        /// </summary>
        private void UpdateI2CAccountDetail()
        {
            var fileUrl = "";
            IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var basicUrl = ConfigurationData.GetValue<string>("BasicUrl");
            var url = basicUrl + Constants.UpdateI2CAccountDetail;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.PreAuthenticate = true;
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", "Bearer " + GenerateToken());
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var jsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                logger.Info("Error while getting file from UpdateI2CAccountDetail  " + ex);
            }
        }
    }
}
