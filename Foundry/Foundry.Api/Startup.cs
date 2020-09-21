using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using ElmahCore;
using ElmahCore.Mvc;
using ElmahCore.Mvc.Notifiers;
using ElmahCore.Sql;
using Foundry.Api.Attributes;
using Foundry.Api.Extensions;
using Foundry.Domain.ApiModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using Swashbuckle.AspNetCore.Swagger;

namespace Foundry.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Services.IDatabaseConnectionFactory>(provider => new Services.DatabaseConnectionFactory(Configuration.GetConnectionString("FoundryDb")));
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLoggerService();
            services.ConfigureRepositories();
            services.ConfigureIdentity();
            services.AddSingleton<IConfiguration>(Configuration);
            services.ConfigureIdentityServer(Configuration);
            services.AddScoped<IsOrganisationActiveAttribute>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Foundry Mobile Application", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
                //// Set the comments path for the Swagger JSON and UI.
                var xmlFile = "Foundry.Api.xml";
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            //    c.DocumentFilter<SwaggerAuthorizationFilter>();
            });

            services.Configure<SendGridSettings>(Configuration.GetSection("SendGridSettings"));


            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.AddAutoMapper();
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperExtension());
            });

            mappingConfig.CreateMapper();
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
                options.Path = @"elmahapilog";
                options.LogPath = "~/logs";
                options.Notifiers.Add(new ErrorMailNotifier("Email", emailOptions));
                options.FiltersConfig = @"elmah.xml";
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY"); // This
             //   context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN"); // Or this

                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                // Information Disclosure via Response Header
                // need to check the result
                context.Response.Headers.Remove("Server");
                await next();
            });

            //app.UseForwardedHeaders will forward proxy headers to the current request.This will help us during the Linux deployment.

            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.All
            //});

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseMvc();
            app.UseStaticFiles();

           // app.UseSwaggerAuthorized();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.  
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foundry Mobile API V1");
                c.RoutePrefix = string.Empty;
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseElmah();
          
        }
    }
}
