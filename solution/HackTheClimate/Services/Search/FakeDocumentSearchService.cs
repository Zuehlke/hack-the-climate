using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackTheClimate.Services.Search
{
    // For faster local development
    public class FakeDocumentSearchService : IDocumentSearchService
    {
        public Task<IEnumerable<(string DocumentId, double ConfidenceScore)>> SearchAsync(string searchTerm)
        {
            var ids = new[]
            {
                "1571",
                "4620",
                "9769",
                "8127",
                "9363",
                "1292",
                "9768",
                "8646",
                "8763",
                "9762",
                "9739",
                "9761",
                "9760"
            };

            var rand = new Random();
            return Task.FromResult(ids.Select(id => (DocumentId: id, ConfidenceScore: rand.NextDouble())));
        }
    }
}