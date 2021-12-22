using AutoMapper;
using Base.Marker;
using CommandProtocol.Requestable;
using FactsetProcessor;
using FactsetProcessor.proxy;
using FactsetProcessor.Workers;
using FluentValidation;
using gRPCBaseCollector;
using gRPCBaseCollector.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using ServiceProtocol.Validators;
using System.IO;

namespace LP.Collectors.Factset.gRPC
{
    public class Startup
    {
        private string contentRoot = string.Empty;
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration, IWebHostEnvironment env) 
        {
            Configuration = configuration;
			contentRoot = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            BaseMarker.Init<gRPCBaseCollectorMaker>(services);
            BaseMarker.Init<ServiceProtocolMarker>(services);
            BaseMarker.Init<FactsetProcessorMarker>(services);

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IAckOutgoingMessageGrpcMapper, AckOutgoingMessageGrpcMapper>();
            services.AddSingleton<IOutgoingMessageGrpcMapper, OutgoingGrpcMessageMapperImpl>();
            services.AddSingleton<IIncomingRequestGrpcMapper, IncomingGrpcMessageMapperImpl>();


            services.AddGrpc();

            services.AddControllers();

            //Auto Mapper
            services.AddAutoMapper(typeof(Startup));

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfiles());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            //FluentValidation
            services.AddTransient<IValidator<IncomingRequest>, IncomingRequestValidator>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<MdsGrpcServiceImpl>();
            });
        }
    }
}
