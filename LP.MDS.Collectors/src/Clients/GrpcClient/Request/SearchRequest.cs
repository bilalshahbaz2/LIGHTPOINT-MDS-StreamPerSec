using GrpcService.MDS;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GrpcClient.Request
{

    public class SearchRequest
    {
        BufferBlock<IncomingRequest> bufferBlock = new BufferBlock<IncomingRequest>(new DataflowBlockOptions());
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();


        public void GetSearchData(MdsGrpcService.MdsGrpcServiceClient grpcServiceClient, string text)
        {
            ActionBlock<IncomingRequest> refblock = new ActionBlock<IncomingRequest>((request) =>
            {
                grpcServiceClient.GetSearchData(request);

            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1, MaxDegreeOfParallelism = 6 });

            bufferBlock.LinkTo(refblock, new DataflowLinkOptions() { PropagateCompletion = true });


            bufferBlock.Post(getSearchData(text));
            //bufferBlock.Complete();
            //bufferBlock.Completion.Wait();
        }

        private IncomingRequest getSearchData(string text)
        {
            var incomingrequest = getRequest();
            incomingrequest.RequestBag.Keyword = text;
            return incomingrequest;
        }

        private static IncomingRequest getRequest()
        {
            List<String> names = new List<string>()
            {
                "msft",
                "aapl",
                "dash"
            };

            List<String> menmmomincs = new List<string>()
            {
                "PX_LAST",
                "SEUCRITY_TYPE",
                "BID",
                "ASK",
                "BID_SIZE",
                "ASK_SIZE"
            };

            IncomingRequest r = new IncomingRequest();
            r.CorrelationId = NewId.NextGuid().ToString();
            r.Datasource = "Factset";
            r.Requestor = "TestApp";
            r.UserId = "hali-lightpointft";
            r.Timestamp = DateTime.Now.ToString();
            r.Version = "1.01";
            r.Requesttype = RequestType.Search;

            List<SecurityDefinitionRequest> securities = new List<SecurityDefinitionRequest>();
            foreach (var sname in names)
            {
                securities.Add(new SecurityDefinitionRequest()
                {
                    IdentifierType = "TICKER",
                    SecurityIdentifier = sname
                });
            }

            r.RequestBag = new RequestBag();
            r.RequestBag.Securities.Add(securities);

            r.RequestBag.Fields.Add(menmmomincs.ToList());

            return r;

        }

    }

}
