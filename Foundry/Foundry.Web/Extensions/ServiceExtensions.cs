using Foundry.Domain.DbModel;
using Foundry.Identity;
using Foundry.LogService;
using Foundry.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Foundry.Web.Extensions
{
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
        }
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {

            services.Configure<IISOptions>(options =>
            {

            });
        }
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

        }
    }
}
