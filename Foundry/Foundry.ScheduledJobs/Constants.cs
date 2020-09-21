using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.ScheduledJobs
{
    public static class Constants
    {
        public const string GenerateUserToken = "connect/token";
        public const string UserNotifications = "api/Notifications/UserNotifications";
        public const string ScheduleOffersRewards = "api/Scheduler/ScheduleOfferNRewards";
        public const string GetAlli2cUserAccountDetail = "api/I2CAccount/GetAlli2cUserAccountDetail";
        public const string GetTransactionHistory = "api/I2CAccount/TransactionHistory";
        public const string UpdateUserTransactionBalance = "api/I2CAccount/UpdateUserTransactionBalance";
        public const string Geti2cBank2CardTransfer = "api/I2CAccount/Geti2cBank2CardTransfer";
        public const string UpdateI2CAccountDetail = "api/I2CAccount/UpdateI2CAccountDetail";
    }

    public static class ConstantsBitenotification
    {
        public const string PointsCredited = "/Loyalty/PointsCredited";
        public const string RewardIssued = "/Loyalty/RewardIssued";
        public const string FirstTransactionBonus = "/Loyalty/1stTransactionBonus";

        public const string SodexoBiteBaseUrl = "SodexoBiteBaseUrl";
        public const string Is2ndUrlActiveForBite = "Is2ndUrlActiveForBite";
        public const string SecondSodexoBiteBaseUrl = "2ndSodexoBiteBaseUrl";


    }
    public static class JPOSConstants
    {
        public const string JPOS = "JPOS";
        public const string JPOS_Version = "JPOS_Version";
        public const string JPOS_ConsumerId = "JPOS_ConsumerId";
        public const string JPOS_N = "JPOS_N";
        public const string JPOS_HostURL = "JPOS_HostURL";
    }

}
