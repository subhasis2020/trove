using Foundry.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.ScheduledJobs
{
    public class App
    {
        private readonly IConfiguration _config;
        public App(IConfiguration config)
        {
            _config = config;
        }        // This is now equivalent to Main in Program.cs
        public void Run()
        {
            var logDirectory = _config.GetValue<string>("Runtime:LogOutputDirectory");
        }
    }
}
