
using Microsoft.Extensions.Configuration;

namespace Foundry.Domain
{
    public static class ApiConstants
    {
        public const string ResetUserPassword = "api/Account/CheckUserForResetPassword";
        public const string ForgotUserPassword = "api/Account/ForgotPasswordResetLink";
        public const string UserInactiveCheck = "api/Account/InactivityCheck";
        public const string LoginAuthenticate = "api/Account/Login";
        public const string CheckUseResetPasswordLink = "api/Account/CheckUseResetPasswordLink";
        public const string UserInfoValues = "api/Account/UserValues";
        public const string GenerateUserToken = "connect/token";
        public const string GenerateClaims = "api/account/claims";
        public const string LinkedUsers = "api/benefactor/LinkedUsers";
        public const string LinkedUsersInformation = "api/benefactor/LinkedUsersInformation";
        public const string LinkedUsersTransactions = "api/benefactor/LinkedUsersTransactions";
        public const string ReloadAmount = "api/benefactor/ReloadAmount";
        public const string CurrentBalanceUser = "api/userTransactions/GetUserAvailableBalanceForVPL";
        public const string GetUserAvailableBalanceByUserId = "api/userTransactions/GetUserAvailableBalanceByUserId";
        public const string DeleteInvitation = "api/benefactor/DeleteInvitation";
        public const string AcceptInvitation = "api/benefactor/AcceptInvitation";
        public const string BenefactorNotifications = "api/benefactor/BenefactorNotifications";
        public const string DeleteBenefactorUser = "api/benefactor/DeleteLinkedAccounts";
        public const string ReloadRuleUser = "api/benefactor/ReloadRuleOfUser";
        public const string InviteAdmin = "api/Account/InviteAdmin";
        public const string SetReloadRules = "api/benefactor/AddUpdateReloadRules";
        public const string CancelReloadRules = "api/benefactor/CancelSubscriptionRule";
        public const string GetBiteUserLoyaltyTrackingBalance = "api/Account/GetBiteUserLoyaltyTrackingBalance";
        public const string GetJPOSBiteUserBalance = "api/JPOS/GetJPOSBiteUserBalance";
        public const string GetUserLoyaltyRewardTransactions = "api/Partner/GetUserLoyaltyRewardTransactions";
        public const string GetUserBitePayTransactions = "api/Partner/GetUserBitePayTransactions";
        public const string GetIssuerProperties = "api/JPOS/GetIssuerProperties";
        public const string UpdateIssuer = "api/JPOS/UpdateIssuer";
        public const string BenefactorDetails = "api/benefactor/BenefectorDetails";



        #region Account Holders (User)
        public const string GetAccountHolders = "api/Account/GetAccountHolders";
        public const string GetAccountHoldersListByOrganization = "api/Account/GetAccountHoldersListByOrganization";
        public const string DeleteAccountHolder = "api/Account/DeleteAccountHolder";
        public const string GetAccountHolderDetail = "api/Account/GetAccountHolderDetail";
        public const string RegisterAccountHolder = "api/Account/RegisterAccountHolder";
        public const string GetUserLoyaltyTrackingTransactions = "api/Account/GetUserLoyaltyTrackingTransactions";
        #endregion

        #region Program
        public const string ProgramType = "api/program/ProgramTypes";
        public const string GetAllPrograms = "api/program/Programs";
        public const string ProgramLevelAdminList = "api/Program/ProgramLevelAdminList";
        public const string AddUpdateProgramLevelAdminUser = "api/account/AddUpdateProgramLevelAdminUser";
        public const string InviteProgramLevelAdmin = "api/Program/InviteProgramLevelAdmin";
        public const string Transactions = "api/Program/Transactions";
        public const string ProgramInfoById = "api/Program/ProgramInfoById";
        public const string MaxProgramInfoValue = "api/Program/MaxProgramInfoValue";
        public const string GetAllProgramsList = "api/Program/GetAllProgramsList";
        public const string AddUpdateProgramInfo = "api/Program/AddUpdateProgramInfo";
        public const string DeleteProgram = "api/Program/DeleteProgram";
        public const string GetUserProgramByUserId = "api/Program/GetUserProgramByUserId";
        public const string GetAllProgramsForPrgAdminList = "api/Program/GetAllProgramsForPrgAdminList";
        public const string GetPrimaryOrgNPrgDetailOfProgramAdmin = "api/Program/GetPrimaryOrgNPrgDetailOfProgramAdmin";
        public const string AddUpdateSiteLevelOverrideSettings = "api/Loyality/AddUpdateSiteLevelOverrideSettings";

        #endregion

        #region Organisation
        public const string Organisations = "api/Organisation/Organisations";
        public const string OrganisationInfoNProgramTypes = "api/Organisation/OrganisationInfoNProgramTypes";
        public const string AddUpdateOrganisationInfo = "api/Organisation/AddUpdateOrganisationDetail";
        public const string OrganisationsAdminInfo = "api/Organisation/OrganisationsAdmins";
     
