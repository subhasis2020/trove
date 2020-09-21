using Foundry.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using Unity;

namespace Foundry.ScheduledJobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            // return _serviceProvider.GetRequiredService<I2CJob>();
            //_serviceProvider.GetRequiredService<LoyaltyCalculationEngineJob>();
            // return _serviceProvider.GetRequiredService<ReportJob>();

            var jobDetail = bundle.JobDetail;
            return (IJob)_serviceProvider.GetService(jobDetail.JobType);
        }
        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
