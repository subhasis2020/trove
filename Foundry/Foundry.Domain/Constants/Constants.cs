
using Microsoft.Extensions.Configuration;

namespace Foundry.Domain
{
    public class Constants
    {
        IConfiguration _config;
        public Constants(IConfiguration config)
        {
            _config = config;
        }
        public static class GeneralConstants
        {
            public const string UploadFolder = "wwwroot\\Uploads";
        }
        public static class SMTPConstants
        {
            public const string SMTP_Host = "SMTP-Host";
            public const string SMTP_Port = "SMTP-Port";
            public const string SMTP_UserName = "SMTP-UserName";
            public const string SMTP_Password = "SMTP-Password";
            public const string SMTP_EnableSSL = "SMTP-EnableSSL";
            public const string EmailFrom = "EmailFrom";
            public const string SMTP = "SMTP";
            public const string SMSEmail = "SMSEmail";
        }
        public static class JPOSConstants
        {
            public const string JPOS = "JPOS";
            public const string JPOS_Version = "JPOS_Version";
            public const string JPOS_ConsumerId = "JPOS_ConsumerId";
            public const string JPOS_N = "JPOS_N";
            public const string JPOS_HostURL = "JPOS_HostURL";
        }

        public static class GeneralSettingsKeyGroup
        {
            public const string JPOS_Staging = "JPOS_Staging";
            public const string JPOS_Apiary = "JPOS_Apiary";
            public const string JPOS_Version0 = "JPOS_Version0";
        }
        public static class FireBaseConstants
        {
            public const string FireBaseServerKey = "FireBase";

        }
        public static class EmailTemplates
        {
            public const string ResetPassword = "ResetPassword";
            public const string LinkAccountVerification = "LinkAccountVerification";
            public const string RegistrationFoundry = "RegistrationFoundry";
            public const string BenefactorInvitation = "BenefactorInvitation";
            public const string ReloadBalanceRequest = "ReloadBalanceRequest";
            public const string ForgotPassword = "ForgotPassword";
            public const string BalanceReload = "BalanceReload";
            public const string BenefactorAddValue = "BenefactorAddValue";           
            public const string InvitationAcceptance = "InvitationAcceptance";
            public const string OrganizationAdminIntimation = "OrganizationAdminIntimation";
            public const string ProgramAdminIntimation = "ProgramAdminIntimation";
            public const string MerchantAdminIntimation = "MerchantAdminIntimation";
            public const string MagicLink = "MagicLink";
            public const string VirtualCardBitePay = "VirtualCardBitePay";
            public const string SetSubscriptionRule = "SetSubscriptionRule";
            public const string BenefactorSetSubscriptionRule = "BenefactorSetSubscriptionRule";
            public const string PartnerReloadBalanceRequest = "SodexoBiteReloadBalanceRequest";
        }
        public static class SMSTemplates
        {
            public const string ResetPassword = "ResetPassword";
            public const string LinkAccountVerification = "LinkAccountVerification";
            public const string BenefactorInvitation = "BenefactorInvitation";
            public const string MagicLink = "MagicLink";

        }
        public class ImagesDefault
        {
            public readonly static string UserDefaultImage = string.Concat("Images", "/avatar.png");
            public readonly static string ProgramDefaultImage = string.Concat("Images", "/image_not_available.png");
            public readonly static string OrganisationDefaultImage = string.Concat("Images", "/image_not_available.png");
            public readonly static string FoodDefaultImage = string.Concat("Images", "/image_not_available.png");
            public readonly static string MerchantDefaultImage = string.Concat("Images", "/image_not_available.png");
            public readonly static string PromotionDefaultImage = string.Concat("Images", "/image_not_available.png");
            public readonly static string BrandingDefaultImage = string.Concat("Images", "/image_not_available.png");
            public readonly static string OtherDefaultImage = string.Concat("Images", "/image_not_available.png");
        }
        public static class PhotoType
        {
            public const int UserProfile = 1;
            public const int Organisation = 2;
            public const int Offers = 3;
            public const int Program = 4;
        }
        public class ProgramExpiryType
        {
            public const int DateExpiration = 1;
            public const int DurationExpiration = 2;
        }
        public static class InvitationType
        {
            public const int Benefactor = 1;
        }
        public static class TransactionUserEnityType
        {
            public const int User = 1;
            public const int Benefactor = 2;
            public const int Merchant = 3;
            public const int Admin = 4;
            public const int OrganisationAdmin = 5;
            public const int ProgramAdmin = 6;
            public const int MerchantAdmin = 7;
            public const int BankAccountCredit = 8;
        }
        public static class ConnectionsType
        {
            public const int Invitee = 1;
            public const int ExistingConnection = 2;
        }
        public static class Roles
        {
            public const string BasicUser = "Basic User";
            public const string Benefactor = "Benefactor";
            public const string SuperAdmin = "Super Admin";
            public const string OrganizationFull = "Organization Full";
            public const string OrganizationReporting = "Organization Reporting";
            public const string MerchantFull = "Merchant Full";
            public const string MerchantReporting = "Merchant Reporting";
            public const string ProgramFull = "Program Full";
            public const string ProgramReporting = "Program Reporting";
        }
        public static class RoleType
        {
            public const int BasicUserRoleType = 1;
            public const int BenefactorRoleType = 2;
            public const int OrganizationRoleType = 3;
            public const int SuperAdminRoleType = 4;
            public const int MerchantRoleType = 5;
            public const int ProgramRoleType = 6;
        }
        public static class GeneralSettingsConstants
        {
            public const string SMTP = "SMTP";
            public const string Reload = "Reload";
            public const string Radius = "Radius";
            public const string SMS = "SMS";
            public const string UserFirstTransactionBonus = "UserFirstTransactionBonus";
        }
        public static class Groups
        {
            public const string Benefactors = "Benefactors";
            public const string Students = "Students";
        }
        public static class TableType
        {
            public const int Transactions = 1;
            public const int LinkedAccount = 2;
        }
        public static class AccountTypeConstants
        {
            public const int MealPasses = 1;
            public const int FlexSpending = 2;
            public const int Discretionary = 3;
        }
        public static class OrganasationType
        {
            public const int Foundry = 1;
            public const int University = 2;
            public const int Merchant = 3;
        }
        public static class OfferType
        {
            public const int Promotions = 1;
            public const int Rewards = 2;
        }
        public static class OfferSubType
        {
            public const int DailyPromotion = 1;
            public const int MultiDayPromotion = 2;
            public const int NoOfVisitsRewards = 3;
            public const int AmountRewards = 4;
        }
        public static class JPOSAPIURLConstants
        {
            public const string Organization = "organizations";
            public const string Issuers = "issuers";
            public const string AccountHolder = "accountholders";
            public const string Merchants = "acquirers/{acquirer}/merchants";
            public const string Terminals = "acquirers/{issuer}/merchants/{merchant}/terminals";
            public const string Plans = "issuers/issuer/plans";
            public const string Accounts = "issuers/issuer/accounts";
            public const string CardsCollection = "cards";
            public const string txnlog = "transactions/txnlog";
        }
        public static class JPOSAPIConstants
        {
            public const string Organization = "organizations";
            public const string Issuers = "issuers";
            public const string AccountHolder = "accountholders";
            public const string Merchants = "merchants";
            public const string Terminals = "terminals";
            public const string Plans = "plans";
            public const string Accounts = "accounts";
            public const string Transactions = "transactions";
            
        }
        public static string[] TransactionsColumn = new string[6] { "Merchant Name", "Period", "Amount", "Date", "Time", "Plan Name" };


        public static class I2CPush
        {
            public const string IsActive = "IsForwardI2CPushActive";
            public const string ForwardI2CPushUrl = "ForwardI2CPushUrl";
        }
        public static class SodexoBiteNotification
        {
            public const string SodexoBiteBaseUrl = "SodexoBiteBaseUrl";
            public const string Is2ndUrlActiveForBite = "Is2ndUrlActiveForBite";
            public const string SecondSodexoBiteBaseUrl = "2ndSodexoBiteBaseUrl";
            public const string BitePayCardActivated = "bite-paycard/activated";
            public const string BitePayCardIssued = "bite-paycard/issued";
            public const string BitePayBenefactorInviteAccepted = "benefactor/invite-accepted";
            public const string BitePayAddValue = "bite-paycard/funds-added";
            public const string ReloadSetUp = "benefactor/Auto-Reload-Setup";
            public const string StatusChange = "Loyalty/StatusChange";
            public const string InPersonSaleUrl = "Purchase/In-Person";
            public const string OrderAheadUrl = "Purchase/OrderAhead";


        }


    }
}
