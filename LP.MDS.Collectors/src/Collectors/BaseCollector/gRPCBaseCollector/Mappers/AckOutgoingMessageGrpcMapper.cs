using AutoMapper;
using CommandProtocol.Transferable;
using GrpcService.MDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gRPCBaseCollector.Mappers
{
    public class AckOutgoingMessageGrpcMapper : IAckOutgoingMessageGrpcMapper
    {

        public AckOutgoingMessageGrpcMapper()
        {

        }
        public AckOutgoingMessage Map(AckMessage source)
        {
            AckOutgoingMessage ackOutgoingMessage = new AckOutgoingMessage();

            ackOutgoingMessage.CorrelationId = source.CorrelationId;
            ackOutgoingMessage.Datasource = source.Datasource;
            ackOutgoingMessage.Message = string.Empty;
            ackOutgoingMessage.Requestor = source.Requestor;
            ackOutgoingMessage.Timestamp = DateTime.Now.ToString();
            ackOutgoingMessage.UserId = source.UserId;
            ackOutgoingMessage.Version = source.Version;

            return ackOutgoingMessage;
        }

        public AckMessage Map(AckOutgoingMessage destination)
        {
            return new AckMessage();
        }
    }
}
