using System.Collections.Generic;
using System.Threading.Tasks;

namespace HackTheClimate.Services.Search
{
    public interface IDocumentSearchService
    {
        Task<IEnumerable<(string DocumentId, double ConfidenceScore)>> SearchAsync(string searchTerm);
    }
}
