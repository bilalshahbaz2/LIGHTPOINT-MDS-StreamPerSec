using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using FactsetProcessor.Mappers;
using FactsetProcessor.V2.FactsetProxy;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FactsetProcessor.V2
{
    public class ReferenceProcessImp : ReferenceDataProcessor
    {
        private readonly BufferBlock<IncomingRequest> incomingbuffer;

        private readonly TransformBlock<IncomingRequest, Tuple<List<Dictionary<string, string>> , IncomingRequest>> transeformIncomingReq;

        private readonly TransformBlock<Tuple<List<Dictionary<string, string>>, IncomingRequest>, OutgoingMessage> transformOutgoingRes;

        private readonly TransformBlock<Tuple<List<Dictionary<string, string>>, IncomingRequest>, OutgoingMessage> nullBlock;

        private readonly ActionBlock<OutgoingMessage> publichmessage;

        private readonly IFactsetProxy factsetProxy;
        private readonly IClientPublishableService clientPublishableService;
        private readonly IFieldMapper fieldMapper;

        public ReferenceProcessImp(IFactsetProxy _factsetProxy, IClientPublishableService _clientPublishableService, IFieldMapper _fieldMapper)
        {
            factsetProxy = _factsetProxy;
            clientPublishableService = _clientPublishableService;
            fieldMapper = _fieldMapper;
            incomingbuffer = new BufferBlock<IncomingRequest>();

            publichmessage = new ActionBlock<OutgoingMessage>((request) =>
            {
                clientPublishableService.Publish(request);
            });

            nullBlock = new TransformBlock<Tuple<List<Dictionary<string, string>>, IncomingRequest>, OutgoingMessage>((response) =>
            {
                return response.Item2.ToOutgoingMessage();
            });

            transformOutgoingRes = new TransformBlock<Tuple<List<Dictionary<string, string>>, IncomingRequest>, OutgoingMessage>((response) =>
            {
                return this.MapToOutgoingRequest(response.Item1, response.Item2);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });

            transeformIncomingReq = new TransformBlock<IncomingRequest, Tuple<List<Dictionary<string, string>>, IncomingRequest>>((incomingRequest) =>
            {
                return Tuple.Create(this.factsetProxy.GetRefDataAsync(incomingRequest), incomingRequest);

            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });

            incomingbuffer.LinkTo(this.transeformIncomingReq, new DataflowLinkOptions() { PropagateCompletion = true });

            transeformIncomingReq.LinkTo(this.transformOutgoingRes, new DataflowLinkOptions() { PropagateCompletion = true }, item => item.Item1 != null);

            transeformIncomingReq.LinkTo(this.nullBlock, new DataflowLinkOptions() { PropagateCompletion = true }, item => item.Item1 == null);

            transformOutgoingRes.LinkTo(this.publichmessage, new DataflowLinkOptions() { PropagateCompletion = true });

            nullBlock.LinkTo(this.publichmessage, new DataflowLinkOptions() { PropagateCompletion = true });

        }

        private Task<OutgoingMessage> MapToOutgoingRequest(List<Dictionary<string, string>> request, IncomingRequest incomingRequest)
        {
            var responseBag = fieldMapper.ToSourceCollector(request, incomingRequest);

            var result = incomingRequest.ToOutgoingMessage();
            result.ResponseBag = responseBag;

            return Task.FromResult(result);
        }

        

        public OutgoingMessage Post(IncomingRequest incomginRequest)
        {
            throw new NotImplementedException();
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {
            this.incomingbuffer.SendAsync(incomingRequest).GetAwaiter();
        }
    }
}
