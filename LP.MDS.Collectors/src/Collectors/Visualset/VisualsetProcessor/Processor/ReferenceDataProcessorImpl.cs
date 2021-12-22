using System;
using System.Collections.Generic;
using Bogus;
using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using NLog;
using ProcessorProtocol;
using ServiceProtocol.Dispatchers;

namespace VisualsetProcessor.Processor
{
    public class ReferenceDataProcessorImpl : ReferenceDataProcessor
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly Faker faker;
        private readonly IBackgroundDispatcher backgroundDispatcher;

        public ReferenceDataProcessorImpl(IBackgroundDispatcher backgroundDispatcher)
        {
            this.faker = new Faker();
            this.backgroundDispatcher = backgroundDispatcher;
        }

        public OutgoingMessage Post(IncomingRequest request)
        {
            logger.Info("Processing @{r}", request.ToLogger());
            OutgoingMessage outgoingMessage = request.ToOutgoingMessage();
            outgoingMessage.ResponseBag = new ResponseBag();
            List<CommandProtocol.Requestable.SecurityDefinition> securities = request.RequestBag.Securities;
            foreach (var security in securities)
            {
                outgoingMessage.ResponseBag.Items.Add(this.processRequestItem(security.SecurityIdentifier, request.RequestBag.Fields));
            }
            return outgoingMessage;
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {
            this.backgroundDispatcher.Post(incomingRequest);
        }

        public void ProcessHandler()
        {
            throw new NotImplementedException();
        }

       

        private ResponseBagItem processRequestItem( String ticker, List<String> fields )
        {
            ResponseBagItem bagItem = new ResponseBagItem();
            bagItem.Security = new CommandProtocol.Transferable.SecurityDefinition();
            bagItem.Security.SecurityIdentifier = ticker;
            bagItem.Security.IdentifierType = "TICKER";
            bagItem.Security.Message = "";
            bagItem.Security.LastUpdate = DateTime.UtcNow.ToString();

            foreach( string field in fields)
            {
                FieldDescriptor fieldDescriptor = new FieldDescriptor();
                fieldDescriptor.Key = ticker;
                fieldDescriptor.Value = this.faker.Random.Decimal((decimal)1.25, (decimal)10.99).ToString();
                fieldDescriptor.HasError = false;
                fieldDescriptor.OriginatingSource = "VisualSet";
                fieldDescriptor.Timestamp = DateTime.UtcNow.ToString();
                fieldDescriptor.CollectorCode = $"{field}-VisualSet";
                bagItem.FieldValues.Add(field, fieldDescriptor);
            }
            return bagItem;
        }
    }
}
