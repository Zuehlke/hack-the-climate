using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    /// <summary>
    ///     Orchestrates the search
    /// </summary>
    public class GraphService
    {
        private const double SilimarityThreshold = 0.4;

        private readonly LegislationService _legislationService;
        private readonly SearchService _searchService;
        private readonly SimilarityService _similarityService;

        public GraphService(SearchService searchService, SimilarityService similarityService,
            LegislationService legislationService)
        {
            _searchService = searchService;
            _similarityService = similarityService;
            _legislationService = legislationService;
        }

        public async Task<SearchResult> SearchAsync(string searchTerm)
        {
            var graph = new Graph();

            // get search results and add nodes
            var rankedLegislations = new List<RankedLegislation>();
            var fullTextSearchResult = await _searchService.SearchAsync(searchTerm);
            foreach (var (id, rank) in fullTextSearchResult)
            {
                var legislation = _legislationService.GetLegislation(id);
                if (legislation != null)
                {
                    var rankedLegislation = new RankedLegislation(rank, legislation);
                    rankedLegislations.Add(rankedLegislation);
                    graph.Nodes.Add(new Node(legislation.Id, rank));
                }
                else
                {
                    Console.WriteLine("No legislation found for id " + id);
                }
            }

            // calculate similarities and add edges
            var calculatedCombinations = new HashSet<string>();
            foreach (var outer in rankedLegislations)
            foreach (var inner in rankedLegislations)
                if (!calculatedCombinations.Contains(outer.Legislation.Id + inner.Legislation.Id))
                {
                    var similarity = _similarityService.CalculateSimilarity(outer.Legislation, inner.Legislation);
                    calculatedCombinations.Add(outer.Legislation.Id + inner.Legislation.Id);
                    calculatedCombinations.Add(inner.Legislation.Id + outer.Legislation.Id);

                    if (similarity.Similarity > SilimarityThreshold)
                        graph.Links.Add(new Link(outer.Legislation.Id, inner.Legislation.Id, similarity.Similarity));
                }

            return new SearchResult
            {
                Graph = graph,
                RankedLegislations = rankedLegislations
            };
        }
    }

    public class SearchResult
    {
        public IEnumerable<RankedLegislation> RankedLegislations;
        public Graph Graph { set; get; }
    }
}