using Microsoft.Extensions.Hosting;
using NLog;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProtocol.HostedServices
{
    public class SnapshotWorker : BackgroundService
    {
        private static ILogger logger = LogManager.GetLogger("snapshot");
        private Timer timer;
        private readonly SubscriptionManager subscriptionManager;

        public SnapshotWorker(SubscriptionManager subscriptionManager) {
            this.subscriptionManager = subscriptionManager;
        }


        private void Dump(object state)
        {
            var totalSubscriptionsCount = this.subscriptionManager.TotalSubscribedTickers();
            var totalconnectedClients = this.subscriptionManager.TotalConnectedClient();
            logger.Info("Prepraing Snapshot, @{stats}", new
            {
                ConnectedClient = totalconnectedClients,
                SubscriptionsCount = totalSubscriptionsCount,
            });
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(Dump, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;

        }
    }
}
