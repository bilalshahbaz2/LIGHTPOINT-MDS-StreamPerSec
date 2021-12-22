using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServiceProtocol.Services
{
    public interface IFactoryDataService : IDataServier, IAsyncDataService
    {
        
    }

   
    public class FactoryDataService : IFactoryDataService
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();


        private readonly IMarketDataService marketDataService;
        private readonly IReferenceDataService referenceDataService;
        private readonly IUnSubscribeService unSubscribeService;
        private ActionBlock<IncomingRequest> actionBlock;

        private BufferBlock<IncomingRequest> incomingbuffer;

        public FactoryDataService(IMarketDataService marketDataService, IReferenceDataService referenceDataService, IUnSubscribeService _unSubscribeService)
        {
            this.marketDataService = marketDataService;
            this.referenceDataService = referenceDataService;
            unSubscribeService = _unSubscribeService;
            this.actionBlock = new ActionBlock<IncomingRequest>( request => {
                this.Process(request);
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 1
            });

            incomingbuffer = new BufferBlock<IncomingRequest>();

            incomingbuffer.LinkTo(this.actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });
        }


        private void Process(IncomingRequest request)
        {
            logger.Info($"Incoming request in Factory data serivce Action Block : {request.CorrelationId}");

            if (request.requestType == RequestType.Subscription) 
            {
                this.marketDataService.PostAsync(request);
            }

            else if(request.requestType == RequestType.UnSubscribe)
            {
                this.unSubscribeService.PostAsync(request);
            }
            
            else if (request.requestType == RequestType.Search || request.requestType == RequestType.Reference)
            {
                this.referenceDataService.PostAsync(request);
            }
            
        }


        public OutgoingMessage Post(IncomingRequest request)
        {
            OutgoingMessage outgoingMessage = null;
            if (request.requestType == RequestType.Subscription )
            {
                outgoingMessage = this.marketDataService.Post(request);
            }
            else if(request.requestType == RequestType.UnSubscribe)
            {
                this.unSubscribeService.PostAsync(request);
            }
            else if (request.requestType == RequestType.Reference)
            {
                outgoingMessage = this.referenceDataService.Post(request);
            }
            else
            {
                throw new Exception("Invalid Request type");
            }

            return outgoingMessage;
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {
            logger.Info($"Incoming request in Factory : {incomingRequest.CorrelationId}");

            this.incomingbuffer.Post(incomingRequest);
        }
    }
}
