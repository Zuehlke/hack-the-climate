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
                "8646",
                "8763",
                "9762",
                "9739",
                "9761",
                "9760",
                "9759",
                "9758",
                "9757",
                "9756",
                "9714",
                "9755",
                "9753",
                "9749",
                "8934",
                "8713",
                "8669",
                "9750",
                "9751",
                "9748",
                "9747",
                "9746",
                "9745",
                "9744",
                "8475",
                "9743",
                "9742",
                "9741"
            };

            var rand = new Random();
            return ids.Select(id => (DocumentId: id, ConfidenceScore: rand.NextDouble()));
        }
    }
}