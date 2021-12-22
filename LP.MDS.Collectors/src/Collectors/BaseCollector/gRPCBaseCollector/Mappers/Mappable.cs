using CommandProtocol.Mappers;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Google.Protobuf;
using GrpcService.MDS;

namespace gRPCBaseCollector.Mappers
{

    public interface IGRPCRequestMappable<R, PF> : IMappable<R, PF>
    where R : IProcessableRequest
    where PF : IMessage
    {
    }

    public interface IGRPCTransferableMapper<T, PF> : IMappable<T, PF>
    where T : IPublishable
    where PF : IMessage
    {
    }

    public interface IIncomingRequestGrpcMapper : IGRPCRequestMappable<CommandProtocol.Requestable.IncomingRequest, GrpcService.MDS.IncomingRequest>
    {
        CommandProtocol.Requestable.IncomingRequest Map(GrpcService.MDS.IncomingRequest incomingRequest, string connectionid);
    }

    public interface IOutgoingMessageGrpcMapper : IGRPCTransferableMapper<CommandProtocol.Transferable.OutgoingMessage, GrpcService.MDS.OutgoingMessage>
    {

    }

    public interface IAckOutgoingMessageGrpcMapper : IGRPCTransferableMapper<CommandProtocol.Transferable.AckMessage, GrpcService.MDS.AckOutgoingMessage>
    {

    }

}