using Base.Marker;
using FactsetProcessor;
using FactsetProcessor.Mappers;
using FactsetProcessor.V2.FactsetProxy;
using gRPCBaseCollector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCollector
{
    public class Utililes
    {
        public static ServiceProvider setupDI()
        {
            var _service = new ServiceCollection().AddLogging();
            BaseMarker.Init<gRPCBaseCollectorMaker>(_service);
            BaseMarker.Init<ServiceProtocolMarker>(_service);

            _service.AddSingleton<FactsetProxy>();
            _service.AddSingleton<IFieldMapper, FactsetFieldMapper>();
            _service.AddSingleton<MarketDataProcessor, SubscriptionDataProcessImpl>();
            _service.AddSingleton<FactsetSubscriptionDispatcher>();
            _service.AddSingleton<SubscriptionManager, SubscriptionManagerImpl>();
            _service.AddSingleton<ReferenceDataProcessor, ReferenceDataProcessImpl>();

            var _serviceProvider = _service.BuildServiceProvider();

            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            var log = loggerFactory.CreateLogger<ReferenceDataService>();
            _service.AddSingleton(typeof(ILogger), log);

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _service.AddSingleton<IConfiguration>(configuration);

            _serviceProvider = _service.BuildServiceProvider();

            return _serviceProvider;
        }
    }
}
