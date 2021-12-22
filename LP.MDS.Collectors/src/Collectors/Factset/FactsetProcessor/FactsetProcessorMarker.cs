using Base.Marker;
using FactSet.Datafeed;
using FactsetProcessor.Mappers;
using FactsetProcessor.proxy;
using FactsetProcessor.V2;
using FactsetProcessor.V2.FactsetProxy;
using FactsetProcessor.Workers;
using gRPCFactset;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessorProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FactsetProcessor
{
    public class FactsetProcessorMarker : BaseMarker
    {
        public override void init(IServiceCollection services)
        {
            services.AddSingleton<FactsetConfiguration>();
            services.AddSingleton<IFieldMapper, FactsetFieldMapper>();
            services.AddSingleton<IFactsetTickerMapper, FactsetTickerMapper>();

            services.AddSingleton<IFactsetProxy, FactsetProxy>();

            services.AddSingleton<MarketDataProcessor, SubscriptionDataProcessImpl>();

            services.AddSingleton<ReferenceDataProcessor, ReferenceProcessImp>();
            services.AddSingleton<SearchDataProcessor, SearchProcessImp>();

            services.AddSingleton<FactsetSubscriptionDispatcher>();
            //services.AddHostedService<FactsetWorker>();

        }



    }
}
