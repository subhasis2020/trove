using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
//using Foundry.Api.Models;
using log4net;

namespace Foundry.ScheduledJobs
{
    [DisallowConcurrentExecution]
    public class NotificationJob //: IJob
    {
    //    private static readonly ILog logger = LogManager.GetLogger("NotificationJobScheduler");
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        ScheduleOfferNRewards();
    //        return Task.CompletedTask;
    //    }
    //    /// <summary>
    //    /// ScheduleOfferNRewards
    //    /// </summary>
    //    public void ScheduleOfferNRewards()
    //    {
    //        var fileUrl = "";
    //        IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
    //        var url = Configuration.GetValue<string>("BasicUrl") + Constants.ScheduleOffersRewards;
    //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
    //        request.Method = "GET";
    //        request.PreAuthenticate = true;
    //        request.ContentType = "text/xml";
    //        HttpWebResponse response = null;
    //        try
    //        {
    //            response = (HttpWebResponse)request.GetResponse();
    //            fileUrl = new StreamReader(response.GetResponseStream()).ReadToEnd();
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Info("Error while getting Running  ScheduleOfferNRewards API " + ex);

    //        }
    //    }

    //    /// <summary>
    //    /// Generate Token 
    //    /// </summary>
    //    /// <returns></returns>
    //    private string GenerateToken()
    //    {
    //        IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
    //        string[] getUserNamePwd = DbFunction.GetUserNamePassword();
    //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ConfigurationData.GetValue<string>("BasicUrl") + Constants.GenerateUserToken);
    //        request.Method = "POST";
    //        request.ContentType = "application/x-www-form-urlencoded";
    //        var postData = "username=" + (getUserNamePwd[0] != null ? getUserNamePwd[0] : null);
    //        postData += "&password=" + (getUserNamePwd[1] != null ? getUserNamePwd[1] : null);
    //        postData += "&scope=openid offline_access";
    //        postData += "&client_id=ro.angular";
    //        postData += "&grant_type=password";
    //        postData += "&client_secret=secret";
    //        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
    //        request.ContentLength = byteArray.Length;
    //        Stream dataStream = request.GetRequestStream();
    //        dataStream.Write(byteArray, 0, byteArray.Length);
    //        dataStream.Close();
    //        HttpWebResponse response = null;
    //        var access_token = "";
    //        var refresh_token = "";
    //        try
    //        {
    //            response = (HttpWebResponse)request.GetResponse();
    //            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

    //            JObject objects = JObject.Parse(responseString);
    //            foreach (var kv in objects)
    //            {
    //                if (kv.Key == "access_token")
    //                {
    //                    access_token = kv.Value.ToString();
    //                }
    //                if (kv.Key == "refresh_token")
    //                {
    //                    refresh_token = kv.Value.ToString();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // logger.Info("UpdateAudioFile => Generate Token ERROR" + ex);
    //            //Console.WriteLine("Web exception occurred. Status code: {0}", ex.Message);
    //        }
    //        return access_token;
    //    }
    //    /// <summary>
    //    /// get notifications
    //    /// </summary>
    //    private void GetNotifications()
    //    {
    //        var fileUrl = "";
    //        IConfiguration ConfigurationData = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
    //        var basicUrl = ConfigurationData.GetValue<string>("BasicUrl");
    //        var url = basicUrl + Constants.UserNotifications;
    //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
    //        request.Method = "GET";
    //        request.PreAuthenticate = true;
    //        request.ContentType = "text/xml";
    //        request.Headers.Add("Authorization", "Bearer " + GenerateToken());

    //        HttpWebResponse response = null;
    //        try
    //        {

    //            response = (HttpWebResponse)request.GetResponse();
    //            fileUrl = new StreamReader(response.GetResponseStream()).ReadToEnd();
    //        }

    //        catch (Exception ex)
    //        {
    //            // logger.Info("Error while getting file from AWS  " + ex + "--filename-" + fileName);

    //        }
    //    }

    //    /// <summary>
    //    /// Send Push Notification
    //    /// </summary>
    //    private async void SendPushNotification()
    //    {
    //        PushNotifications objPushNotifications = new PushNotifications();
    //        try
    //        {
    //            var a = await objPushNotifications.AndroidPushNotification("0911a3fe2743c554", "Hello Test Message", "Aman -Test Notification", "", "", "", "AAAAge0fUa4:APA91bGiL9I7smSF3ViyEV0viFeArfDTmkL2L31R2kRm269thQpuko1KXW-aol9HO_gaQ2fsLD3Q2K_Pf0lDfl8kluBneZ4TQn0lCOiZM7sjqUgT3GQZLrUvFa8BGOX4Pyx5sUZwXvFS");
    //        }
    //        catch (Exception ex)
    //        {

    //        }

    //    }
   }
}
