using Base.Marker;
using MappingProtocol.Repos.GlobalMappings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingProtocol
{
    public class MappingProtocolMarker : BaseMarker
    {
        public override void init(IServiceCollection services)
        {
            services.AddDbContext<Context>();
            services.AddTransient<IGlobalMappingRepo, GlobalMappingRepo>();
        }
    }
}
