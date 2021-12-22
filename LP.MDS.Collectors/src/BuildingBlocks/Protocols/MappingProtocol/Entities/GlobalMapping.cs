using System.ComponentModel.DataAnnotations;

namespace MappingProtocol.Entities
{
    public class GlobalMapping
    {
        public long Id { get; set; }
        public string? SourceField { get; set; }
        [Required]
        public string BBValue { get; set; }
        [Required]
        public string CollectorValue { get; set; }

        public GlobalMapping FindByCollectorValue(string collectorValue)
        {
            return new GlobalMapping();
        }
    }
}

