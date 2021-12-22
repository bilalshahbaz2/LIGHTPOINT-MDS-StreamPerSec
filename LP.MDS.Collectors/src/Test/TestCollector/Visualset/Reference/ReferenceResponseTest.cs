using gRPCBaseCollector.Mappers;
using GrpcService.MDS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessorProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualsetProcessor.Processor;
using Xunit;

namespace TestCollector.Visualset.Reference
{
    public class ReferenceResponseTest : IClassFixture<ReferenceResponseTest>
    {

        private ServiceCollection _service;
        private IServiceProvider _serviceProvider;
        private IIncomingRequestGrpcMapper _incomingRequestMapInterface;
        public ReferenceResponseTest()
        {
            _service = new ServiceCollection();
            _service.AddAutoMapper(typeof(AutoMapperProfiles));
            _service.AddSingleton<IIncomingRequestGrpcMapper, IncomingGrpcMessageMapperImpl>();
            _service.AddSingleton<IOutgoingMessageGrpcMapper, OutgoingGrpcMessageMapperImpl>();
            _service.AddSingleton<ReferenceDataProcessor, ReferenceDataProcessorImpl>();
            _serviceProvider = _service.BuildServiceProvider();
            _incomingRequestMapInterface = _serviceProvider.GetRequiredService<IIncomingRequestGrpcMapper>();
        }

        [Fact]
        public void TestOutputResponseFromProcess()
        {
            //Incoming Map Test

            #region "Constructing Grpc Model : grpcRequest"
            var grpcRequest = new IncomingRequest()
            {
                CorrelationId = "1A",
                Version = "2.2",
                Requestor = "Client1",
                Timestamp = "11/4/2021 11:16:21 AM",
                Datasource = "google",
                UserId = "1",
                RequestBag = null
            };
            #endregion

            var mappedIncoming = _incomingRequestMapInterface.Map(grpcRequest);

            Assert.Equal("1A", mappedIncoming.CorrelationId);
            Assert.Equal("2.2", mappedIncoming.Version);
            Assert.Equal("Client1", mappedIncoming.Requestor);
            Assert.Equal("11/4/2021 11:16:21 AM", mappedIncoming.Timestamp);
            Assert.Equal("google", mappedIncoming.Datasource);
            Assert.Equal("1", mappedIncoming.UserId);

            #region "Constructing the RequestBag in "mappedIncoming" model" 
            mappedIncoming.ConnectionId = "ConnectionId1";

            var SecurityDefinition1 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "AAPL",
                IdentifierType = "Factset"
            };
            var SecurityDefinition2 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "MSFT",
                IdentifierType = "Factset"
            };

            mappedIncoming.RequestBag = new CommandProtocol.Requestable.RequestBag()
            {
                Options = "",
                Rate = 0,
                Fields = new List<string>() { "PX_LAST",
                    "SEUCRITY_TYPE",
                    "BID",
                    "ASK" },
                Securities = new List<CommandProtocol.Requestable.SecurityDefinition>() { SecurityDefinition1, SecurityDefinition2 }
            };
            #endregion


            //Processor Testing

            var service = _serviceProvider.GetRequiredService<ReferenceDataProcessor>();

            var result = service.Post(mappedIncoming);

            Assert.Equal("1A", result.CorrelationId);
            Assert.Equal("ConnectionId1", result.ConnectionId);
            Assert.Equal("2.2", result.Version);
            Assert.Equal("Client1", result.Requestor);
            Assert.Equal("google", result.Datasource);
            Assert.Equal("1", result.UserId);

            foreach (var item in result.ResponseBag.Items)
            {
                Assert.Equal(0, item.FieldErrors.Count);
            }
        }
    }
}
