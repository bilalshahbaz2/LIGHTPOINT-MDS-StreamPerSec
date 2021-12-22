using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Microsoft.AspNetCore.SignalR;
using NLog;
using ServiceProtocol.Dispatchers;
using ServiceProtocol.Services;

namespace WebsocketBaseCollector.Hubs
{

    public interface ICollectorHub
    {
        Task Broadcast(KeepAliveMessage message);

        Task Send( IncomingRequest request );

        Task<ChannelReader<OutgoingMessage>> FeedHandler(CancellationToken cancellationToken);
    }

    public class CollectorHub : Hub
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IFactoryDataService factoryDataService;
        private readonly IClientPublishableService publishableService;
        private readonly IBackgroundDispatcher backgroundDispatcher;

        private bool flag;
        private Timer time;
        //private readonly List<OutgoingMessage> minData;
        //private int currentSec;

        private DataCounter _secData;
        private TimeCounter _currentSec;

        public CollectorHub(IFactoryDataService factoryDataService, IClientPublishableService publishableService, IBackgroundDispatcher _backgroundDispatcher, DataCounter secData, TimeCounter currentSec) : base()
        {
            this.factoryDataService = factoryDataService;
            this.publishableService = publishableService;
            backgroundDispatcher = _backgroundDispatcher;

            this.flag = true;
            //this.minData = new List<OutgoingMessage>();
            //this.currentSec = 1;

            this._secData = secData;
            this._currentSec = currentSec;
        }

        public override Task OnConnectedAsync()
        {
            publishableService.Register( Context.ConnectionId );
            logger.Warn("New Client Connected {0}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            publishableService.Remove(Context.ConnectionId);
            IncomingRequest incomingRequest = new IncomingRequest();
            incomingRequest.requestType = RequestType.UnSubscribe;
            incomingRequest.ConnectionId = Context.ConnectionId;
            backgroundDispatcher.Post(incomingRequest);

            return base.OnDisconnectedAsync(exception);
        }

        public Task Send(IncomingRequest request)
        {
            logger.Warn("FROM CLIENT  @{request}", request.ToLogger());
            logger.Info($"COLLECTOR HUB - SEND METHOD - CorrelationID = {request.CorrelationId}");
            request.ConnectionId = Context.ConnectionId;
            //AckMessage ackMessage = factoryDataService.PostAsync(request);
            //await Clients.All.SendAsync("onMessage", serverMessage);

            return Task.FromResult(backgroundDispatcher.Post(request));
        }

        public async Task Broadcast(KeepAliveMessage message)
        {
            await Clients.All.SendAsync("OnMessage", message);
        }

        public Task<ChannelReader<OutgoingMessage>> FeedHandler(CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<OutgoingMessage>(new UnboundedChannelOptions { SingleWriter = true });
            _ = WriteItemsAsync(channel.Writer, cancellationToken);
            return Task.FromResult( channel.Reader );
        }

        private async Task WriteItemsAsync(ChannelWriter<OutgoingMessage> writer, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    CommandProtocol.Transferable.OutgoingMessage message = await this.publishableService.ReceiveAsync(Context.ConnectionId);
                    await writer.WriteAsync(message, cancellationToken);
                    //if (flag)
                    //{
                    //    this.flag = false;
                    //    this.time = new Timer(minCount, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
                    //}
                    //Console.WriteLine(".");
                    //this.minData.Add(message);



                    await this._currentSec.startTimer();
                    await this._secData.AddMessage(message);

                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }
            finally
            {
                writer.Complete(localException);
            }
        }

        //private void minCount(object state)
        //{
        //    //Console.WriteLine($"{this.currentSec}--{this.minData.Count}");
        //    //this.minData.Clear();
        //    //currentSec++;

        //    Console.WriteLine($"{this.currentSec}--{this._secData.GetCount()}");
        //    this._secData.EmptyData();
        //    currentSec++;
        //}

    }
}
