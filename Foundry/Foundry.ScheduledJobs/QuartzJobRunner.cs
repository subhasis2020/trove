using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.ScheduledJobs
{
    [DisallowConcurrentExecution]
    public class QuartzJobRunner : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public QuartzJobRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
 
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var job = scope.ServiceProvider.GetRequiredService(context.JobDetail.JobType) as IJob;

                await job.Execute(context);
            }
        }
    }
}
