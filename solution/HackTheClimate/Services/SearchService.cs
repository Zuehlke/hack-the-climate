using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackTheClimate.Services
{
    /// <summary>
    ///     Calls the full text search.
    /// </summary>
    public class SearchService
    {
        /// <summary>
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns>array of id/rank mapping</returns>
        public async Task<IEnumerable<Tuple<string, double>>> SearchAsync(string searchTerm)
        {
            var ids = new[]
            {
                "1571",
                "9771",
                "9769",
                "8127",
                "9363",
                "1292",
                "9369",
                "9768",
                "8646"
            };

            var rand = new Random();
            return ids.Select(id => new Tuple<string, double>(id, rand.NextDouble()));
        }
    }
}