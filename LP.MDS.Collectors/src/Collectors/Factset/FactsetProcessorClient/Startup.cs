using Base.Marker;
using FactsetProcessor;
using FactsetProcessor.Mappers;
using FactsetProcessor.proxy;
using FactsetProcessor.V2;
using FactsetProcessor.V2.FactsetProxy;
using FactsetProcessor.Workers;
using FactsetProcessorClient.Executors;
using FactsetProcessorClient.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessorProtocol;
using ServiceProtocol.Services;

namespace FactsetProcessorClient
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            BaseMarker.Init<FactsetProcessorMarker>(services);

            services.AddSingleton<RequestPipeline>();
            services.AddSingleton<IFactsetTickerMapper, NoTickerMapper>();
            services
            .AddSingleton<IClientPublishableService, DummyClientPublisher>()
            .AddSingleton<FactsetSubscriptionDispatcher>()
            .AddSingleton<BulkExecutor>()
            .AddHostedService<FactsetWorker>()
            .AddSingleton<ReferenceDataProcessor, ReferenceProcessImp>()
            .AddSingleton<MarketDataProcessor, SubscriptionDataProcessImpl>()
            .AddSingleton<SubscriptionManager, SubscriptionManagerImpl>()
            .AddSingleton<IFactsetProxy, FactsetProcessor.V2.FactsetProxy.FactsetProxy>();



            services.BuildServiceProvider();

        }
    }
}
