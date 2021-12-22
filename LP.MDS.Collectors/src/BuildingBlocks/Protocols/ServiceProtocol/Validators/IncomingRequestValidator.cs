using CommandProtocol.Requestable;
using FluentValidation;
using ServiceProtocol.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ServiceProtocol.Validators
{
    public class IncomingRequestValidator : AbstractValidator<IncomingRequest>
    {

        public IncomingRequestValidator(SubscriptionManager subscriptionManager)
        {
            RuleFor(x => x.CorrelationId)
                .NotNull()
                .NotEmpty()
                .Matches("^[a-zA-Z0-9- ]*$");

            RuleFor(x => x.ConnectionId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.RequestBag)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.RequestBag.Securities)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.RequestBag.Fields)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.requestType).NotNull().WithMessage("Request Type cannot be Empty");

            //For Search type request
            RuleFor(x => x.RequestBag.Keyword)
                .NotNull()
                .NotEmpty()
                .When(x => x.requestType == RequestType.Search);

            RuleFor(x => x)
                .Must( (request) => {
                    return IsCorrelationUnique(request, subscriptionManager);
                }).WithMessage("Dublicate CorrelationId found");


        }
       

        private bool IsCorrelationUnique(IncomingRequest incomingRequest, SubscriptionManager subscriptionManager)
        {
            var correlationId = incomingRequest.CorrelationId;
            var request = subscriptionManager.FindByCorrelationId(correlationId);
            if (request != null && incomingRequest.requestType == RequestType.Subscription)
                return false;
            else
                return true;
        }
    }
}
