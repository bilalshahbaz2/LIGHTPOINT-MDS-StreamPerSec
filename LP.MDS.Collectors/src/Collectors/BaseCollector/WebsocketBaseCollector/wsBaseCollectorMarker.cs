using System;
using Base.Marker;
using Microsoft.Extensions.DependencyInjection;
using WebsocketBaseCollector.HostedServices;

namespace WebsocketBaseCollector
{
    public class wsBaseCollectorMarker : BaseMarker
    {

        public override void init(IServiceCollection services)
        {
            services.AddHostedService<SignalrKeepAlive>();

        }
    }
}
