using AutoMapper;
using System;
using ElmahCore;
using ElmahCore.Mvc;
using ElmahCore.Mvc.Notifiers;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Joonasw.AspNetCore.SecurityHeaders;
using Foundry.Web.Extensions;

namespace Foundry.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });
            services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(o =>
        {
            o.LoginPath = "/Account/Login";
            o.LogoutPath = "/Account/Signout";
            o.AccessDeniedPath = "/Account/AccessDenied";
            o.ExpireTimeSpan = TimeSpan.FromDays(7);
        });
            services.AddAutoMapper(typeof(Startup));
            services.AddMvc(

            //By the type  
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddScoped<CustomActionAttribute>();
            services.AddScoped<IsAccountLinkedAttribute>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddElmah<SqlErrorLog>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("FoundryDb"); // DB structure see here: https://bitbucket.org/project-elmah/main/downloads/ELMAH-1.2-db-SQLServer.sql
            });
            EmailOptions emailOptions = new EmailOptions();
            emailOptions.SmtpServer = Configuration["SmtpServer"];
            emailOptions.SmtpPort = Convert.ToInt32(Configuration["SmtpPort"]);
            emailOptions.AuthUserName = Configuration["AuthUserName"];
            emailOptions.AuthPassword = Configuration["AuthPassword"];
            emailOptions.UseSsl = Convert.ToBoolean(Configuration["UseSsl"]);
            emailOptions.MailSubjectFormat = string.Concat(Configuration["MailSubjectFormatProjectName"], " - Instance", " (", Configuration["Instance"], ") - ", Configuration["MailSubjectWithProjectType"]);
            emailOptions.MailRecipient = Configuration["MailRecipient"];
            emailOptions.MailCopyRecipient = Configuration["MailCopyRecipient"];
            emailOptions.MailSender = Configuration["MailSender"];
            services.AddElmah<XmlFileErrorLog>(options =>
            {
                options.Path = @"elmahweblog";
                options.LogPath = "~/logs";
                options.Notifiers.Add(new ErrorMailNotifier("Email", emailOptions));
                options.FiltersConfig = @"elmah.xml";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseAuthentication();
            // Enforce HTTPS in ASP.NET Core
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseElmah();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Index}/{id?}");

            });
          //  app.UseCookiePolicy();

          
            app.Use(async (context, next) =>
            {
                //  Clickjacking - Framable Page
                context.Response.Headers.Add("X-Frame-Options", "DENY"); // This
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN"); // Or this

                // Setting X - XSS - Protection at the Code Level
                context.Response.Headers.Add("X-Xss-Protection", "1");

                //Setting X-Content-Type-Options At The Code Level
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                // Content-Security-Policy header in ASP.NET Core to prevent XSS attacks.
                context.Response.Headers.Add("Content-Security-Policy","script-src 'self'; " + "style-src 'self'; " + "img-src 'self'");
                //context.Response.Headers.Add("Content-Security-Policy","default-src 'self'; report-uri /cspreport");

                // no-referrer
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                // Information Disclosure via Response Header
                // need to check the result
                context.Response.Headers.Remove("Server");

             



                await next();
            });

            //HTTP Strict Transport Security (HSTS) in ASP.NET Core
            if (env.IsDevelopment() == false)
            {
               // need to uncomment the below line of code
              //  app.UseHttpsEnforcement();
                app.UseHsts(new HstsOptions
                {
                    Seconds = 30 * 24 * 60 * 60,
                    IncludeSubDomains = false,
                    Preload = false
                });
            }
        }
    }
}
