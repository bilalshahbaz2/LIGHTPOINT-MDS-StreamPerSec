using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NLog;
using ProcessorProtocol;
using ServiceProtocol.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly ILogger<ReferenceDataService> _logger;

        private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IncomingRequestValidator validator;
        private readonly ReferenceDataProcessor _processor;
        private readonly SearchDataProcessor searchDataProcessor;

        public ReferenceDataService(ILogger<ReferenceDataService> logger,
            IncomingRequestValidator validator,
            ReferenceDataProcessor processor,
            SearchDataProcessor _searchDataProcessor
            )
        {
            this._logger = logger;
            this.validator = validator;
            this._processor = processor;
            searchDataProcessor = _searchDataProcessor;
        }

        
        public void PostAsync(IncomingRequest incomingRequest)
        {
            logger.Info($"Incoming request in ReferenceDataService {incomingRequest.CorrelationId}");
            this.validator.ValidateAndThrow(incomingRequest);

            if (incomingRequest.requestType == RequestType.Reference)
            {
                _processor.PostAsync(incomingRequest);
            }
            else searchDataProcessor.PostAsync(incomingRequest);
        }

        public OutgoingMessage Post(IncomingRequest incomingRequest)
        {
            this.validator.ValidateAndThrow(incomingRequest);
            return _processor.Post(incomingRequest);            
        }
        
    }
}
