using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Microsoft.Extensions.Options;

namespace HackTheClimate.Services.Search
{
    public class AzureSearchFacade
    {
        private readonly IOptions<AzureSearchConfiguration> _searchOptions;

        public AzureSearchFacade(IOptions<AzureSearchConfiguration> searchOptions)
        {
            _searchOptions = searchOptions;
        }

        public async Task<IEnumerable<DocumentQueryResult>> QueryAsync(string queryTerm, int numberOfResults = 10)
        {
            // Create a client
            var credential = new AzureKeyCredential(_searchOptions.Value.Key);
            var client = new SearchClient(new Uri(_searchOptions.Value.Endpoint), _searchOptions.Value.IndexName,
                credential);

            var response = await client.SearchAsync<SearchModel>(queryTerm, new SearchOptions
            {
                Size = numberOfResults
            });

            return (await response.Value
                    .GetResultsAsync().ToListAsync())
                .Select(result => new DocumentQueryResult
                {
                    DocumentId = result.Document.Id,
                    ConfidenceScore = result.Score ?? 0
                });
        }
    }
}
