
namespace Foundry.Domain
{
    public static class MessagesConstants
    {
        #region User Message

        public const string UserNotExist = "The email address you entered does not exist. Please try again or contact your program manager at {supportemail}.";
        public const string BiteUserNotExist = "User ID you entered does not exist";
        public const string UnabletoAuthorize = "Unable to authorize.Please contact your Administrator";
        public const string UserLoggedIn = "You have successfully logged in.";
        public const string LogInPasswordIncorrect = "The password you entered is incorrect. Please try again or contact your program manager at {supportemail}.";
        public const string LogInUserLockedOut = "Your account has been locked due to multiple failed attempts to log in. Please contact your program manager at {supportemail}.";

        public const string ForgotPasswordEmailAddressIncorrect = "You have not entered a valid email address. Please try again or contact the administrator at {supportemail}.";

        public const string EmailNotConfirmed = "The email address you have entered has not been confirmed. Please try again or contact the administrator at {supportemail}.";

        public const string VerificationCodeSent = "A verification code has been sent to ";

        public const string InvalidVerificationCode = "You have entered an invalid code. Please try again or contact your program manager at {supportemail}.";

        public const string VerificationCodeExpired = "This verification code has expired.";

        public const string OneTimeUserVerificationCode = "A verification code can only be used just once.";

        public const string VerificationCodeMatched = "Verification code successful.";
        public const string PasswordChangedSuccess = "Password changed successfully.";
        public const string PasswordCreatedSuccess = "Password created successfully.";

        public const string InvalidUser = "You are not a valid user. Please try again or contact the administrator at {supportemail}.";

        public const string UserSuccessfulRegistration = "Registration successful.";
        public const string Invalidcredentials = "Invalid Credentials.";
        

        public const string GetSuccessfulUserDetail = "User detail returned successfully.";

        public const string UserAccountDeactivated = "Your account has been de-activated. Please contact your program manager at {supportemail} to reactivate your account.";
        public const string ValidationCodeSent = "A validation code has been sent to your ";
        public const string ResetPasswordLinkExpired = "Your link to reset your password has expired.";

        public const string OneTimeResetLinkUse = "Reset Password link can only be used once. Please resend it and try again.";
        public const string LinkIsActiveForUse = "Reset Password link is still valid.";

        public const string ResentPasswordLinkSent = "A reset password link has been sent to your email address.";

        public const string PhoneNumberExists = "The phone number you entered is already in use.";

        public const string PhoneNumberNotExists = "This phone number does not exist.";

        public const string UserDevicecNLocationSettingsUpdated = "Your location has been updated successfully.";

        public const string UserDevicecNLocationSettingsNotUpdated = "Your location has failed to update.";

        public const string ProfileUpdatedSuccessfully = "Profile has been updated successfully.";

        public const string GetLinkingProgramSuccessMessage = "You have linked to the program successfully.";

        public const string LinkingProgramWithUserAlreadyExists = "You are already linked to this program.";

        public const string MasterRoleNProgramType = "Role and program type is returned successfully.";
        public const string MasterOfferCodeNWeekDays = "Offer codes and week days are returned successfully.";
        public const string EmailExists = "This email has already been registered in the system.";
        public const string PartnerUserIdExists = "This Partner User Id has already been registered in the system.";
        public const string InvalidProgramIssuerId = "Invalid Program Issuer Id";
        public const string UserUnSuccessfulRegistration = "You have not registered successfully.";

        public const string AdminUserDetailUnSuccessfulReturn = "User detail has not been returned successfully.";

        public const string AdminUserDetailSuccessfulReturn = "Admin user detail has not been returned successfully.";

        public const string UserDeletedSuccessfully = "User is deleted successfully.";
        public const string UserNotDeletedSuccessfully = "User is not deleted successfully.";
        public const string UserAddedSuccessfully = "User detail is added successfully.";
        public const string UserNotAddedSuccessfully = "User detail is unable to add successfully.";
        public const string AccountholderAddedSuccessfully = "Account  holder is saved successfully.";
        public const string AccountholderNotAddedSuccessfully = "Currently unable to process the request. Please try again later.";
        public const string NoUserProgramsExist = "No user programs exist.";
        #endregion

        #region Program

