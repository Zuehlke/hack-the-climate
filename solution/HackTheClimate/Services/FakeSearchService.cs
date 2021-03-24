using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackTheClimate.Services.Search
{
    // For faster local development
    public class FakeSearchService
    {
        public async Task<IEnumerable<(string DocumentId, double ConfidenceScore)>> SearchAsync(string searchTerm)
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
                "8646"
            };

            var rand = new Random();
            return ids.Select(id => (DocumentId: id, ConfidenceScore: rand.NextDouble()));
        }
    }
}