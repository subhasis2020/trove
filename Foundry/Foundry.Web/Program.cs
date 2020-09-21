using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Foundry.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            ////Information Disclosure via Response Header
            //.UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>();
    }
}