        public const string NoProgramAssociationWithId = "Program is not associated with the given user id.";

        public const string NoProgramsExist = "No programs exist.";
        public const string NoProgramTypesExist = "No program types exist.";

        #endregion

        #region General

        public const string BadRequest = "Bad request error.";
        public const string ImageSavingIssue = "The image has not been saved properly.";
        public const string NoErrorMessagesExist = "No error messages exist.";
        public const string NoSessionMatchExist = "You have loggen in on another device. Your current session has expired.";
        public const string SomeIssueInProcessing = "There has been an issue in processing this request.";
        public const string DataSuccessfullyReturned = "Data have been successfully returned.";
        public const string DataNotSuccessfullyReturned = "No data exists.";
        #endregion

        #region Benefactor
        public const string NoRelationsExist = "No relations exist.";

        public const string ConnectionAlreadyExists = "The invitee is already added as your connection.";

        public const string InvitationAlreadySent = "An invitation has already been sent to your connection.";

        public const string NoConnectionsExist = "No connections exist.";

        public const string InvitationSuccessfulSent = "An invitation has been successfully sent to your connection.";

        public const string UserConnectionNotExists = "Connection does not exist.";

        public const string ConnectionDeletedSuccessfully = "Connection has not been deleted successfully.";

        public const string ReloadRequestSuccessfully = "A request to reload your balance has been successfully sent.";
        public const string ReloadBalanceSuccessfully = "Your balance has been reloaded successfully.";
        public const string ReloadRequestAlreadySent = "A request to relaod your balance has already sent.";
        public const string BalanceReturnedSuccessfully = "Your current balance is returned successfully.";
        public const string ReloadRuleData = "Your reload rule is returned successfully.";
        public const string NoReloadRuleData = "No reload rule exists.";
        public const string ProgramExpired = "Your program has expired. Please enroll in it again.";
        public const string InvitationDeletedSuccessfully = "Invitation has been deleted successfully.";
        public const string InvitationAcceptedSuccessfully = "Invitation has been accepted successfully.";
        public const string NotificationsReturnedSuccessfully = "Notifications are returned successfully.";
        public const string NoNotificationsExist = "No notifications exists.";
        public const string InsufficientBalance = "You account has an insufficient balance.";
        public const string BenefactorInvitationPending = "You Benefactor has not accepted your invitation.";
        #endregion


        #region Transactions
        public const string NoTransactionsExist = "No transactions exist.";
        public const string NoLinkedUsersExist = "No linked accounts exist.";
        #endregion

        #region Organisation
        public const string NoRestaurantsExist = "No restaurants exist.";
        public const string UserRemainingMealsReturned = "Remaining meals of user has been successfully returned.";
        public const string NoOffersExist = "No offers exist.";
        public const string OffersReturnedSuccessfully = "Offers are returned successfully.";
        public const string RestauretMadeFavorite = "Data updated successfully.";
        public const string OrganisationAdminStatusUpdatedSuccessfully = "Organisation level admin status has been changed successfully.";
        public const string OrganisationAdminStatusNotUpdatedSuccessfully = "Organisation level admin status is unable to update.";
        public const string OrganisationAddUpdatedSuccessfully = "Organisation details added/updated successfully.";
        public const string OrganisationNotAddUpdatedSuccessfully = "Organisation details is unable to add/update.";
        public const string OrganisationScheduleAddUpdatedSuccessfully = "Organisation schedule details added/updated successfully.";
        public const string OrganisationScheduleNotAddUpdatedSuccessfully = "Organisation schedule details is unable to add/update.";
        public const string OrganisationDeletedSuccessfully = "Organisation has been deleted successfully.";
        public const string OrganisationNotDeletedSuccessfully = "Organisation has not been deleted successfully.";
        public const string MerchantClonedSuccessfully = "Merchant cloned successfully.";
        public const string MerchantNotClonedSuccessfully = "Merchant has not been cloned successfully.";

        #endregion

