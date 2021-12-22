using CommandProtocol.Requestable;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FactsetProcessor
{
    public class FactsetSubscriptionDispatcher
    {
        private Channel<IncomingRequest> incomcingChannelRrequest;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public FactsetSubscriptionDispatcher()
        {
            this.incomcingChannelRrequest = Channel.CreateUnbounded<IncomingRequest>();
        }

        public void Post(IncomingRequest request)
        {
            logger.Info($"Adding Request into FactsetSubscription Dispatcher - CorrelationID = {request.CorrelationId}");
            this.incomcingChannelRrequest.Writer.WriteAsync(request).GetAwaiter();
        }

        public async Task<IncomingRequest> recieve()
        {
            return await this.incomcingChannelRrequest.Reader.ReadAsync();
        }
    }
}
