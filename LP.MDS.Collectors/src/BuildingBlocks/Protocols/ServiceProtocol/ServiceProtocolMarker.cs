using Base.Marker;
using CommandProtocol.Requestable;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceProtocol.Dispatchers;
using ServiceProtocol.HostedServices;
using ServiceProtocol.Services;
using ServiceProtocol.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceProtocol
{
    public class ServiceProtocolMarker : BaseMarker
    {
        public override void init(IServiceCollection services)
        {
			services.AddTransient<IncomingRequestValidator>();
            services.AddSingleton<IReferenceDataService, ReferenceDataService>();
            services.AddSingleton<IMarketDataService, MarketDataService>();
			services.AddSingleton<IFactoryDataService, FactoryDataService>();
            services.AddSingleton<IUnSubscribeService, UnSubscribeService>();
            
            services.AddSingleton<SubscriptionManager, SubscriptionManagerImpl>();
            services.AddSingleton<IClientPublishableService, ClientPublishableService>();
            services.AddSingleton<IFactoryDataService, FactoryDataService>();
            services.AddSingleton<IBackgroundDispatcher, BackgroundDispatcher>();
            services.AddSingleton<BloombergCollector>();

            services.AddHostedService<SnapshotWorker>();
            services.AddHostedService<BackgroundProcessor>();

        }
    }
}
