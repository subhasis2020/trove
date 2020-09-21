using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class User
    {
        public User()
        {
            AccountTypeCreatedByNavigation = new HashSet<AccountType>();
            AccountTypeModifiedByNavigation = new HashSet<AccountType>();
            BenefactorProgramBenefactor = new HashSet<BenefactorProgram>();
            BenefactorProgramCreatedByNavigation = new HashSet<BenefactorProgram>();
            BenefactorProgramModifiedByNavigation = new HashSet<BenefactorProgram>();
            BenefactorUsersLinkingBenefactor = new HashSet<BenefactorUsersLinking>();
            BenefactorUsersLinkingCreatedByNavigation = new HashSet<BenefactorUsersLinking>();
            BenefactorUsersLinkingModifiedByNavigation = new HashSet<BenefactorUsersLinking>();
            BenefactorUsersLinkingUser = new HashSet<BenefactorUsersLinking>();
            GroupCreatedByNavigation = new HashSet<Group>();
            GroupModifiedByNavigation = new HashSet<Group>();
            GroupTypeCreatedByNavigation = new HashSet<GroupType>();
            GroupTypeModifiedByNavigation = new HashSet<GroupType>();
            InverseCreatedByNavigation = new HashSet<User>();
            InverseModifiedByNavigation = new HashSet<User>();
            OfferCodeCreatedByNavigation = new HashSet<OfferCode>();
            OfferCodeModifiedByNavigation = new HashSet<OfferCode>();
            OfferCreatedByNavigation = new HashSet<Offer>();
            OfferMerchantCreatedByNavigation = new HashSet<OfferMerchant>();
            OfferMerchantModifiedByNavigation = new HashSet<OfferMerchant>();
            OfferModifiedByNavigation = new HashSet<Offer>();
            OfferSubTypeCreatedByNavigation = new HashSet<OfferSubType>();
            OfferSubTypeModifiedByNavigation = new HashSet<OfferSubType>();
            OfferTypeCreatedByNavigation = new HashSet<OfferType>();
            OfferTypeModifiedByNavigation = new HashSet<OfferType>();
            OrganisationCreatedByNavigation = new HashSet<Organisation>();
            OrganisationModifiedByNavigation = new HashSet<Organisation>();
            OrganisationScheduleCreatedByNavigation = new HashSet<OrganisationSchedule>();
            OrganisationScheduleModifiedByNavigation = new HashSet<OrganisationSchedule>();
            ProgramCreatedByNavigation = new HashSet<Program>();
            ProgramModifiedByNavigation = new HashSet<Program>();
            ProgramPackageCreatedByNavigation = new HashSet<ProgramPackage>();
            ProgramPackageModifiedByNavigation = new HashSet<ProgramPackage>();
            ReloadBalanceRequestBenefactorUser = new HashSet<ReloadBalanceRequest>();
            ReloadBalanceRequestCreatedByNavigation = new HashSet<ReloadBalanceRequest>();
            ReloadBalanceRequestModifiedByNavigation = new HashSet<ReloadBalanceRequest>();
            ReloadBalanceRequestUser = new HashSet<ReloadBalanceRequest>();
            ReloadRulesBenefactorUser = new HashSet<ReloadRules>();
            ReloadRulesCreatedByNavigation = new HashSet<ReloadRules>();
            ReloadRulesModifiedByNavigation = new HashSet<ReloadRules>();
            ReloadRulesUser = new HashSet<ReloadRules>();
            ResetUserPassword = new HashSet<ResetUserPassword>();
            RoleCreatedByNavigation = new HashSet<Role>();
            RoleModifiedByNavigation = new HashSet<Role>();
            UserClaim = new HashSet<UserClaim>();
            UserFavorites = new HashSet<UserFavorites>();
            UserGroup = new HashSet<UserGroup>();
            UserLogin = new HashSet<UserLogin>();
            UserProgramCreatedByNavigation = new HashSet<UserProgram>();
            UserProgramModifiedByNavigation = new HashSet<UserProgram>();
            UserProgramUser = new HashSet<UserProgram>();
            UserRole = new HashSet<UserRole>();
            UserToken = new HashSet<UserToken>();
            UserTransactionInfoCreatedByNavigation = new HashSet<UserTransactionInfo>();
            UserTransactionInfoCreditUser = new HashSet<UserTransactionInfo>();
            UserTransactionInfoDebitUser = new HashSet<UserTransactionInfo>();
            UserTransactionInfoModifiedByNavigation = new HashSet<UserTransactionInfo>();
            UserWallet = new HashSet<UserWallet>();
        }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int? OrganisationId { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string UserCode { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public string Custom6 { get; set; }
        public string Custom7 { get; set; }
        public string Custom8 { get; set; }
        public string Custom9 { get; set; }
        public string Custom10 { get; set; }
        public string Custom11 { get; set; }
        public string Custom12 { get; set; }
        public string UserDeviceId { get; set; }
        public string UserDeviceType { get; set; }
        public string SessionId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<AccountType> AccountTypeCreatedByNavigation { get; set; }
        public virtual ICollection<AccountType> AccountTypeModifiedByNavigation { get; set; }
        public virtual ICollection<BenefactorProgram> BenefactorProgramBenefactor { get; set; }
        public virtual ICollection<BenefactorProgram> BenefactorProgramCreatedByNavigation { get; set; }
        public virtual ICollection<BenefactorProgram> BenefactorProgramModifiedByNavigation { get; set; }
        public virtual ICollection<BenefactorUsersLinking> BenefactorUsersLinkingBenefactor { get; set; }
        public virtual ICollection<BenefactorUsersLinking> BenefactorUsersLinkingCreatedByNavigation { get; set; }
        public virtual ICollection<BenefactorUsersLinking> BenefactorUsersLinkingModifiedByNavigation { get; set; }
        public virtual ICollection<BenefactorUsersLinking> BenefactorUsersLinkingUser { get; set; }
        public virtual ICollection<Group> GroupCreatedByNavigation { get; set; }
        public virtual ICollection<Group> GroupModifiedByNavigation { get; set; }
        public virtual ICollection<GroupType> GroupTypeCreatedByNavigation { get; set; }
        public virtual ICollection<GroupType> GroupTypeModifiedByNavigation { get; set; }
        public virtual ICollection<User> InverseCreatedByNavigation { get; set; }
        public virtual ICollection<User> InverseModifiedByNavigation { get; set; }
        public virtual ICollection<OfferCode> OfferCodeCreatedByNavigation { get; set; }
        public virtual ICollection<OfferCode> OfferCodeModifiedByNavigation { get; set; }
        public virtual ICollection<Offer> OfferCreatedByNavigation { get; set; }
        public virtual ICollection<OfferMerchant> OfferMerchantCreatedByNavigation { get; set; }
        public virtual ICollection<OfferMerchant> OfferMerchantModifiedByNavigation { get; set; }
        public virtual ICollection<Offer> OfferModifiedByNavigation { get; set; }
        public virtual ICollection<OfferSubType> OfferSubTypeCreatedByNavigation { get; set; }
        public virtual ICollection<OfferSubType> OfferSubTypeModifiedByNavigation { get; set; }
        public virtual ICollection<OfferType> OfferTypeCreatedByNavigation { get; set; }
        public virtual ICollection<OfferType> OfferTypeModifiedByNavigation { get; set; }
        public virtual ICollection<Organisation> OrganisationCreatedByNavigation { get; set; }
        public virtual ICollection<Organisation> OrganisationModifiedByNavigation { get; set; }
        public virtual ICollection<OrganisationSchedule> OrganisationScheduleCreatedByNavigation { get; set; }
        public virtual ICollection<OrganisationSchedule> OrganisationScheduleModifiedByNavigation { get; set; }
        public virtual ICollection<Program> ProgramCreatedByNavigation { get; set; }
        public virtual ICollection<Program> ProgramModifiedByNavigation { get; set; }
        public virtual ICollection<ProgramPackage> ProgramPackageCreatedByNavigation { get; set; }
        public virtual ICollection<ProgramPackage> ProgramPackageModifiedByNavigation { get; set; }
        public virtual ICollection<ReloadBalanceRequest> ReloadBalanceRequestBenefactorUser { get; set; }
        public virtual ICollection<ReloadBalanceRequest> ReloadBalanceRequestCreatedByNavigation { get; set; }
        public virtual ICollection<ReloadBalanceRequest> ReloadBalanceRequestModifiedByNavigation { get; set; }
        public virtual ICollection<ReloadBalanceRequest> ReloadBalanceRequestUser { get; set; }
        public virtual ICollection<ReloadRules> ReloadRulesBenefactorUser { get; set; }
        public virtual ICollection<ReloadRules> ReloadRulesCreatedByNavigation { get; set; }
        public virtual ICollection<ReloadRules> ReloadRulesModifiedByNavigation { get; set; }
        public virtual ICollection<ReloadRules> ReloadRulesUser { get; set; }
        public virtual ICollection<ResetUserPassword> ResetUserPassword { get; set; }
        public virtual ICollection<Role> RoleCreatedByNavigation { get; set; }
        public virtual ICollection<Role> RoleModifiedByNavigation { get; set; }
        public virtual ICollection<UserClaim> UserClaim { get; set; }
        public virtual ICollection<UserFavorites> UserFavorites { get; set; }
        public virtual ICollection<UserGroup> UserGroup { get; set; }
        public virtual ICollection<UserLogin> UserLogin { get; set; }
        public virtual ICollection<UserProgram> UserProgramCreatedByNavigation { get; set; }
        public virtual ICollection<UserProgram> UserProgramModifiedByNavigation { get; set; }
        public virtual ICollection<UserProgram> UserProgramUser { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<UserToken> UserToken { get; set; }
        public virtual ICollection<UserTransactionInfo> UserTransactionInfoCreatedByNavigation { get; set; }
        public virtual ICollection<UserTransactionInfo> UserTransactionInfoCreditUser { get; set; }
        public virtual ICollection<UserTransactionInfo> UserTransactionInfoDebitUser { get; set; }
        public virtual ICollection<UserTransactionInfo> UserTransactionInfoModifiedByNavigation { get; set; }
        public virtual ICollection<UserWallet> UserWallet { get; set; }
    }
}
