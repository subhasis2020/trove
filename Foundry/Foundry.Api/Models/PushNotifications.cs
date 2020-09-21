using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PushSharp.Apple;
using PushSharp.Common;
using FirebaseNet.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Foundry.Services;
using Foundry.Domain;

namespace Foundry.Api.Models
{
    /// <summary>
    /// This class is used to send push notifications to the user.
    /// </summary>
    public class PushNotifications
    {
        /// <summary>
        /// This is an empty constructor.
        /// </summary>
        public PushNotifications()
        {
        }

        /// <summary>
        /// Send push
        /// </summary>
        /// <param name="registerationId"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="deviceType"></param>
        /// <param name="data"></param>
        /// <param name="customData"></param>
        /// <param name="icon"></param>
        /// <param name="type"></param>
        /// <param name="badge"></param>
        /// <param name="firebaseServerKey"></param>
        /// <returns></returns>
        public async Task SendPush(string registerationId, string title, string body, string deviceType, string data, string customData, string icon, string type, int badge, string firebaseServerKey)
        {
            await AndroidPushNotification(registerationId, body, title, icon, data, type, firebaseServerKey);
        }


        /// <summary>
        /// Send Push
        /// </summary>
        /// <param name="registerationId"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="deviceType"></param>
        /// <param name="data"></param>
        /// <param name="customData"></param>
        /// <param name="icon"></param>
        /// <param name="type"></param>
        /// <param name="badge"></param>
        /// <param name="firebaseServerKey"></param>
        /// <param name="isRedirect"></param>
        /// <param name="notificationSubType"></param>
        /// <param name="customReferenceId"></param>
        /// <returns></returns>
        public async Task SendPushBulk(List<string> registerationId, string title, string body, string deviceType, string data, string customData, string icon, string type, int badge, string firebaseServerKey, bool isRedirect, string notificationSubType, int customReferenceId)
        {
            await AndroidPushNotificationBulk(registerationId, body, title, icon, data, type, firebaseServerKey, isRedirect, notificationSubType, customReferenceId);
        }

        /// <summary>
        /// Send Push Android.
        /// </summary>
        /// <param name="registerationId"></param>
        /// <param name="body"></param>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="firebaseServerKey"></param>
        /// <returns></returns>
        public async Task<bool> AndroidPushNotification(string registerationId, string body, string title, string icon, string data, string type, string firebaseServerKey)
        {
            if (!string.IsNullOrEmpty(firebaseServerKey))
            {
                FCMClient client = new FCMClient(firebaseServerKey);
                var message = new Message()
                {
                    To = registerationId,
                    Notification = new AndroidNotification() { Body = body, Title = title, Icon = data, ClickAction = ".ClientDashboardActivity" },
                    Data = new Dictionary<string, string> { { "data", data }, { "NotificationType", type }, { "AccountType", "2" } }
                };
                try
                {
                    await client.SendMessageAsync(message);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerationId"></param>
        /// <param name="body"></param>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="firebaseServerKey"></param>
        /// <param name="isRedirect"></param>
        /// <param name="notificationSubType"></param>
        /// <param name="customReferenceId"></param>
        /// <returns></returns>
        public async Task<bool> AndroidPushNotificationBulk(List<string> registerationId, string body, string title, string icon, string data, string type, string firebaseServerKey, bool isRedirect, string notificationSubType, int customReferenceId)
        {
            if (!string.IsNullOrEmpty(firebaseServerKey))
            {
                FCMClient client = new FCMClient(firebaseServerKey);
                var message = new Message()
                {
                    RegistrationIds = registerationId,
                    Notification = new AndroidNotification() { Body = body, Title = title, Icon = data, ClickAction = ".ClientDashboardActivity" },
                    Data = new Dictionary<string, string> { { "data", data }, { "notificationType", type }, { "accountType", "2" }, { "IsRedirect", isRedirect.ToString() }, { "NotificationSubType", notificationSubType }, { "CustomReferenceId", customReferenceId.ToString() } }
                };
                try
                {
                     await client.SendMessageAsync(message);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
