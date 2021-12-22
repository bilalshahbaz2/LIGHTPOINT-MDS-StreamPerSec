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
    public class ReferenceRequest
    {
        BufferBlock<IncomingRequest> bufferBlock = new BufferBlock<IncomingRequest>(new DataflowBlockOptions() { BoundedCapacity = 1 });
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private HashSet<string> tickers = new HashSet<string>();
        public ReferenceRequest()
        {
            int count = 0;
            var filepath = @"E:\tickers.txt";
            string[] lines = File.ReadAllLines(filepath);

            foreach (var ticker in lines)
            {
                tickers.Add(ticker);
                if(count > 10)
                {
                    break;
                }
                count++;
            }
        }

        public void GetRefData(MdsGrpcService.MdsGrpcServiceClient grpcServiceClient)
        {
            ActionBlock<IncomingRequest> refblock = new ActionBlock<IncomingRequest>((request) =>
            {
                grpcServiceClient.GetReferenceDataSubsciption(request);

            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1, MaxDegreeOfParallelism = 6 });

            bufferBlock.LinkTo(refblock, new DataflowLinkOptions() { PropagateCompletion = true });


            bufferBlock.Post(getRequest());
            //bufferBlock.Complete();
            //bufferBlock.Completion.Wait();
        }

        private IncomingRequest getSearchData(string text)
        {
            var incomingrequest = getRequest();
            incomingrequest.RequestBag.Keyword = text;

            return incomingrequest;
        }

        private IncomingRequest getRequest()
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
            r.Requesttype = RequestType.Reference;


            List<SecurityDefinitionRequest> securities = new List<SecurityDefinitionRequest>();
            foreach (var ticker in tickers)
            {
                securities.Add(new SecurityDefinitionRequest()
                {
                    IdentifierType = "TICKER",
                    SecurityIdentifier = ticker
                });
            }

            r.RequestBag = new RequestBag();
            r.RequestBag.Securities.Add(securities);

            r.RequestBag.Fields.Add(menmmomincs.ToList());

            return r;

        }

    }
}
