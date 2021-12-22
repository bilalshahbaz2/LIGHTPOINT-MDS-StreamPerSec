using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using ProcessorProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FactsetProcessor.V2.FactsetProxy
{
    public class SearchProcessImp : SearchDataProcessor
    {
        private readonly BufferBlock<IncomingRequest> incomingBuffer;

        private readonly TransformBlock<IncomingRequest, Tuple<IncomingRequest, SearchEventArgs>> transformIncomingReq;

        private readonly TransformBlock<Tuple<IncomingRequest, SearchEventArgs>, OutgoingMessage> transformOutgoingRes;

        private readonly TransformBlock<Tuple<IncomingRequest, SearchEventArgs>, OutgoingMessage> nullBlock;

        private readonly ActionBlock<OutgoingMessage> publichmessage;
        private readonly IFactsetProxy factsetProxy;
        private readonly IClientPublishableService clientPublishableService;


        public SearchProcessImp(IFactsetProxy _factsetProxy, IClientPublishableService _clientPublishableService)
        {
            factsetProxy = _factsetProxy;
            clientPublishableService = _clientPublishableService;
            incomingBuffer = new BufferBlock<IncomingRequest>();

            publichmessage = new ActionBlock<OutgoingMessage>((request) =>
            {
                clientPublishableService.Publish(request);
            });

            nullBlock = new TransformBlock<Tuple<IncomingRequest, SearchEventArgs>, OutgoingMessage>((request) =>
            {
                return request.Item1.ToOutgoingMessage();
            });

            transformOutgoingRes = new TransformBlock<Tuple<IncomingRequest, SearchEventArgs>, OutgoingMessage>(request =>
            {
                return MapToOutgoingRequest(request.Item1, request.Item2);

            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });

            transformIncomingReq = new TransformBlock<IncomingRequest, Tuple<IncomingRequest, SearchEventArgs>>((incomingRequest) =>
            {
                var result = this.factsetProxy.GetSearchDataAsync(incomingRequest).Result;

                return Tuple.Create(incomingRequest, result);

            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });

            incomingBuffer.LinkTo(this.transformIncomingReq, new DataflowLinkOptions() { PropagateCompletion = true });

            transformIncomingReq.LinkTo(this.transformOutgoingRes, new DataflowLinkOptions() { PropagateCompletion = true }, item => item.Item2 != null);

            transformIncomingReq.LinkTo(this.nullBlock, new DataflowLinkOptions() { PropagateCompletion = true }, item => item.Item2 == null);

            transformOutgoingRes.LinkTo(this.publichmessage, new DataflowLinkOptions() { PropagateCompletion = true });

            nullBlock.LinkTo(this.publichmessage, new DataflowLinkOptions() { PropagateCompletion = true });
        }

        private Task<OutgoingMessage> MapToOutgoingRequest(IncomingRequest incomingRequest, SearchEventArgs searchResult)
        {
            var outgoingMessage = incomingRequest.ToOutgoingMessage(searchResult);

            return Task.FromResult(outgoingMessage);
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {
            this.incomingBuffer.SendAsync(incomingRequest).GetAwaiter();
        }

        public OutgoingMessage Post(IncomingRequest incomingRequest)
        {
            throw new NotImplementedException();
        }
    }
}
