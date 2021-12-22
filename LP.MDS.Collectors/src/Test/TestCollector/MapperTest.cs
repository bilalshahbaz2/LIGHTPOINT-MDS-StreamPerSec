using gRPCBaseCollector.Mappers;
using GrpcService.MDS;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace TestCollector
{
    public class MapperTest : IClassFixture<MapperTest>
    {
        private ServiceCollection _service;
        private IServiceProvider _serviceProvider;
        private IIncomingRequestGrpcMapper _incomingRequestMapInterface;
        private IOutgoingMessageGrpcMapper _outgoingMessageMapInterface;
        public MapperTest()
        {
            _service = new ServiceCollection();
            _service.AddAutoMapper(typeof(AutoMapperProfiles));
            _service.AddSingleton<IIncomingRequestGrpcMapper, IncomingGrpcMessageMapperImpl>();
            _service.AddSingleton<IOutgoingMessageGrpcMapper, OutgoingGrpcMessageMapperImpl>();
            _serviceProvider = _service.BuildServiceProvider();
            _incomingRequestMapInterface = _serviceProvider.GetRequiredService<IIncomingRequestGrpcMapper>();
            _outgoingMessageMapInterface = _serviceProvider.GetRequiredService<IOutgoingMessageGrpcMapper>();
        }

        [Fact]
        public void IncomingRequestMapTest()
        {
            //Constructing Grpc Model
            var grpcRequest = new IncomingRequest()
            {
                CorrelationId = "1A",
                Version = "2.2",
                Requestor = "Client1",
                Timestamp = "2",
                Datasource = "google",
                UserId = "1",
                RequestBag = null,
                Requesttype = RequestType.Reference
            };

            //Constructing Incoming Model
            var incomingModel = new CommandProtocol.Requestable.IncomingRequest()
            {
                ConnectionId = null,
                CorrelationId = "1A",
                Version = "2.2",
                Requestor = "Client1",
                Timestamp = "2",
                Datasource = "google",
                UserId = "1",
                RequestBag = null,
                requestType = CommandProtocol.Requestable.RequestType.Reference
            };

            var result = _incomingRequestMapInterface.Map(grpcRequest);

            Assert.Equal(incomingModel.ConnectionId, result.ConnectionId);
            Assert.Equal(incomingModel.CorrelationId, result.CorrelationId);
            Assert.Equal(incomingModel.Version, result.Version);
            Assert.Equal(incomingModel.Requestor, result.Requestor);
            Assert.Equal(incomingModel.Timestamp, result.Timestamp);
            Assert.Equal(incomingModel.Datasource, result.Datasource);
            Assert.Equal(incomingModel.UserId, result.UserId);
            Assert.Equal(incomingModel.RequestBag, result.RequestBag);
            Assert.Equal(incomingModel.requestType, result.requestType);
        }

        [Fact]
        public void OutgoingMessageMapTest()
        {
            //Constructing Outgoing Model
            var outgoingMessage = new CommandProtocol.Transferable.OutgoingMessage()
            {
                Version = "2.2",
                Requestor = "user1",
                Timestamp = "No Time",
                Datasource = "Google",
                CorrelationId = "1A",
                UserId = "123",
                ResponseBag = null,
                RequestType = CommandProtocol.Requestable.RequestType.Reference
            };

            //Constructing Grpc Outgoing Message

            var GrpcOutgoingMessage = new OutgoingMessage()
            {
                Version = "2.2",
                Requestor = "user1",
                Timestamp = "No Time",
                Datasource = "Google",
                CorrelationId = "1A",
                UserId = "123",
                ResponseBag = null,
                Requesttype = RequestType.Reference
            };

            var result = _outgoingMessageMapInterface.Map(outgoingMessage);

            Assert.Equal(GrpcOutgoingMessage.Version, result.Version);
            Assert.Equal(GrpcOutgoingMessage.Requestor, result.Requestor);
            Assert.Equal(GrpcOutgoingMessage.Timestamp, result.Timestamp);
            Assert.Equal(GrpcOutgoingMessage.Datasource, result.Datasource);
            Assert.Equal(GrpcOutgoingMessage.CorrelationId, result.CorrelationId);
            Assert.Equal(GrpcOutgoingMessage.UserId, result.UserId);
            Assert.Equal(GrpcOutgoingMessage.ResponseBag, result.ResponseBag);
            Assert.Equal(GrpcOutgoingMessage.Requesttype, result.Requesttype);

        }

    }
}
