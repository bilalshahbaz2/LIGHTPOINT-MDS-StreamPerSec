using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessorProtocol
{
    public interface IAsyncProcessor
    {
        void PostAsync(IncomingRequest incomingRequest);
    }

    public interface IDataProcessor
    {
        OutgoingMessage Post(IncomingRequest incomingRequest);
    }


    public interface ReferenceDataProcessor : IAsyncProcessor, IDataProcessor
    {
    }

    public interface SearchDataProcessor : IAsyncProcessor, IDataProcessor
    {

    }

    public interface MarketDataProcessor : IAsyncProcessor, IDataProcessor
    {
        void HandleEvent( RealtimeEventArgs arg );
    }
}
