using Base.Marker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Logger.Marker
{
    public class LoggerMarker : BaseMarker
    {
   
        public override void init(IServiceCollection service)
        {
            //service.AddSingleton<ILoggerManager, LoggerManager>();
            service.AddLogging();
        }
    }
}
