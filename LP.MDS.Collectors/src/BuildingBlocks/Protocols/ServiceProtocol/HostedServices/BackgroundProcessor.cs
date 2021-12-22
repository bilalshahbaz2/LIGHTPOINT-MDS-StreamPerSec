using Microsoft.Extensions.Hosting;
using NLog;
using ServiceProtocol.Dispatchers;
using ServiceProtocol.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProtocol.HostedServices
{
    public class BackgroundProcessor : IHostedService
    {

        private readonly IBackgroundDispatcher backgroundDispatcher;
        private readonly IFactoryDataService factoryDataService;

        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public BackgroundProcessor(IBackgroundDispatcher _backgroundDispatcher, IFactoryDataService _factoryDataService)
        {
            backgroundDispatcher = _backgroundDispatcher;
            factoryDataService = _factoryDataService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.ConsumeBackground(cancellationToken);

            return Task.CompletedTask;
        }

        private void ConsumeBackground(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested) 
                {
                    var incomginrequest = await this.backgroundDispatcher.RecieveAsync();
                    factoryDataService.PostAsync(incomginrequest);
                    logger.Info($"Backgorund processor - Recieved - CorrelationID - {incomginrequest.CorrelationId}");
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
