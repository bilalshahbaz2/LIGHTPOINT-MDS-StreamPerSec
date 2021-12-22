using AutoMapper;
using CommandProtocol.Requestable;
using NLog;
using System;
namespace gRPCBaseCollector.Mappers
{
    /**
     * 
     * 
     */
    public class IncomingGrpcMessageMapperImpl : IIncomingRequestGrpcMapper
    {
        private readonly IMapper mapper;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public IncomingGrpcMessageMapperImpl(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CommandProtocol.Requestable.IncomingRequest Map(GrpcService.MDS.IncomingRequest protobuff)
        {
            return mapper.Map<CommandProtocol.Requestable.IncomingRequest>(protobuff);
        }

        public GrpcService.MDS.IncomingRequest Map(CommandProtocol.Requestable.IncomingRequest requestable)
        {
            return mapper.Map<GrpcService.MDS.IncomingRequest>(requestable);
        }

        public CommandProtocol.Requestable.IncomingRequest Map(GrpcService.MDS.IncomingRequest incomingRequest, string connectionid)
        {
            var request = this.Map(incomingRequest);
            request.ConnectionId = connectionid;
            request.requestType = (RequestType) incomingRequest.Requesttype;

            return request;
        }
    }
}
