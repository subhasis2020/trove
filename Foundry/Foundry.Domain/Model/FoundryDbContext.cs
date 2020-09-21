using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Foundry.Domain.Model
{
    public partial class FoundryDbContext : DbContext
    {
        public FoundryDbContext()
        {
        }

        public FoundryDbContext(DbContextOptions<FoundryDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<BenefactorProgram> BenefactorProgram { get; set; }
        public virtual DbSet<BenefactorUsersLinking> BenefactorUsersLinking { get; set; }
        public virtual DbSet<ElmahError> ElmahError { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<ErrorMessagesDetail> ErrorMessagesDetail { get; set; }
        public virtual DbSet<GeneralSetting> GeneralSetting { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<GroupType> GroupType { get; set; }
        public virtual DbSet<Invitation> Invitation { get; set; }
        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<OfferCode> OfferCode { get; set; }
        public virtual DbSet<OfferGroup> OfferGroup { get; set; }
        public virtual DbSet<OfferMerchant> OfferMerchant { get; set; }
        public virtual DbSet<OfferSubType> OfferSubType { get; set; }
        public virtual DbSet<OfferType> OfferType { get; set; }
        public virtual DbSet<Organisation> Organisation { get; set; }
        public virtual DbSet<OrganisationGroup> OrganisationGroup { get; set; }
        public virtual DbSet<OrganisationMapping> OrganisationMapping { get; set; }
        public virtual DbSet<OrganisationProgram> OrganisationProgram { get; set; }
        public virtual DbSet<OrganisationSchedule> OrganisationSchedule { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<Program> Program { get; set; }
        public virtual DbSet<ProgramAccountLinking> ProgramAccountLinking { get; set; }
        public virtual DbSet<ProgramGroup> ProgramGroup { get; set; }
        public virtual DbSet<ProgramMerchant> ProgramMerchant { get; set; }
        public virtual DbSet<ProgramMerchantAccountType> ProgramMerchantAccountType { get; set; }
        public virtual DbSet<ProgramPackage> ProgramPackage { get; set; }
        public virtual DbSet<ReloadBalanceRequest> ReloadBalanceRequest { get; set; }
        public virtual DbSet<ReloadRules> ReloadRules { get; set; }
        public virtual DbSet<ResetUserPassword> ResetUserPassword { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleClaim> RoleClaim { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserClaim> UserClaim { get; set; }
        public virtual DbSet<UserFavorites> UserFavorites { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserProgram> UserProgram { get; set; }
        public virtual DbSet<UserRelations> UserRelations { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserToken> UserToken { get; set; }
        public virtual DbSet<UserTransactionInfo> UserTransactionInfo { get; set; }
        public virtual DbSet<UserWallet> UserWallet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=FoundryDb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountType1)
                    .IsRequired()
                    .HasColumnName("accountType")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(700)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AccountTypeCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_AccountType_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.AccountTypeModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AccountType_ModifiedUser");
            });

            modelBuilder.Entity<BenefactorProgram>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BenefactorId).HasColumnName("benefactorId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.ProgramPackageId).HasColumnName("programPackageId");

                entity.HasOne(d => d.Benefactor)
                    .WithMany(p => p.BenefactorProgramBenefactor)
                    .HasForeignKey(d => d.BenefactorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefactorProgram_User");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.BenefactorProgramCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_BenefactorProgram_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.BenefactorProgramModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_BenefactorProgram_ModifiedUser");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.BenefactorProgram)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefactorProgram_Program");

                entity.HasOne(d => d.ProgramPackage)
                    .WithMany(p => p.BenefactorProgram)
                    .HasForeignKey(d => d.ProgramPackageId)
                    .HasConstraintName("FK_BenefactorProgram_ProgramPackage");
            });

            modelBuilder.Entity<BenefactorUsersLinking>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BenefactorId).HasColumnName("benefactorId");

                entity.Property(e => e.CanViewTransaction)
                    .HasColumnName("canViewTransaction")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsInvitationSent).HasColumnName("isInvitationSent");

                entity.Property(e => e.IsRequestAccepted).HasColumnName("isRequestAccepted");

                entity.Property(e => e.LinkedDateTime)
                    .HasColumnName("linkedDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.RelationshipId).HasColumnName("relationshipId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Benefactor)
                    .WithMany(p => p.BenefactorUsersLinkingBenefactor)
                    .HasForeignKey(d => d.BenefactorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefactorUsersLinking_BenefactorUser");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.BenefactorUsersLinkingCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_BenefactorUsersLinking_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.BenefactorUsersLinkingModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_BenefactorUsersLinking_ModifiedUser");

                entity.HasOne(d => d.Relationship)
                    .WithMany(p => p.BenefactorUsersLinking)
                    .HasForeignKey(d => d.RelationshipId)
                    .HasConstraintName("FK_BenefactorUsersLinking_UserRelations");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BenefactorUsersLinkingUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefactorUsersLinking_User");
            });

            modelBuilder.Entity<ElmahError>(entity =>
            {
                entity.HasKey(e => e.ErrorId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("ELMAH_Error");

                entity.Property(e => e.ErrorId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AllXml).IsRequired();

                entity.Property(e => e.Application)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Sequence).ValueGeneratedOnAdd();

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.TimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.User)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.Property(e => e.Bccemail)
                    .HasColumnName("BCCEmail")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Body).IsRequired();

                entity.Property(e => e.Ccemail)
                    .HasColumnName("CCEmail")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ErrorMessagesDetail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ErrorMessages).HasMaxLength(200);

                entity.Property(e => e.ErrorParameterType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<GeneralSetting>(entity =>
            {
                entity.Property(e => e.KeyGroup)
                    .HasColumnName("keyGroup")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.KeyName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(700)
                    .IsUnicode(false);

                entity.Property(e => e.GroupType).HasColumnName("groupType");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.GroupCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Group_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.GroupModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Group_ModifiedUser");
            });

            modelBuilder.Entity<GroupType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.GroupType1)
                    .IsRequired()
                    .HasColumnName("groupType")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.GroupTypeCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_GroupType_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.GroupTypeModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_GroupType_ModifiedUser");
            });

            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ImagePath).HasMaxLength(500);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsRequestAccepted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.RelationshipId).HasColumnName("relationshipId");
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BuyQuantity).HasColumnName("buyQuantity");

                entity.Property(e => e.CouponCode)
                    .HasColumnName("couponCode")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DiscountInCash)
                    .HasColumnName("discountInCash")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.DiscountInPercentage)
                    .HasColumnName("discountInPercentage")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FreeQuantity).HasColumnName("freeQuantity");

                entity.Property(e => e.GetQuantity).HasColumnName("getQuantity");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsCouponValid).HasColumnName("isCouponValid");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.OfferDayName).HasMaxLength(100);

                entity.Property(e => e.OfferSubTypeId).HasColumnName("offerSubTypeId");

                entity.Property(e => e.OfferTypeId).HasColumnName("offerTypeId");

                entity.Property(e => e.OfferValidFrom)
                    .HasColumnName("offerValidFrom")
                    .HasColumnType("datetime");

                entity.Property(e => e.OfferValidTill)
                    .HasColumnName("offerValidTill")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.VisitNumber).HasColumnName("visitNumber");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OfferCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Offer_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OfferModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Offer_ModifiedUser");

                entity.HasOne(d => d.OfferSubType)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.OfferSubTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Offer_OfferSubType");

                entity.HasOne(d => d.OfferType)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.OfferTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Offer_OfferType");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_Offer_Program");
            });

            modelBuilder.Entity<OfferCode>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.OfferIconPath)
                    .HasColumnName("offerIconPath")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.OfferName)
                    .IsRequired()
                    .HasColumnName("offerName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OfferCodeCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_OfferCode_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OfferCodeModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_OfferCode_User1");
            });

            modelBuilder.Entity<OfferGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GroupId).HasColumnName("groupId");

                entity.Property(e => e.OfferId).HasColumnName("offerId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.OfferGroup)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfferGroup_Group");

                entity.HasOne(d => d.Offer)
                    .WithMany(p => p.OfferGroup)
                    .HasForeignKey(d => d.OfferId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfferGroup_Offer");
            });

            modelBuilder.Entity<OfferMerchant>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MerchantId).HasColumnName("merchantId");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.OfferId).HasColumnName("offerId");

                entity.Property(e => e.OfferLeftDate)
                    .HasColumnName("offerLeftDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OfferMerchantCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_OfferMerchant_User");

                entity.HasOne(d => d.Merchant)
                    .WithMany(p => p.OfferMerchant)
                    .HasForeignKey(d => d.MerchantId)
                    .HasConstraintName("FK_OfferMerchant_Organisation");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OfferMerchantModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_OfferMerchant_ModifiedUser");

                entity.HasOne(d => d.Offer)
                    .WithMany(p => p.OfferMerchant)
                    .HasForeignKey(d => d.OfferId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfferMerchant_Offer");
            });

            modelBuilder.Entity<OfferSubType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(700)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.OfferTypeId).HasColumnName("offerTypeId");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OfferSubTypeCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_OfferSubType_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OfferSubTypeModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_OfferSubType_ModifiedUser");

                entity.HasOne(d => d.OfferCode)
                    .WithMany(p => p.OfferSubType)
                    .HasForeignKey(d => d.OfferCodeId)
                    .HasConstraintName("FK__OfferSubT__Offer__3528CC84");

                entity.HasOne(d => d.OfferType)
                    .WithMany(p => p.OfferSubType)
                    .HasForeignKey(d => d.OfferTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfferSubType_OfferType");
            });

            modelBuilder.Entity<OfferType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.OfferType1)
                    .IsRequired()
                    .HasColumnName("offerType")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OfferTypeCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_OfferType_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OfferTypeModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_OfferType_ModifiedUser");
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddressLine1)
                    .IsRequired()
                    .HasColumnName("addressLine1")
                    .HasMaxLength(500);

                entity.Property(e => e.AddressLine2)
                    .HasColumnName("addressLine2")
                    .HasMaxLength(500);

                entity.Property(e => e.ContactNumber)
                    .HasColumnName("contactNumber")
                    .HasMaxLength(17);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasColumnName("emailAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsMaster)
                    .HasColumnName("isMaster")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(300);

                entity.Property(e => e.MaxCapacity).HasColumnName("maxCapacity");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(300);

                entity.Property(e => e.OrganisationType).HasColumnName("organisationType");

                entity.Property(e => e.WebsiteUrl)
                    .HasColumnName("websiteURL")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OrganisationCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Organisation_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OrganisationModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Organisation_ModifiedUser");
            });

            modelBuilder.Entity<OrganisationGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GroupId).HasColumnName("groupId");

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.OrganisationGroup)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_OrganisationGroup_Group");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationGroup)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationGroup_Organisation");
            });

            modelBuilder.Entity<OrganisationMapping>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.ParentOrganisationId).HasColumnName("parentOrganisationId");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationMappingOrganisation)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_OrganisationMapping");

                entity.HasOne(d => d.ParentOrganisation)
                    .WithMany(p => p.OrganisationMappingParentOrganisation)
                    .HasForeignKey(d => d.ParentOrganisationId)
                    .HasConstraintName("FK_Organisation_OrganisationMappingParent");
            });

            modelBuilder.Entity<OrganisationProgram>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.OrganisationProgram)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_OrganisationProgram_Program");
            });

            modelBuilder.Entity<OrganisationSchedule>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClosedTime).HasColumnName("closedTime");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.OpenTime).HasColumnName("openTime");

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.WorkingDay)
                    .IsRequired()
                    .HasColumnName("workingDay")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.OrganisationScheduleCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_OrganisationSchedule_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.OrganisationScheduleModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_OrganisationSchedule_ModifiedUser");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationSchedule)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationSchedule_Organisation");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnName("entityId");

                entity.Property(e => e.PhotoPath)
                    .IsRequired()
                    .HasColumnName("photoPath")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoType).HasColumnName("photoType");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ColorCode)
                    .HasColumnName("colorCode")
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(700)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnName("endDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LogoPath).HasColumnName("logoPath");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.ProgramExpiryDuration).HasColumnName("programExpiryDuration");

                entity.Property(e => e.StartDate)
                    .HasColumnName("startDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProgramCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Program_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProgramModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Program_ModifiedUser");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Program)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Program_Organisation");
            });

            modelBuilder.Entity<ProgramAccountLinking>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountTypeId).HasColumnName("accountTypeId");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.ProgramAccountLinking)
                    .HasForeignKey(d => d.AccountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramAccountLinking_AccountType");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.ProgramAccountLinking)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramAccountLinking_Program");
            });

            modelBuilder.Entity<ProgramGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GroupId).HasColumnName("groupId");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.ProgramGroup)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_ProgramGroup_Group");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.ProgramGroup)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramGroup_Program");
            });

            modelBuilder.Entity<ProgramMerchant>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.ProgramMerchant)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramMerchant_Organisation");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.ProgramMerchant)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramMerchant_Program");
            });

            modelBuilder.Entity<ProgramMerchantAccountType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.ProgramAccountLinkingId).HasColumnName("programAccountLinkingId");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.ProgramMerchantAccountType)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramMerchantAccountType_Organisation");

                entity.HasOne(d => d.ProgramAccountLinking)
                    .WithMany(p => p.ProgramMerchantAccountType)
                    .HasForeignKey(d => d.ProgramAccountLinkingId)
                    .HasConstraintName("FK_ProgramMerchantAccountType_ProgramAccountLinking");
            });

            modelBuilder.Entity<ProgramPackage>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.NoOfFlexPoints).HasColumnName("noOfFlexPoints");

                entity.Property(e => e.NoOfMealPasses).HasColumnName("noOfMealPasses");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProgramPackageCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProgramPackage_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProgramPackageModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProgramPackage_ModifiedUser");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.ProgramPackage)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramPackage_Program");
            });

            modelBuilder.Entity<ReloadBalanceRequest>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BenefactorUserId).HasColumnName("benefactorUserId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsRequestAccepted).HasColumnName("isRequestAccepted");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.RequestedAmount)
                    .HasColumnName("requestedAmount")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.BenefactorUser)
                    .WithMany(p => p.ReloadBalanceRequestBenefactorUser)
                    .HasForeignKey(d => d.BenefactorUserId)
                    .HasConstraintName("FK_ReloadBalanceRequest_BenefactorLink");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ReloadBalanceRequestCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ReloadBalanceRequest_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ReloadBalanceRequestModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ReloadBalanceRequest_ModifiedUser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReloadBalanceRequestUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReloadBalanceRequest_UserLink");
            });

            modelBuilder.Entity<ReloadRules>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BenefactorUserId).HasColumnName("benefactorUserId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsAutoReloadAmount).HasColumnName("isAutoReloadAmount");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.ReloadAmount)
                    .HasColumnName("reloadAmount")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UserDroppedAmount)
                    .HasColumnName("userDroppedAmount")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.BenefactorUser)
                    .WithMany(p => p.ReloadRulesBenefactorUser)
                    .HasForeignKey(d => d.BenefactorUserId)
                    .HasConstraintName("FK_ReloadRules_BenefactorLink");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ReloadRulesCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ReloadRules_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ReloadRulesModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ReloadRules_ModifiedUser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReloadRulesUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReloadRules_UserLink");
            });

            modelBuilder.Entity<ResetUserPassword>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.InviteeId).HasColumnName("inviteeId");

                entity.Property(e => e.IsPasswordReset)
                    .HasColumnName("isPasswordReset")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ResetToken)
                    .HasColumnName("resetToken")
                    .HasMaxLength(20);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.ValidTill)
                    .HasColumnName("validTill")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ResetUserPassword)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ResetUserPassword_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrencyStamp");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.Property(e => e.NormalizedName)
                    .HasColumnName("normalizedName")
                    .HasMaxLength(256);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.RoleCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Role_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.RoleModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Role_ModifiedUser");
            });

            modelBuilder.Entity<RoleClaim>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaim)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccessFailedCount).HasColumnName("accessFailedCount");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(700);

                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrencyStamp");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Custom1)
                    .HasColumnName("custom1")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom10)
                    .HasColumnName("custom10")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom11)
                    .HasColumnName("custom11")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom12)
                    .HasColumnName("custom12")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom2)
                    .HasColumnName("custom2")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom3)
                    .HasColumnName("custom3")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom4)
                    .HasColumnName("custom4")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom5)
                    .HasColumnName("custom5")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom6)
                    .HasColumnName("custom6")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom7)
                    .HasColumnName("custom7")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom8)
                    .HasColumnName("custom8")
                    .HasMaxLength(300);

                entity.Property(e => e.Custom9)
                    .HasColumnName("custom9")
                    .HasMaxLength(300);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(50);

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(300);

                entity.Property(e => e.LockoutEnabled).HasColumnName("lockoutEnabled");

                entity.Property(e => e.LockoutEnd).HasColumnName("lockoutEnd");

                entity.Property(e => e.MiddleName)
                    .HasColumnName("middleName")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NormalizedEmail)
                    .HasColumnName("normalizedEmail")
                    .HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName)
                    .HasColumnName("normalizedUserName")
                    .HasMaxLength(256);

                entity.Property(e => e.OrganisationId).HasColumnName("organisationId");

                entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phoneNumber")
                    .HasMaxLength(20);

                entity.Property(e => e.SecurityStamp).HasColumnName("securityStamp");

                entity.Property(e => e.SessionId)
                    .HasColumnName("sessionId")
                    .HasMaxLength(50);

                entity.Property(e => e.TwoFactorEnabled).HasColumnName("twoFactorEnabled");

                entity.Property(e => e.UserCode)
                    .HasColumnName("userCode")
                    .HasMaxLength(30);

                entity.Property(e => e.UserDeviceId)
                    .HasColumnName("userDeviceId")
                    .HasMaxLength(300);

                entity.Property(e => e.UserDeviceType)
                    .HasColumnName("userDeviceType")
                    .HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(256);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_User_User");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.InverseModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_User_ModifiedUser");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.OrganisationId)
                    .HasConstraintName("FK_User_Organisation");
            });

            modelBuilder.Entity<UserClaim>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClaim)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserFavorites>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsFavorite).HasColumnName("isFavorite");

                entity.Property(e => e.OrgnisationId).HasColumnName("orgnisationId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Orgnisation)
                    .WithMany(p => p.UserFavorites)
                    .HasForeignKey(d => d.OrgnisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFavorites_Organisation");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFavorites)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFavorites_User");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GroupId).HasColumnName("groupId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UserGroup)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserGroup_Group");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserGroup)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserGroup_User");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserProgram>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLinkedProgram)
                    .HasColumnName("isLinkedProgram")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsVerificationCodeDone)
                    .HasColumnName("isVerificationCodeDone")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LinkAccountVerificationCode)
                    .HasColumnName("linkAccountVerificationCode")
                    .HasMaxLength(10);

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.ProgramPackageId).HasColumnName("programPackageId");

                entity.Property(e => e.UserEmailAddress)
                    .HasColumnName("userEmailAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.VerificationCodeValidTill)
                    .HasColumnName("verificationCodeValidTill")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.UserProgramCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_UserProgram_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.UserProgramModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_UserProgram_ModifiedUser");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.UserProgram)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProgram_Program");

                entity.HasOne(d => d.ProgramPackage)
                    .WithMany(p => p.UserProgram)
                    .HasForeignKey(d => d.ProgramPackageId)
                    .HasConstraintName("FK_UserProgram_ProgramPackage");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserProgramUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProgram_User");
            });

            modelBuilder.Entity<UserRelations>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(700)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnName("displayOrder");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.RelationName)
                    .IsRequired()
                    .HasColumnName("relationName")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserToken)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserTransactionInfo>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountTypeId).HasColumnName("accountTypeId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.CreditUserId).HasColumnName("creditUserId");

                entity.Property(e => e.DebitUserId).HasColumnName("debitUserId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.PeriodRemark)
                    .HasColumnName("periodRemark")
                    .HasMaxLength(100);

                entity.Property(e => e.ProgramId).HasColumnName("programId");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnName("transactionAmount")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TransactionDate)
                    .HasColumnName("transactionDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.UserTransactionInfo)
                    .HasForeignKey(d => d.AccountTypeId)
                    .HasConstraintName("FK_UserTransactionInfo_AccountType");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.UserTransactionInfoCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_UserTransactionInfo_User");

                entity.HasOne(d => d.CreditUser)
                    .WithMany(p => p.UserTransactionInfoCreditUser)
                    .HasForeignKey(d => d.CreditUserId)
                    .HasConstraintName("FK_UserTransactionInfo_CreditUser");

                entity.HasOne(d => d.DebitUser)
                    .WithMany(p => p.UserTransactionInfoDebitUser)
                    .HasForeignKey(d => d.DebitUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserTransactionInfo_DebitUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.UserTransactionInfoModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_UserTransactionInfo_ModifiedUser");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.UserTransactionInfo)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_UserTransactionInfo_Program");
            });

            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountTypeId).HasColumnName("accountTypeId");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnName("expirationDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.UserProgramId).HasColumnName("userProgramId");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.UserWallet)
                    .HasForeignKey(d => d.AccountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserWallet_AccountType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserWallet)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserWallet_User");

                entity.HasOne(d => d.UserProgram)
                    .WithMany(p => p.UserWallet)
                    .HasForeignKey(d => d.UserProgramId)
                    .HasConstraintName("FK_UserWallet_UserProgram");
            });
        }
    }
}