        #region Images
        public const string UploadedFileEmpty = "Uploaded file is empty.";
        public const string ImageUploadedSuccessfully = "Image has been uploaded successfully.";
        public const string ExpectedDifferentRequest = "Expected a multipart request, but got {0}.";
        public const string FormCountLimitExceeded = "Form key count limit {0} exceeded.";
        public const string Undefined = "Undefined.";
        public const string ImageDeletedSuccessfully = "Image has been deleted successfully.";
        public const string ImageNotDeletedSuccessfully = "Image is unable to delete successfully.";
        public const string ImageReturnedSuccessfully = "Image is returned successfully.";
        #endregion

        #region Notifications
        public const string UserNotificationNotExists = "No user notifications exist.";
        public const string UserNotificationExists = "User notifications exist and returned.";
        public const string UserNotificationSettingsUpdated = "Notification settings updated successfully.";
        public const string UserNotificationSettingsNotUpdated = "Notification settings are unable to update.";
        public const string NewConnectionAddedMessageNotify = "New connection added. Please check details.";
        public const string NewConnectionAddedTitleNotify = "New connection added";
        public const string PasswordChangedNotificationMessage = "Your password has been changed. Please check the details.";
        public const string PasswordChangedNotificationTitle = "Your password has been changed.";
        public const string ProfileUpdationNotificationMessage = "Your profile has been updated. Please check the details.";
        public const string ProfileUpdatedNotifyTitle = "Your profile has been updated.";
        public const string RejectionConnectionNotificationMessage = "Your request for a connection has been rejected.";
        public const string RejectionConnectionNotificationTitle = "Connection request rejected";
        public const string NewOfferFavNotificationMessage = "You have a new offer from your favorite merchant. Please check details.";
        public const string NewOfferFavNotificationTitle = "The is a new offer from your favorite merchant.";
        public const string NewOfferNotificationMessage = "You have a new offer from a merchant. Please check the details.";
        public const string NewOfferNotificationTitle = "New offer from a merchant.";
        public const string NewRewardNotificationMessage = "You have a new reward from a merchant. Please check the details.";
        public const string NewRewardNotificationTitle = "New reward from a merchant";
        public const string AheadRewardNotificationMessage = "You need one more visit to win a reward from ";
        public const string AheadRewardNotificationTitle = "You need one more visit to win!!";
        public const string AheadRewardNotificationForAmtMessage = "You need 100 more dollars to win a reward from ";
        public const string AheadRewardNotificationForAmtTitle = "Pay an amount to win";
        public const string WonRewardNotificationMessage = "Congratulations!!! You have won a reward from ";
        public const string WonRewardNotificationTitle = "Congrats!! You won!";
        public const string BalanceAddedNotificationMessage = "{Balance} has been credited to your account by {Benefactor}. Please check your balance";
        public const string BalanceAddedNotifyTitle = "Your balance has been credited";
        #endregion

        #region OrganisationProgram
        public const string OrganisationProgramDeletedSuccessfully = "User program has been deleted successfully.";
        public const string OrganisationProgramNotDeletedSuccessfully = "User program has not deleted successfully.";
        #endregion

        #region Plan
        public const string NoPlanExist = "No plan exist.";
        public const string PlanAddUpdatedSuccessfully = "Plan details added/updated successfully.";
        public const string PlanStatusNotUpdatedSuccessfully = "Plan details is unable to add/update.";
        public const string PlanDeletedSuccessfully = "Plan has been deleted successfully.";
        public const string PlanNotDeletedSuccessfully = "Plan has not deleted successfully.";
        public const string PlanClonedSuccessfully = "Plan cloned successfully.";
        public const string PlanNotClonedSuccessfully = "Plan has not cloned successfully.";
        public const string NoMealPlanExist = "No meal plan description given.";
        #endregion

        #region Program Account
        public const string NoProgramAccountExist = "No Program Account exists.";
        public const string ProgramAccountAddUpdatedSuccessfully = "Program Account details added/updated successfully.";
        public const string ProgramAccountStatusNotUpdatedSuccessfully = "Program Account details are unable to add/update.";
        public const string AccountDeletedSuccessfully = "Account has been deleted successfully.";
        public const string AccountNotDeletedSuccessfully = "Account has not deleted successfully.";
        public const string AccountClonedSuccessfully = "Account cloned successfully.";
        public const string AccountNotClonedSuccessfully = "Account not cloned successfully.";
        #endregion

