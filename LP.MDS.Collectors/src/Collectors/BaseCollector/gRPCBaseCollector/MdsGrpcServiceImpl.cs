using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCBaseCollector.Mappers;
using GrpcService.MDS;
using NLog;
using ServiceProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace gRPCBaseCollector
{
    public class MdsGrpcServiceImpl : MdsGrpcService.MdsGrpcServiceBase
    {
        
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IIncomingRequestGrpcMapper incomingMessageMapper;
        private readonly IOutgoingMessageGrpcMapper outgoingMessageMapper;
        private readonly IClientPublishableService publishableService;

        private readonly IFactoryDataService factoryDataService;
        private readonly IAckOutgoingMessageGrpcMapper ackOutgoingMessage;

        public MdsGrpcServiceImpl(
            IIncomingRequestGrpcMapper incomingMessageMapper,
            IOutgoingMessageGrpcMapper outgoingMessageMapper,
            IClientPublishableService publishableService,
            IFactoryDataService _factoryDataService,
            IAckOutgoingMessageGrpcMapper _ackOutgoingMessage)
        {
            this.incomingMessageMapper = incomingMessageMapper;
            this.outgoingMessageMapper = outgoingMessageMapper;
            this.publishableService = publishableService;
            this.factoryDataService = _factoryDataService;
            ackOutgoingMessage = _ackOutgoingMessage;
        }

        public async override Task<GrpcService.MDS.OutgoingMessage> GetReferenceData(GrpcService.MDS.IncomingRequest request, ServerCallContext context)
        {
            logger.Info($"Incoming reference data request in Grpc layer. Incoming request {request} ");

            var incomingRequest = this.incomingMessageMapper.Map(request, context.GetHttpContext().Connection.Id);

            var outgoingMessage = factoryDataService.Post(incomingRequest);

            var grpcOutgoingMessage = this.outgoingMessageMapper.Map(outgoingMessage);
            return grpcOutgoingMessage;
        }

        public override Task<AckOutgoingMessage> GetSearchData(GrpcService.MDS.IncomingRequest request, ServerCallContext context)
        {
            logger.Info($"Incoming search data request in Grpc layer. Incoming request {request}");
            try
            {
                var incomingRequest = this.incomingMessageMapper.Map(request, context.GetHttpContext().Connection.Id);
                factoryDataService.PostAsync(incomingRequest);

                return Task.FromResult(this.ackOutgoingMessage.Map(incomingRequest.ToAckMessage()));

            }
            catch (Exception ex)
            {
                logger.Error($"Exception message : {ex.Message}");
            }


            return null;
        }


        public override Task<AckOutgoingMessage> GetReferenceDataSubsciption(GrpcService.MDS.IncomingRequest request, ServerCallContext context)
        {
            logger.Info($"Incoming reference data request in Grpc layer. Incoming request {request.CorrelationId} ");

            var incomingRequest = this.incomingMessageMapper.Map(request, context.GetHttpContext().Connection.Id);

            factoryDataService.PostAsync(incomingRequest);

            return Task.FromResult(this.ackOutgoingMessage.Map(incomingRequest.ToAckMessage()));
        }

        /**
         * 
         * 
         */
        public override async Task SubscriptionFeedHandler(Empty request, IServerStreamWriter<GrpcService.MDS.OutgoingMessage> responseStream, ServerCallContext context)
        {
            var connectionId = context.GetHttpContext().Connection.Id;
            this.publishableService.Register(connectionId);

            logger.Info($"ConnectionID:: {context.GetHttpContext().Connection.Id}, Outgoing response {responseStream}");
            while ( !context.CancellationToken.IsCancellationRequested )
            {
                CommandProtocol.Transferable.OutgoingMessage message = await this.publishableService.ReceiveAsync(connectionId) ;
                GrpcService.MDS.OutgoingMessage grpcMessage = this.outgoingMessageMapper.Map(message);
                await responseStream.WriteAsync(grpcMessage);
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                this.publishableService.Remove( connectionId );
            }
        }

        public override Task<AckOutgoingMessage> Subscribe(GrpcService.MDS.IncomingRequest request, ServerCallContext context)
        {
            logger.Info("Request in subscription method. | Grpc level");

            var incomingRequest = this.incomingMessageMapper.Map(request, context.GetHttpContext().Connection.Id);
            factoryDataService.PostAsync(incomingRequest);

            var outgoingMessage = new AckOutgoingMessage();
            return Task.FromResult(outgoingMessage);
        }

        public override Task<AckOutgoingMessage> UnSubscribe(UnSubscriptionRequest request, ServerCallContext context)
        {
            return Task.FromResult(new AckOutgoingMessage());
        }
    }
}
