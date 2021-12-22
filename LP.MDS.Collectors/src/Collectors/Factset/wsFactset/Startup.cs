using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebsocketBaseCollector.Hubs;
using Base.Marker;
using NLog;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using System.IO;
using FactsetProcessor.Workers;
using gRPCBaseCollector;
using FactsetProcessor;
using gRPCFactset;
using FactsetProcessor.proxy;
using gRPCBaseCollector.Mappers;
using FluentValidation;
using CommandProtocol.Requestable;
using ServiceProtocol.Validators;
using AutoMapper;
using WebsocketBaseCollector;
using MappingProtocol;

namespace wsFactset
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
            services.AddHostedService<FactsetWorker>();


            BaseMarker.Init<gRPCBaseCollectorMaker>(services);
            BaseMarker.Init<ServiceProtocolMarker>(services);
            BaseMarker.Init<FactsetProcessorMarker>(services);
            BaseMarker.Init<wsBaseCollectorMarker>(services);
            BaseMarker.Init<MappingProtocolMarker>(services);

            services.AddAutoMapper(typeof(Startup));
            var mapperConfig = new MapperConfiguration(mc =>
            {
            });


            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            
            services.AddSingleton<IAckOutgoingMessageGrpcMapper, AckOutgoingMessageGrpcMapper>();
            services.AddSingleton<IOutgoingMessageGrpcMapper, OutgoingGrpcMessageMapperImpl>();
            services.AddSingleton<IIncomingRequestGrpcMapper, IncomingGrpcMessageMapperImpl>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IValidator<IncomingRequest>, IncomingRequestValidator>();

            services.AddSingleton<DataCounter>();
            services.AddSingleton<TimeCounter>();


            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
            }));

            services.AddSignalR().AddMessagePackProtocol();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "wsFactset", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "wsFactset v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CollectorHub>("/hubs/collector");
            });
        }
    }
}
