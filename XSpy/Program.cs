using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XSpy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseKestrel(options =>
                        {
                            options.Limits.MaxRequestBodySize = null;
                            options.Limits.MaxRequestBufferSize = null;
                        }).UseUrls("http://*:5000")
                        .UseSentry("https://b6659bf76fc14969b5fa31ca02cf82a1@o439063.ingest.sentry.io/5721589");
                });
    }
}