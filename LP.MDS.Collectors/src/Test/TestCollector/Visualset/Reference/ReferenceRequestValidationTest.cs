using gRPCBaseCollector.Mappers;
using GrpcService.MDS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessorProtocol;
using ServiceProtocol;
using ServiceProtocol.Services;
using ServiceProtocol.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestCollector.Visualset.Reference
{
    public class ReferenceRequestValidationTest
    {
        public Mock<ILogger<ReferenceDataService>> _loggerMock = new Mock<ILogger<ReferenceDataService>>();
        public Mock<ReferenceDataProcessor> _referenceDataProcessorMock = new Mock<ReferenceDataProcessor>();

        private ServiceCollection _service;
        private IServiceProvider _serviceProvider;
        private SubscriptionManager subscriptionManager;

        public ReferenceRequestValidationTest()
        {
            _serviceProvider = Utililes.setupDI();
        }

        [Fact]
        public void TestReferenceIncomingRequestValidation()
        {
            #region "Constructing a New Incoming Request : incomingRequest"
            CommandProtocol.Requestable.IncomingRequest incomingRequest = new CommandProtocol.Requestable.IncomingRequest()
            {
                ConnectionId = "ConnectionId3",
                CorrelationId = "1E@",
                Version = "4.4",
                Requestor = "user3",
                Timestamp = "time",
                Datasource = "DuckDuckGo",
                UserId = "3",
                RequestBag = null,
                requestType = CommandProtocol.Requestable.RequestType.Reference
            };
            var SecurityDefinition1 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "AAPL",
                IdentifierType = "Factset"
            };
            var SecurityDefinition2 = new CommandProtocol.Requestable.SecurityDefinition()
            {
                SecurityIdentifier = "MSFT",
                IdentifierType = "Factset"
            };
            incomingRequest.RequestBag = new CommandProtocol.Requestable.RequestBag()
            {
                Options = "",
                Rate = 0,
                Fields = new List<string>() { "PX_LAST",
                    "SEUCRITY_TYPE",
                    "BID",
                    "ASK" },
                Securities = new List<CommandProtocol.Requestable.SecurityDefinition>() { SecurityDefinition1, SecurityDefinition2 }
            };
            #endregion

            //Testing if the Request is valid or not
            _referenceDataProcessorMock.Setup(x => x.Post(incomingRequest)).Returns(new CommandProtocol.Transferable.OutgoingMessage() { });

             var service = _serviceProvider.GetRequiredService<IReferenceDataService>();

            Assert.Throws<FluentValidation.ValidationException>(() => service.Post(incomingRequest));

        }
    }
}
