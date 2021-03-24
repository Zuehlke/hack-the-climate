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

        public async Task<Graph<RankedLegislation>> SearchAsync(string searchTerm)
        {
            var graph = new Graph<RankedLegislation>();

            // get search results and add nodes
            var legislations = new HashSet<GraphNode<RankedLegislation>>();
            var fullTextSearchResult = await _searchService.SearchAsync(searchTerm);
            foreach (var (id, rank) in fullTextSearchResult)
            {
                var legislation = _legislationService.GetLegislation(id);
                if (legislation != null)
                {
                    var rankedLegislation = new RankedLegislation(rank, legislation);
                    graph.AddNode(rankedLegislation);
                    legislations.Add(new GraphNode<RankedLegislation>(rankedLegislation));
                }
                else
                {
                    Console.WriteLine("No legislation found for id " + id);
                }
            }

            // calculate similarities and add edges
            var calculatedCombinations = new HashSet<string>();
            foreach (var outer in legislations)
            foreach (var inner in legislations)
                if (!calculatedCombinations.Contains(outer.Value.Id + inner.Value.Id))
                {
                    var similarity = _similarityService.CalculateSimilarity(outer.Value, inner.Value);
                    graph.AddUndirectedEdge(outer, inner, similarity.Similarity);

                    calculatedCombinations.Add(outer.Value.Id + inner.Value.Id);
                    calculatedCombinations.Add(inner.Value.Id + outer.Value.Id);
                }


            return graph;
        }
    }
}