using CommandProtocol.Transferable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommandProtocol.Dispatchable
{
    public interface IDispatchable<T>
    {
        AckMessage Post(T post);

        Task<T> RecieveAsync();
    }
}
