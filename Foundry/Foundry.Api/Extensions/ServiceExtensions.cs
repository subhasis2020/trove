using Foundry.Domain.DbModel;
using Foundry.Identity;
using Foundry.LogService;
using Foundry.Services;
using Foundry.Services.AcquirerService;
using Foundry.Services.Errors;
using Foundry.Services.PartnerNotificationsLogs;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Foundry.Api.Extensions
{
    /// <summary>
    /// This class is used to configure the project along with startup.cs
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// This method is used to configure cors for project.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        /// <summary>
        /// This method is used to configure services used in the project.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
                .AddUserManager<CustomUserManager>()
                .AddDefaultTokenProviders();
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<Role>, RoleStore>();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });
            services.AddTransient<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();
        }
        /// <summary>
        /// This method is used to configure IISIntegration
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }
        /// <summary>
        /// This method is used to configure logger services.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
        /// <summary>
        /// This method is used to configure repositories
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUserClaimRepository, UserClaimRepository>();
            services.AddScoped<IUsersProgram, UsersProgram>();
            services.AddScoped<IResetPassword, ResetPassword>();
            services.AddScoped<IPhotos, Photos>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOrganisation, Services.Organisation>();
            services.AddScoped<IGeneralErrorDetail, GeneralErrorDetail>();
            services.AddScoped<IBenefactorService, BenefactorService>();
            services.AddScoped<IGeneralSettingService, GeneralSettingService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<IUsersGroup, UsersGroup>();
            services.AddScoped<IBenefactorsProgram, BenefactorsProgram>();
            services.AddScoped<IBenefactorNotifications, BenefactorNotifications>();
            services.AddScoped<IPrograms, Programs>();
            services.AddScoped<IUserFavoriteService, UserFavoriteService>();
            services.AddScoped<IReloadBalanceService, ReloadBalanceService>();
            services.AddScoped<IUserTransactionInfoes, UserTransactionInfoes>();
            services.AddScoped<IReloadRule, ReloadRule>();
            services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
            services.AddScoped<IUserNotificationSettingsService, UserNotificationSettingsService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IOrganisationProgram, Services.OrganisationProgram>();
            services.AddScoped<IProgramTypeService, ProgramTypeService>();
            services.AddScoped<IOrganisationSchedule, OrganisationSchedules>();
            services.AddScoped<IOrganisationProgramTypeService, OrganisationProgramTypeService>();
            services.AddScoped<IAdminProgramAccessService, AdminProgramAccessService>();
            services.AddScoped<IOrganisationSchedule, OrganisationSchedules>();
            services.AddScoped<IAccountTypeService, AccountTypeService>();
            services.AddScoped<IAdminProgramAccessService, AdminProgramAccessService>();
            services.AddScoped<IPromotions, Promotions>();
            services.AddScoped<IOfferCodeService, OfferCodeService>();
            services.AddScoped<IOfferTypeService, OfferTypeService>();
            services.AddScoped<IOfferSubTypeService, OfferSubTypeService>();
            services.AddScoped<IMerchantTerminals, MerchantTerminals>();
            services.AddScoped<IMealPeriodService, MealPeriodService>();
            services.AddScoped<IProgramMerchantAccountTypeService, ProgramMerchantAccountTypeService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IPlanProgramAccountLinkingService, PlanProgramAccountLinkingService>();
            services.AddScoped<IProgramAccountService, ProgramAccountService>();
            services.AddScoped<IProgramBrandingService, ProgramBrandingService>();
            services.AddScoped<IBusinessTypeService, BusinessTypeService>();
            services.AddScoped<IAccountMerchantRulesService, AccountMerchantRulesService>();
            services.AddScoped<IAccountMerchantRulesDetailService, AccountMerchantRulesDetailService>();
            services.AddScoped<IUserPlanService, UserPlanService>();
            services.AddScoped<IAcquirerService, AcquirerService>();
            services.AddScoped<II2CAccountDetailService, I2CAccountDetailService>();
            services.AddScoped<II2CLogService, I2CLogService>();
            services.AddScoped<II2CCardBankAccountService, I2CCardBankAccountService>();
            services.AddScoped<IProgramAccountLinkService, ProgramAccountLinkService>();
            services.AddScoped<II2CBank2CardTransferService, I2CBank2CardTransferService>();
            services.AddScoped<IUserPushedNotificationService, UserPushedNotificationService>();
            services.AddScoped<IUserRewardsProgressLinkingService, UserRewardsProgressLinkingService>();
            services.AddScoped<IUserRelationsService, UserRelationsService>();
            services.AddScoped<IUserPushedNotificationsStatusService, UserPushedNotificationsStatusService>();
            services.AddScoped<IMerchantAdminService, MerchantAdminService>();
            services.AddScoped<ICardHolderAgreementService, CardHolderAgreementService>();
            services.AddScoped<ISharedJPOSService, SharedJPOSService>();
            services.AddScoped<IProgramAdminService, ProgramAdminService>();
            services.AddScoped<IJPOSCallLogService, JPOSCallLogService>();
            services.AddScoped<IUserAgreementHistoryService, UserAgreementHistoryService>();
            services.AddScoped<IGatewayCardWebHookTokenService, GatewayCardWebHookTokenService>();
            services.AddScoped<IGatewayRequestResponseLogService, GatewayRequestResponseLogService>();
            services.AddScoped<IFiservPaymentTransactionLogService, FiservPaymentTransactionLogService>();
            services.AddScoped<ILoyalityGlobalSetting, LoyalityGlobalSetting>();
            services.AddScoped<IPartnerNotificationsLogServicer, PartnerNotificationsLogServicer>();
            services.AddScoped<ISiteLevelOverrideSetting, SiteLevelOverrideSetting>();
            services.AddScoped<IUserLoyaltyPointsHistoryInfo, UserLoyaltyPointsHistoryInfo>();
            services.AddScoped<ITranlog, Tranlog>();
            services.AddScoped<IBinDataService, BinDataService>();
            services.AddScoped<IFiservMethods, FiservMethods>();
        }



        /// <summary>
        /// This method is used to configure identity server.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryClients(GetClients(configuration))
                              .AddAspNetIdentity<User>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // base-address of your identityserver
                options.Authority = configuration["AppSettings:Authority"];
                // name of the API resource
                options.Audience = configuration["AppSettings:Audience"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AppSettings:RequireHttpsMetadata"]);
            });
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();
        }

        /// <summary>
        /// This method is used to get identity resource for identity server.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };
        }
        /// <summary>
        /// This method is used to get API resources.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }
        /// <summary>
        /// This method is used to get clients.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            // client credentials client
            return new List<Client>
            {                
                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.angular",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "api1",
                         IdentityServerConstants.StandardScopes.OfflineAccess,
                    },
                    AllowOfflineAccess=true,
                    RefreshTokenUsage=TokenUsage.OneTimeOnly,
                    UpdateAccessTokenClaimsOnRefresh=true,
                    AccessTokenLifetime=Convert.ToInt32(configuration["AccessTokenLifetime"]),  // 10 days
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime=Convert.ToInt32(configuration["SlidingRefreshTokenLifetime"]) // 20 days
                }
            };
        }
    }
    //public static class SwaggerAuthorizeExtensions
    //{
    //    public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<SwaggerAuthorizedMiddleware>();
    //    }
    //}

    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>();
        }
    }
}
