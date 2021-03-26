using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackTheClimate.Data;
using HackTheClimate.Services.Search;

namespace HackTheClimate.Services
{
    /// <summary>
    ///     Orchestrates the search
    /// </summary>
    public class GraphService
    {
        private const int MaxSimilarityThreshold = 100;

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

        public async Task<SearchResult> SearchAsync(string searchTerm, SimilarityWeights weights,
            int similarityThreshold)
        {
            var fake = searchTerm == "fake";
            var similarityThresholdFraction = similarityThreshold / (MaxSimilarityThreshold + 0.0);

            if (string.IsNullOrEmpty(searchTerm)) throw new Exception("Don't search for nothing.");

            var fakeSearchService = new FakeSearchService();
            var fakeSimilarityService = new FakeSimilarityService();

            var graph = new Graph();

            // get search results and add nodes
            var rankedLegislations = new List<RankedLegislation>();
            var fullTextSearchResult =
                fake ? await fakeSearchService.SearchAsync(searchTerm) : await _searchService.SearchAsync(searchTerm);
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
            var links = new List<Link>();
            foreach (var outer in rankedLegislations)
            foreach (var inner in rankedLegislations)
                if (!calculatedCombinations.Contains(outer.Legislation.Id + inner.Legislation.Id))
                {
                    var similarity = fake
                        ? fakeSimilarityService.CalculateSimilarity(outer.Legislation, inner.Legislation)
                        : _similarityService.CalculateSimilarity(outer.Legislation, inner.Legislation, weights);
                    calculatedCombinations.Add(outer.Legislation.Id + inner.Legislation.Id);
                    calculatedCombinations.Add(inner.Legislation.Id + outer.Legislation.Id);
                    links.Add(new Link(outer.Legislation.Id, inner.Legislation.Id, similarity.SimilarityScore));
                }

            // strip down links for a nice graph
            var legislationsCount = rankedLegislations.Count;
            var maxNumberOfLinks =
                legislationsCount * (legislationsCount + 1) /
                2; // https://www.arndt-bruenner.de/mathe/Allgemein/summenformel1.htm
            var usedNumberOfLinks = (int) (maxNumberOfLinks * similarityThresholdFraction);
            var orderedLinks = links.OrderByDescending(l => l.SimilarityScore);
            foreach (var link in orderedLinks.Take(usedNumberOfLinks)) graph.Links.Add(link);

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
