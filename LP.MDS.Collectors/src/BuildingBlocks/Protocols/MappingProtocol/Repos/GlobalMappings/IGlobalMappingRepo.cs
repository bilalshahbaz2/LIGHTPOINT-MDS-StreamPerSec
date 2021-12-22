using MappingProtocol.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MappingProtocol.Repos.GlobalMappings
{
    public interface IGlobalMappingRepo
    {
        Task<GlobalMapping> FindByCollectorValue(string collectorValue);
        Task<List<GlobalMapping>> GetGlobalMapping();
    }
}
