using CommandProtocol.Dispatchable;
using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServiceProtocol.Dispatchers
{
    public abstract class AbstractDispatcher<T> : IDispatchable<T>
    {
        private readonly Channel<T> channel; 

        public AbstractDispatcher()
        {
            channel = Channel.CreateUnbounded<T>();
        }

        public AckMessage Post(T post)
        {
            //this.bufferBlock.Post(post);
            this.channel.Writer.WriteAsync(post).GetAwaiter();

            return (post as IncomingRequest).ToAckMessage();
        }

        public async Task<T> RecieveAsync()
        {
            return await this.channel.Reader.ReadAsync();
        }
    }

    public interface IBackgroundDispatcher : IDispatchable<IncomingRequest>
    {

    }

    public class BackgroundDispatcher : AbstractDispatcher<IncomingRequest>, IBackgroundDispatcher
    {

        
    }
}
