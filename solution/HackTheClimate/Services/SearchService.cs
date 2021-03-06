using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackTheClimate.Services.Search;

namespace HackTheClimate.Services
{
    /// <summary>
    ///     Calls the full text search.
    /// </summary>
    public class SearchService
    {
        private const int NumberOfResults = 50;
        private readonly AzureSearchFacade _azureSearchFacade;

        public SearchService(AzureSearchFacade azureSearchFacade)
        {
            _azureSearchFacade = azureSearchFacade;
        }


        /// <summary>
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns>array of id/rank mapping</returns>
        public async Task<IEnumerable<(string DocumentId, double ConfidenceScore)>> SearchAsync(string searchTerm)
        {
            var searchResult = await _azureSearchFacade.QueryAsync(searchTerm, NumberOfResults);
            return searchResult.Select(result =>
                (result.DocumentId, result.ConfidenceScore));
        }
    }
}