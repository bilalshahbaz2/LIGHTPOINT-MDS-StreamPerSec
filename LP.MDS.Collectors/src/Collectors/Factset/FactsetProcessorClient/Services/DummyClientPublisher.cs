using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using NLog;
using RequestCreator;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FactsetProcessorClient.Services
{
    public class DummyClientPublisher : IClientPublishableService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly BufferBlock<OutgoingMessage> publisherBuffer;
       

        public DummyClientPublisher()
        {
            this.publisherBuffer = new BufferBlock<OutgoingMessage>();
        }

        public List<string> GetConnections()
        {
            throw new NotImplementedException();
        }

        public void Publish(IncomingRequest incomingRequest, RealtimeEventArgs realtimeEventArgs)
        {
            logger.Error("Callback.");
        }

        public void Publish(OutgoingMessage message)
        {
            this.publisherBuffer.Post(message);
        }

        public Task<OutgoingMessage> ReceiveAsync(string connectionId)
        {
            return this.publisherBuffer.ReceiveAsync();
        }

        public void Register(string connectionId)
        {
            
        }

        public void Remove(string connectionId)
        {
            throw new NotImplementedException();
        }
    }
}
