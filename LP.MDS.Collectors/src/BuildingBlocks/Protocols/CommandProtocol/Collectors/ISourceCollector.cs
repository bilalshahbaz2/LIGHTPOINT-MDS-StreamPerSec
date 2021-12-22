using System;
using System.Collections.Generic;

namespace CommandProtocol.Collectors
{
    public interface ISourceCollector
    {
        List<String> GetFields();
    }

    public interface ITargetCollector
    {


    }
}
