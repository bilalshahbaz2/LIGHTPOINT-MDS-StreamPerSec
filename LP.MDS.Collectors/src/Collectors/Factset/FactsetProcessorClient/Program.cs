using FactsetProcessorClient.Executors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FactsetProcessorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Info("Application started. CTRL+C to quit this application.");

            var exitEvent = new ManualResetEvent(false);
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                cts.Cancel();
                exitEvent.Set();
            };

            using (var host = CreateHostBuilder(args).Build())
            {
                logger.Info("Starting Hosted Builder");

                await host.StartAsync(cts.Token);
                logger.Info("Host Builder has started");


                var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
                var executor = host.Services.GetRequiredService<BulkExecutor>();

                logger.Warn("Running Custom Code");


                _ = executor.Execute(cts.Token);

                exitEvent.WaitOne();
                lifetime.StopApplication();
                await host.WaitForShutdownAsync(cts.Token);
            }
        }

       
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            return Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices((hostContext, services) =>
                {
                    var startup = new Startup(Configuration);
                    startup.ConfigureServices(services);
                });
        }
    }
}