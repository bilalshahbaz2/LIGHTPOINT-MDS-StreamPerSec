using System;
using Base.Marker;
using Microsoft.Extensions.DependencyInjection;
using ProcessorProtocol;
using VisualsetProcessor.Generator;
using VisualsetProcessor.Processor;

namespace VisualsetProcessor
{
    public class VisualsetProcessorMarker : BaseMarker
    {
        public override void init(IServiceCollection services)
        {
            base.init(services);

            services.AddSingleton<ReferenceDataProcessor, ReferenceDataProcessorImpl>();
            services.AddSingleton<MarketDataProcessor, SubscriptionDataProcessorImpl>();
            services.AddHostedService<RealtimeGenerator>();
        }
    }
}
