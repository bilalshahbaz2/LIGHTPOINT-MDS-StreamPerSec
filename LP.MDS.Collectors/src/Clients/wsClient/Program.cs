using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using NLog;
using wsClient.CollectorClient;

namespace wsClient
{
    class Program
    {
        //private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter when the server is up.");
            Console.ReadLine();

            var logger = LogManager.GetCurrentClassLogger();
            var exitEvent = new ManualResetEvent(false);
            var cts = new CancellationTokenSource();
            logger.Info("Application started. CTRL+C to quit this application.");


            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var CollectorSection = Configuration.GetSection("Collector");
            var collectorURL = CollectorSection.GetValue<string>("Default");
            logger.Info("Connecting to Collector {0}", collectorURL);


            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                cts.Cancel();
                exitEvent.Set();
            };
            SignalRCollectorClient signalRCollectorClient = new SignalRCollectorClient(collectorURL, cts);
            await signalRCollectorClient.Execute();

            exitEvent.WaitOne();
        }


    }
}
