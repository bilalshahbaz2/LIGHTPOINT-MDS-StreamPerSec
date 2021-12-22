using GrpcService.MDS;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


namespace GrpcClient.Request
{
    public class SubscriptionRequest
    {
        BufferBlock<IncomingRequest> bufferBlock = new BufferBlock<IncomingRequest>(new DataflowBlockOptions());
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private HashSet<string> tickers = new HashSet<string>();

        public SubscriptionRequest()
        {
            var filepath = @"E:\tickers.txt";
            //var append = ":D";

            string[] lines = File.ReadAllLines(filepath);

            foreach (var ticker in lines)
            {
                tickers.Add(ticker);
            }

        }

        public void SubscribeRequest(MdsGrpcService.MdsGrpcServiceClient grpcServiceClient)
        {
            ActionBlock<IncomingRequest> refblock = new ActionBlock<IncomingRequest>((request) =>
            {
                grpcServiceClient.Subscribe(request);

            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1, MaxDegreeOfParallelism = 6 });

            bufferBlock.LinkTo(refblock, new DataflowLinkOptions() { PropagateCompletion = true });


            bufferBlock.Post(subscriptionRequest());
            //bufferBlock.Complete();
            //bufferBlock.Completion.Wait();
        }


        private IncomingRequest subscriptionRequest()
        {
            List<String> menmmomincs = new List<string>()
            {
                "PX_LAST",
                "SEUCRITY_TYPE",
                "BID",
                "ASK"
            };

            IncomingRequest r = new IncomingRequest();
            r.CorrelationId = NewId.NextGuid().ToString();
            r.Datasource = "Factset";
            r.Requestor = "TestApp";
            r.UserId = "hali-lightpointft";
            r.Timestamp = DateTime.Now.ToString();
            r.Version = "1.01";
            r.Requesttype = RequestType.Subscription;

            List<SecurityDefinitionRequest> securities = new List<SecurityDefinitionRequest>();
            foreach (var t in tickers)
            {
                securities.Add(new SecurityDefinitionRequest()
                {
                    IdentifierType = "TICKER",
                    SecurityIdentifier = t
                });
            }

            r.RequestBag = new RequestBag();
            r.RequestBag.Securities.Add(securities);

            r.RequestBag.Fields.Add(menmmomincs.ToList());

            return r;
        }

    }

}
