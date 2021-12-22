using System;
using System.Collections.Concurrent;
using CommandProtocol.Transferable;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
using System.Linq;
using CommandProtocol.Requestable;
using CommandProtocol.Extenstions;
using NLog;

namespace ServiceProtocol.Services
{
    public interface IClientPublishableService
    {
        void Register( String connectionId );
        Task<OutgoingMessage> ReceiveAsync( String connectionId );
        void Publish(IncomingRequest incomingRequest, RealtimeEventArgs realtimeEventArgs);
        void Publish( OutgoingMessage message);
        void Remove(string connectionId);

        List<String> GetConnections();
    }

    public class ClientPublishableService : IClientPublishableService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<String, BufferBlock<OutgoingMessage>> publisherContainer;
        private readonly SubscriptionManager subscriptionManager;
        public ClientPublishableService(SubscriptionManager subscriptionManager)
        {
            this.subscriptionManager = subscriptionManager;
            this.publisherContainer = new ConcurrentDictionary<string, BufferBlock<OutgoingMessage>>();
        }

        public void Publish(OutgoingMessage message)
        {
            logger.Info($"ClientPublishableService - Public (Writing in ) - CorrelationID = {message.CorrelationId}");
            var connectionId = message.ConnectionId;
            if (this.publisherContainer.ContainsKey(connectionId)) 
            {
                this.publisherContainer[connectionId].Post(message);
            }
        }

        public void Register(string connectionId)
        {
            if( !this.publisherContainer.ContainsKey(connectionId))
            {
                this.publisherContainer[connectionId] = new BufferBlock<OutgoingMessage>();
                subscriptionManager.RegisterConnection(connectionId);
            }
        }

        public async Task<OutgoingMessage> ReceiveAsync(string connectionId)
        {
            OutgoingMessage message = await this.publisherContainer[connectionId].ReceiveAsync();
            return message;
        }

        public void Remove(string connectionId)
        {
            if (this.publisherContainer.ContainsKey(connectionId))
            {
                logger.Info("ConnectionID Removed {0}", connectionId);
                var blockBuffer = this.publisherContainer[connectionId];
                blockBuffer.Complete();
                this.publisherContainer.Remove(connectionId, out BufferBlock<OutgoingMessage> buffer);
            }
        }

        public List<string> GetConnections()
        {
            return this.publisherContainer.Keys.ToList();
        }

        public void Publish(IncomingRequest request, RealtimeEventArgs arg)
        {
            Task.Run(() =>
            {
                OutgoingMessage outgoingMessage = request.ToOutgoingMessage(arg);

                this.Publish(outgoingMessage);
            });
        }
    }
}
