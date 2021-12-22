using Base.Marker;
using FactsetProcessor;
using FactsetProcessor.proxy;
using FluentValidation;
using gRPCBaseCollector;
using gRPCBaseCollector.Mappers;
using GrpcService.MDS;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using ServiceProtocol.Validators;
using System;
using System.Collections.Generic;
using Xunit;

namespace TestCollector.Visualset.Subscription
{
    public class SubscriptionRequestValidationTest : IClassFixture<SubscriptionRequestValidationTest>
    {
        public Mock<MarketDataProcessor> _marketDataProcessorMock = new Mock<MarketDataProcessor>();

        private ServiceCollection _service;
        private IServiceProvider _serviceProvider;
        private SubscriptionManager subscriptionManager;
        public SubscriptionRequestValidationTest()
        {
            _serviceProvider = Utililes.setupDI();
        }

        [Fact]
        public void TestSubscriptionIncomingRequestValidation()
        {
            #region "Constructing a Request : incomingRequest1"
            CommandProtocol.Requestable.IncomingRequest incomingRequest1 = new CommandProtocol.Requestable.IncomingRequest()
            {
                ConnectionId = "ConnectionId1",
                CorrelationId = "1E",
                Version = "2.2",
                Requestor = "user1",
                Timestamp = "time",
                Datasource = "google",
                UserId = "1",
                RequestBag = null
            };
            var SecurityDefinition11 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "AAPL",
                IdentifierType = "Factset"
            };
            var SecurityDefinition12 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "MSFT",
                IdentifierType = "Factset"
            };
            incomingRequest1.RequestBag = new CommandProtocol.Requestable.RequestBag()
            {
                Options = "",
                Rate = 0,
                Fields = new List<string>() { "PX_LAST",
                    "SEUCRITY_TYPE",
                    "BID",
                    "ASK" },
                Securities = new List<CommandProtocol.Requestable.SecurityDefinition>() { SecurityDefinition11, SecurityDefinition12 }
            };
            #endregion

            #region "Constructing a Request : incomingRequest2"
            CommandProtocol.Requestable.IncomingRequest incomingRequest2 = new CommandProtocol.Requestable.IncomingRequest()
            {
                ConnectionId = "ConnectionId2",
                CorrelationId = "1ED",
                Version = "3.3",
                Requestor = "user2",
                Timestamp = "time",
                Datasource = "bing",
                UserId = "2",
                RequestBag = null
            };
            var SecurityDefinition21 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "AAPL",
                IdentifierType = "Factset"
            };
            var SecurityDefinition22 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "MSFT",
                IdentifierType = "Factset"
            };
            incomingRequest2.RequestBag = new CommandProtocol.Requestable.RequestBag()
            {
                Options = "",
                Rate = 0,
                Fields = new List<string>() { "PX_LAST",
                    "SEUCRITY_TYPE",
                    "BID",
                    "ASK" },
                Securities = new List<CommandProtocol.Requestable.SecurityDefinition>() { SecurityDefinition21, SecurityDefinition22 }
            };
            #endregion

            #region "Constructing a Request : incomingRequest3"
            CommandProtocol.Requestable.IncomingRequest incomingRequest3 = new CommandProtocol.Requestable.IncomingRequest()
            {
                ConnectionId = "ConnectionId3",
                CorrelationId = "1E",
                Version = "4.4",
                Requestor = "user3",
                Timestamp = "time",
                Datasource = "DuckDuckGo",
                UserId = "3",
                RequestBag = null
            };
            var SecurityDefinition31 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "AAPL",
                IdentifierType = "Factset"
            };
            var SecurityDefinition32 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "MSFT",
                IdentifierType = "Factset"
            };
            incomingRequest3.RequestBag = new CommandProtocol.Requestable.RequestBag()
            {
                Options = "",
                Rate = 0,
                Fields = new List<string>() { "PX_LAST",
                    "SEUCRITY_TYPE",
                    "BID",
                    "ASK" },
                Securities = new List<CommandProtocol.Requestable.SecurityDefinition>() { SecurityDefinition31, SecurityDefinition32 }
            };
            #endregion

            var requestList = new List<CommandProtocol.Requestable.IncomingRequest>() 
            {
            incomingRequest1,
            incomingRequest2
            };

            //Testing if the new Request is valid or not
            var service = _serviceProvider.GetRequiredService<IMarketDataService>();

            foreach (var request in requestList)
            {
                _marketDataProcessorMock.Setup(x => x.Post(request)).Returns(new CommandProtocol.Transferable.OutgoingMessage() { });
                //Assert.Throws<FluentValidation.ValidationException>(() => service.Post(request));
                service.Post(request);
            }

            Assert.Throws<FluentValidation.ValidationException>(() => service.Post(incomingRequest3));
        }
    }
}