        public const string MerchantAdmins = "api/Organisation/MerchantAdmins";
        public const string OrganisationPrograms = "api/Organisation/OrganisationPrograms";
        public const string OrganisationLoyalityGlobalSettings = "api/Loyality/GetOrgLoyaltyGlobalSettings";
        public const string SiteLevelOverrideSettings = "api/Loyality/GetSiteLevelOverrideSettings";
        public const string AddUpdateOrganisationPrograms = "api/Organisation/AddUpdateOrganisationProgramsDetail";
        public const string DeleteOrganisationsProgram = "api/Organisation/DeleteOrganisationProgram";
        public const string DeleteOrganisation = "api/Organisation/DeleteOrganisation";
        public const string UpdateOrganisationAdminStatus = "api/Organisation/UpdateOrganisationAdminStatus";
        public const string MasterRolesNProgramType = "api/Organisation/MasterRolesNProgramType";
        public const string AdminUserInfoNProgramType = "api/Organisation/AdminUserInfoNProgramType";
        public const string AddUpdateAdminUser = "api/account/AddUpdateAdminUser";
        public const string AddUpdateOrgLoyalityGlobalSetting = "api/Loyality/AddUpdateOrgLoyaltyGlobalSettings";
        public const string DeleteAdminUser = "api/account/DeleteAdminUser";
        public const string MerchantListWithTransaction = "api/Organisation/GetAllMerchantTransaction";
        public const string GetMerchantDetailInfoWithProgNAccType = "api/Organisation/GetMerchantDetailInfoWithProgNAccType";
        public const string AddUpdateMerchantDetailInfo = "api/Organisation/AddUpdateMerchantDetail";
        public const string MerchantTransaction = "api/Organisation/GetAllTransactionOfMerchant";
        public const string GetMerchantRewardDetail = "api/Organisation/GetMerchantRewardDetailInfoWithBusinessType";
        public const string GetMerchantRewardList = "api/Organisation/GetMerchantRewardList";
        public const string AddUpdateMerchantRewardInfo = "api/Organisation/AddUpdateMerchantRewardDetail";
        public const string GetMerchantBusinesInfo = "api/Organisation/GetMerchantBusinessInfo";
        public const string AddUpdateMerchantBusinessInfo = "api/Organisation/CreatModifyOrganisationBusinessInformation";
        public const string MasterOfferCodeNWeekDays = "api/Organisation/MasterOfferCodeNWeekDays";
        public const string GetMerchantPromotions = "api/Organisation/GetMerchantPromotions";
        public const string DeletePromotion = "api/Organisation/DeletePromotion";
        public const string CloneMerchant = "api/Organisation/CloneMerchant";
        public const string GetPromotionsByID = "api/Organisation/GetPromotionsByID";
        public const string MasterRolesNOrganizationPrograms = "api/Organisation/MasterRolesNOrganizationPrograms";
        public const string CheckOrganisationExistenceById = "api/Organisation/CheckOrganisationExistenceById";
        public const string EditPromotionRewardStatus = "api/Organisation/EditPromotionRewardStatus";

        #endregion

        #region AccountType
        public const string GetAllAccountType = "api/AccountType/GetAccountType";
        public const string GetAccountTypeById = "api/AccountType/GetAccountTypeById";
        #endregion

        #region Image
        public const string UploadImage = "api/Image/PostImage";
        public const string DeleteImage = "api/Image/RemoveImage";
        public const string S3ImageForFileName = "api/Image/S3ImageForFileName";
        #endregion

        #region Plan
        public const string PlanListing = "api/program/PlanListing";
        public const string UpdatePlanStatus = "api/Program/UpdatePlanStatus";
        public const string DeletePlan = "api/Program/DeletePlan";
        public const string ClonePlan = "api/Program/ClonePlan";
        public const string GetPlanDataById = "api/Program/GetPlanInfoWithProgAcc";
        public const string AddUpdatePlanInfo = "api/Program/CreateModifyPlan";
        #endregion

        #region ProgramAccount
        public const string ProgramAccountListing = "api/program/AccountListing";
        public const string UpdateProgramAccountStatus = "api/Program/UpdateProgramAccountStatus";
        public const string DeleteProgramAccount = "api/Program/DeleteProgramAccount";
        public const string CloneProgramAccount = "api/Program/CloneAccount";
        public const string GetProgramAccountDataById = "api/Program/GetProgramAccountInfoWithAccountType";
        public const string AddUpdateProgramAccountInfo = "api/Program/CreateModifyProgramAccount";
        public const string ProgramAccountDropdownForUser = "api/Program/ProgramAccountDropdownForUser";
        
        #endregion

        #region Account Merchant Rule
        public const string GetBusinessTypeAndMerchantListing = "api/program/GetBusinessTypeAndMerchantListing";
        public const string CreateModifyAccountMerchantRules = "api/Program/CreateModifyAccountMerchantRules";
        public const string ModifyAccountMerchantRuleDetails = "api/Program/ModifyAccountMerchantRuleDetails";
        #endregion

