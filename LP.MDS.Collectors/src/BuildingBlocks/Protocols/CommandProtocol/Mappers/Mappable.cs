using System;
namespace CommandProtocol.Mappers
{
    public interface IMappable<Source, Destination>
    {
        Destination Map(Source source);
        Source Map(Destination destination);
    }
}