        #region Account Merchant Rule
        public const string RuleAddUpdatedSuccessfully = "Rule details added/updated successfully.";
        public const string RuleStatusNotUpdatedSuccessfully = "Rule details are unable to add/update.";
        #endregion

        #region Branding
        public const string BrandDeletedSuccessfully = "Brand has been deleted successfully.";
        public const string BrandNotDeletedSuccessfully = "Brand has not been deleted successfully.";
        public const string BrandAddUpdatedSuccessfully = "Brand details added/updated successfully.";
        public const string BrandStatusNotUpdatedSuccessfully = "Brand details are unable to add/update.";
        public const string CardExists = "Card number already exists in the system.";
        public const string NoBrandingExist = "No branding card available.";
        public const string BrandingAlreadyExist = "Branding already exists for the selected account.";
        #endregion

        #region Program
        public const string ProgramDeletedSuccessfully = "Program has been deleted successfully.";
        public const string ProgramNotDeletedSuccessfully = "Program has not been deleted successfully.";
        public const string ProgramAddUpdatedSuccessfully = "Program details have been added/updated successfully.";
        public const string ProgramNotAddUpdatedSuccessfully = "Program details are unable to add/update.";
        #endregion

        #region Import User
        public const string NoRecordExists = "No user record exists.";
        public const string UserCodeAlreadyExists = "User code already exists.";
        #endregion

        #region I2c
        public const string CardCreatedSuccessfully = "I2C Card created successfully.";
        public const string VirtualCardAlreadyExists = "Virtual I2C card already exists.";
        public const string NoBankAccountExists = "No bank account exists.";
        public const string i2cKeyName = "i2c";
        public const string BankAccountCreatedSuccessfully = "Bank account created successfully.";
        public const string i2cAccountVerified = "Your account has been verified.";
        public const string BankAccountRemovedSuccessfully = "No account details found.";
        public const string NoAccountDetailFound = "Bank account removed successfully.";
        public const string CardIsNotCreated = "I2C Card has not been created.";
        public const string S3Bucket = "S3Bucket";
        public const string FileDeleteSuccessfully = "File deleted successfully.";
        public const string AWSSetting = "Aws settings are not configured.";
        #endregion

        #region JPOS

        public const string JPOSQRCodeCreatedSuccessfully = "QR code created successfully.";
        public const string JPOSQRCodeUnsuccessfulCreation = "QR code has not been created successfully.";
        public const string JPOSBalanceReturnedSuccessfully = "JPOS balance returned successfully.";
        public const string JPOSBalanceUnsuccessfuleturn = "JPOS balance doesn't return successfully.";
        public const string JPOSTransactionReturnedSuccessfully = "JPOS transactions returned successfully.";
        public const string JPOSTransactionUnsuccessfuleturn = "JPOS transactions doesn't return successfully.";
        public const string JPOSIssuredPropertiesReturnedSuccessfully = "JPOS Issuer Properties return successfully.";
        public const string JPOSIssuredPropertiesReturnedUnsuccessfully = "JPOS Issuer Properties doesn't return successfully.";
        #endregion

        #region Cardholder Agreement
        public const string NoCardholderAgreementExist = "No cardholder agreement exist.";
        public const string CardHolderAgreementPostSuccessfully = "Cardholder agreement submitted successfully.";
        public const string CardHolderAgreementNotPostSuccessfully = "Cardholder agreement has not been submitted successfully.";
        public const string UserCardholderAgreementSettingsUpdated = "Cardholder agreement settings have been updated successfully.";
        public const string UserCardholderAgreementSettingsNotUpdated = "Cardholder agreement settings has not updated successfully.";
        #endregion

        #region Fiserv
        public const string FiservResponseExist = "Fiserv call.";

        #endregion

        #region Sodexo
        public const string InvalidInputData = "User input data is invalid.";
        public const string LoyalityGlobalSuccessfulSetting = "Organisation global setting is saved successful.";
        public const string SiteLevelOverrideSuccessfulSetting = "Site Level Override setting is saved successful.";
        public const string CancelSubscriptionRuleSuccessful = "Susbription Rule is cancelled successful.";
        public const string UserLoyaltyPointshistorySuccessful = "User Loyality points is submitted successful.";
        #endregion
    }
}