        #region Import User
        public const string GetMaximumSheetRows = "api/ImportUser/GetMaximumSheetRows";
        public const string AddUserFromExcel = "api/ImportUser/AddUserFromExcel";
        public const string CheckUserByEmail = "api/ImportUser/CheckUserByEmail";
        public static string CheckPhoneNumberExistence = "api/ImportUser/CheckPhoneNumberExistence";
        public static string SendMagicLinkInEmail = "api/ImportUser/SendMagicLinkInEmail";
        public static string i2cEndPointUrl = "https://ws2.mycardplace.com:6443/MCPWebServiceDL/services/MCPWSHandler";
        public static string GetUserbyUserCode = "api/ImportUser/GetUserbyUserCode";
        #endregion

        #region Branding
        public const string BrandingListing = "api/program/BrandingListing";
        public const string DeleteBranding = "api/Program/DeleteBranding";
        public const string GetBrandingDataById = "api/Program/GetBrandingInfoWithAccountType";
        public const string AddUpdateBrandingInfo = "api/Program/CreateModifyBranding";
        public const string GetProgramAccountTypeById = "api/Program/GetProgramAccountTypeById";

        #endregion

        #region I2C
        public const string GetBankAccountI2C = "api/I2CAccount/Account";
        public const string Geti2cUserAccountDetail = "api/I2CAccount/Get";
        public const string AddBankAccountDetail = "api/I2CAccount/AddBankAccount";
        public const string VerifyAccountDetail = "api/I2CAccount/VerifyAccount";
        public const string AccountBySerialNo = "api/I2CAccount/AccountBySerialNo";
        public const string Bank2CardTransfer = "api/I2CAccount/Bank2CardTransfer";
        public const string ActivateCard = "api/I2CAccount/ActivateCard";
        public const string GetUserCardwithBankAccount = "api/I2CAccount/GetUserCardwithBankAccount";
        public const string FiservCreditFundsi2c = "api/I2CAccount/CreditFundsOnFiservPayment";
        public const string CreditFundsi2cByAdmin = "api/I2CAccount/CreditFundsByAdmin";
        public const string DebitFundsi2cByAdmin = "api/I2CAccount/DebitFundsByAdmin";
        public const string ReversalI2CTranByAdmin = "api/I2CAccount/ReversalTranByAdmin";
        public const string i2cGetBalance = "api/I2CAccount/GetBalance";
        #endregion

        #region Cardholder Agreement

        public const string CardholderAgreements = "api/Program/CardholderAgreements";
        public const string CardholderAgreementsUserHistory = "api/Program/CardholderAgreementsUserHistory";
        public const string UserAgreementHistoryVersions = "api/Program/UserAgreementHistoryVersions";
        public const string CardholderAgreementById = "api/Program/CardholderAgreementById";
        public const string CardholderAgreementByIdUnauth = "api/Program/CardholderAgreementByIdUnauth";
        public const string AddCardHolderAgreement = "api/Program/AddCardHolderAgreement";
        public const string CardHolderAgreementsExistence = "api/Program/CardHolderAgreementsExistence";
        #endregion

        #region Merchant
        public const string GetAllMerchantsByAdmin = "api/Organisation/GetAllMerchantsByAdmin";
        public const string GetPrimaryOrgNPrgDetailOfMerchantAdmin = "api/Organisation/GetPrimaryOrgNPrgDetailOfMerchantAdmin";
        #endregion

        #region Payment Gateway
        public const string UsersCards = "api/Fiserv/UsersCards";
        public const string UsersWebToken = "api/Fiserv/UsersWebToken";
        public const string AddGatewayWebToken = "api/Fiserv/AddGatewayWebToken";
        public const string AddGatewayWebTokenNLog = "api/Fiserv/AddGatewayWebTokenNLog";        
        public const string AddGatewayWebLogToken = "api/Fiserv/AddGatewayWebLogToken";
        public const string UsersWebTokenByClientToken = "api/Fiserv/UsersWebTokenByClientToken";
        public const string AddPaymentTransactionLog = "api/Fiserv/AddPaymentTransactionLog";
        public const string DeleteGatewayWebToken = "api/Fiserv/DeleteGatewayWebToken";
        public const string UpdateWebToken = "api/Fiserv/UpdateWebToken";
        
        #endregion

        #region UserTransaction
        public const string AddUserTransaction = "api/UserTransactions/AddUserTransaction";
        public const string GettranlogData = "api/Partner/GettranlogData";

        #endregion

        #region Notification
        public const string GetAllNotificationLogs = "api/Notifications/GetAllNotificationLogs";
        public const string GetAllNotificationLogsWithFilter = "api/Notifications/GetAllNotificationLogsWithFilter";
        
        public const string GetAllApiNames = "api/Notifications/GetAllApiNames";
        public const string GetAllStatus = "api/Notifications/GetAllStatus";
        public const string GetorgAllPrograms = "api/Notifications/GetAllPrograms";
        #endregion
    }
}
