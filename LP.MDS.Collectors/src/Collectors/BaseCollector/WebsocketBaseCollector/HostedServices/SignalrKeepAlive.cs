using System;
using System.Threading;
using System.Threading.Tasks;
using CommandProtocol.Transferable;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using NLog;
using WebsocketBaseCollector.Hubs;

namespace WebsocketBaseCollector.HostedServices
{
    public class SignalrKeepAlive : IHostedService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IHubContext<CollectorHub> collectorHub;
        private Timer timer;


        public SignalrKeepAlive(IHubContext<CollectorHub> collectorHub)
        {
            this.collectorHub = collectorHub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //this.publishKeepAlive( cancellationToken );
            timer = new Timer(BroadcastKeepAliveMessages, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));

            return Task.CompletedTask;
        }

        private void BroadcastKeepAliveMessages(object? state)
        {
            logger.Info("Sending KeepAlive message to clients");
            collectorHub.Clients.All.SendAsync("OnMessage", new KeepAliveMessage()
            {
                Message = "KeepAlive"
            }).Wait();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
