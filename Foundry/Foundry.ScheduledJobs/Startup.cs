using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Domain.DbModel;
using Foundry.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;


namespace Foundry.ScheduledJobs
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<QuartzJobRunner>();
            services.AddHostedService<FoundryService>();
            if (configuration["EnabledJob:NotificationsJob"] == "1")
            {
                services.AddSingleton<NotificationJob>();
                services.AddSingleton(new JobSchedule(
                jobType: typeof(NotificationJob),
                cronExpression: configuration["JobSchedule:NotificationsJob"])); //every 10 seconds
            }

            if (configuration["EnabledJob:I2CJob"] == "1")
            {
                services.AddSingleton<I2CJob>();
                services.AddSingleton(new JobSchedule(
                jobType: typeof(I2CJob),
                cronExpression: configuration["JobSchedule:I2CJob"])); //every 10 seconds
            }
            if (configuration["EnabledJob:LoyaltyCalculationEngineJob"] == "1")
            {
                services.AddSingleton<LoyaltyCalculationEngineJob>();
                services.AddSingleton(new JobSchedule(
                jobType: typeof(LoyaltyCalculationEngineJob),
                cronExpression: configuration["JobSchedule:LoyaltyCalculationEngineJob"])); //every 59 seconds
            }
            if (configuration["EnabledJob:ReportJob"] == "1")
            {
                services.AddSingleton<ReportJob>();
                services.AddSingleton(new JobSchedule(
                jobType: typeof(ReportJob),
                cronExpression: configuration["JobSchedule:ReportJob"])); //every 10 seconds
            }

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
           // , Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            
            //loggerFactory.AddLog4Net();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Run(async (context) =>
            {
               // await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
