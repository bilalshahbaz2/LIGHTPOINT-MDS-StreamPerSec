using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol
{
    public interface IServiceProtocol
    {
    }

    public interface IPublishableService<T>
        where T : IPublishable
    {
        void Publish( T response );
    }

    public interface IAsyncDataService
    {
        void PostAsync(IncomingRequest incomingRequest);
    }

    public interface IDataServier
    {
        OutgoingMessage Post(IncomingRequest incomingRequest);
    }



    public interface IMarketDataService : IAsyncDataService, IDataServier
    {
        bool Unsubscribe(IncomingRequest request);
        bool hasSubscription(IncomingRequest request);

    }

    public interface IReferenceDataService : IAsyncDataService, IDataServier
    {
        
    }

    public interface IUnSubscribeService : IAsyncDataService
    {

    }

}
