using MappingProtocol.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MappingProtocol.Repos.GlobalMappings
{
    public class GlobalMappingRepo : IGlobalMappingRepo
    {
        private Context _context;
        public GlobalMappingRepo(Context context)
        {
            this._context = context;
        }
        public async Task<GlobalMapping> FindByCollectorValue(string collectorValue)
        {
            return await _context.GlobalMappings.FirstOrDefaultAsync(x => x.CollectorValue == collectorValue);
        }
        public async Task<List<GlobalMapping>> GetGlobalMapping()
        {
            return await _context.GlobalMappings.ToListAsync();
        }
    }
}
