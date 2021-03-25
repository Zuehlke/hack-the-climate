using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackTheClimate.Data;
using HackTheClimate.Services.Search;
using HackTheClimate.Services.Similarity;

namespace HackTheClimate.Services
{
    /// <summary>
    ///     Orchestrates the search
    /// </summary>
    public class GraphService
    {
        private const double SimilarityThreshold = 0.3;

        private readonly LegislationService _legislationService;
        private readonly IDocumentSearchService _documentSearchService;
        private readonly ISimilarityService _similarityService;

        public GraphService(LegislationService legislationService, IDocumentSearchService documentSearchService, ISimilarityService similarityService)
        {
            _legislationService = legislationService;
            _documentSearchService = documentSearchService;
            _similarityService = similarityService;
        }

        public async Task<SearchResult> SearchAsync(string searchTerm, SimilarityWeights weights)
        {
            if (string.IsNullOrEmpty(searchTerm)) throw new Exception("Don't search for nothing.");

            var graph = new Graph();

            // get search results and add nodes
            var rankedLegislations = new List<RankedLegislation>();
            var fullTextSearchResult = await _documentSearchService.SearchAsync(searchTerm);
            foreach (var (id, rank) in fullTextSearchResult)
            {
                var legislation = _legislationService.GetLegislation(id);
                if (legislation != null)
                {
                    var rankedLegislation = new RankedLegislation(rank, legislation);
                    rankedLegislations.Add(rankedLegislation);
                    graph.Nodes.Add(new Node(legislation.Id, rank,
                        $"{legislation.Title} [{legislation.GeographyIso.ToUpperInvariant()}] "));
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

                    if (similarity.SimilarityScore > SimilarityThreshold)
                        graph.Links.Add(
                            new Link(outer.Legislation.Id, inner.Legislation.Id, similarity.SimilarityScore));
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