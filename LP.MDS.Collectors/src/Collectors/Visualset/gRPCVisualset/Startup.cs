using AutoMapper;
using Base.Marker;
using CommandProtocol.Transferable;
using gRPCBaseCollector;
using gRPCBaseCollector.Mappers;
using GrpcService.MDS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using VisualsetProcessor;
using VisualsetProcessor.Generator;
using VisualsetProcessor.Processor;

namespace gRPCVisualset
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        BaseMarker.Init<ServiceProtocolMarker>(services);
        BaseMarker.Init<gRPCBaseCollectorMaker>(services);
        BaseMarker.Init<VisualsetProcessorMarker>(services);

        //Auto Mapper
        services.AddAutoMapper(typeof(Startup));
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperProfiles());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.AddSingleton<IOutgoingMessageGrpcMapper, OutgoingGrpcMessageMapperImpl>();
        services.AddSingleton<IIncomingRequestGrpcMapper, IncomingGrpcMessageMapperImpl>();
        
        services.AddControllers();
        //gRPCBaseCollector.Register(services);
        services.AddGrpc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      //app.UseAuthorization();

      //app.UseEndpoints(endpoints =>
      //{
      //    endpoints.MapControllers();
      //});

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGrpcService<MdsGrpcServiceImpl>();
      });
    }
  }
}
