using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HackTheClimate.Data;
using HackTheClimate.Services.Similarity;

namespace HackTheClimate.Services
{
    public class SimilarityService
    {
        private static Dictionary<string, double> _precomputed;
        private readonly EntityRecognitionService _entityRecognitionService;
        private readonly TopicBasedSimilarityService _topicBasedSimilarityService;

        public SimilarityService(EntityRecognitionService entityRecognitionService, TopicBasedSimilarityService topicBasedSimilarityService)
        {
            _entityRecognitionService = entityRecognitionService;
            _topicBasedSimilarityService = topicBasedSimilarityService;
        }

        public async Task<IEnumerable<(string Id, double SimilarityScore)>> GetMostSimilarLegislationIds(Legislation a)
        {
            if (_precomputed == null || !_precomputed.Any())
            {
                _precomputed = new Dictionary<string, double>();
                var assembly = Assembly.GetExecutingAssembly();
                await using var stream =
                    assembly.GetManifestResourceStream("HackTheClimate.Services.precomputedSimilarities.csv");
                using var reader = new StreamReader(stream, Encoding.UTF8);
                {
                    var result = await reader.ReadToEndAsync();
                    var lines = result.Split(Environment.NewLine);
                    foreach (var line in lines)
                        if (!string.IsNullOrEmpty(line))
                        {
                            var cells = line.Split(',');
                            var key = cells[0] + "," + cells[1];
                            var value = double.Parse(cells[2]);
                            _precomputed.Add(key, value);
                        }
                }
            }

            return _precomputed.Where(p => p.Key.Contains(a.Id)).OrderBy(p => p.Value).Take(10)
                .Select(p => (p.Key, p.Value));
        }

        public SimilarityResult CalculateSimilarity(Legislation a, Legislation b, SimilarityWeights weights)
        {
            var keywordSimilarity = ListSimilarity(a.Keywords, b.Keywords);
            var sectorSimilarity = ListSimilarity(a.Sectors, b.Sectors);
            var frameworksSimilarity = ListSimilarity(a.Frameworks, b.Frameworks);
            var instrumentsSimilarity = ListSimilarity(a.Instruments, b.Instruments);
            var naturalHazardsSimilarity = ListSimilarity(a.NaturalHazards, b.NaturalHazards);
            var documentTypesSimilarity = ListSimilarity(a.DocumentTypes, b.DocumentTypes);
            var responsesSimilarity = ListSimilarity(a.Responses, b.Responses);
            var locationSimilarity = PropertySimilarity(a.Geography, b.Geography);
            var typeSimilarity = PropertySimilarity(a.Type, b.Type);
            var entitySkillSimilarity = EntityCategorySimilarity(a, b, "Skill");
            var entityProductSimilarity = EntityCategorySimilarity(a, b, "Product");
            var entityEventSimilarity = EntityCategorySimilarity(a, b, "Event");
            var entityLocationSimilarity = EntityCategorySimilarity(a, b, "Location");
            var topicSimilarity = _topicBasedSimilarityService.CalculateSimilarity(a, b);

            return new SimilarityResult
            {
                SimilarityScore = (weights.KeywordWeight * keywordSimilarity.Score
                                   + weights.SectorWeight * sectorSimilarity.Score
                                   + weights.FrameworkWeight * frameworksSimilarity.Score
                                   + weights.InstrumentWeight * instrumentsSimilarity.Score
                                   + weights.NaturalHazardWeight * naturalHazardsSimilarity.Score
                                   + weights.DocumentTypeWeight * documentTypesSimilarity.Score
                                   + weights.ResponseWeight * responsesSimilarity.Score
                                   + weights.LocationWeight * locationSimilarity
                                   + weights.TypeWeight * typeSimilarity
                                   + weights.EntityProductWeight * entityProductSimilarity.Score
                                   + weights.EntitySkillWeight * entitySkillSimilarity.Score
                                   + weights.EntityEventWeight * entityEventSimilarity.Score
                                   + weights.EntityLocationWeight * entityLocationSimilarity.Score
                                   + weights.TopicWeight * topicSimilarity.Score)
                                  / weights.TotalWeight(),

                KeywordSimilarity = keywordSimilarity,
                SectorsSimilarity = keywordSimilarity,
                FrameworksSimilarity = frameworksSimilarity,
                InstrumentsSimilarity = instrumentsSimilarity,
                NaturalHazardsSimilarity = naturalHazardsSimilarity,
                DocumentTypesSimilarity = documentTypesSimilarity,
                ResponsesSimilarity = responsesSimilarity,
                TypeSimilarity = typeSimilarity,
                LocationSimilarity = locationSimilarity,
                SkillEntitiesSimilarity = entitySkillSimilarity,
                ProductEntitiesSimilarity = entityProductSimilarity,
                EventEntitiesSimilarity = entityEventSimilarity,
                LocationEntitiesSimilarity = entityLocationSimilarity
            };
        }

        private ListSimilarityResult<string> EntityCategorySimilarity(Legislation a, Legislation b, string category)
        {
            var entitiesSkillA = _entityRecognitionService.GetEntitiesForCategory(a, category);
            var entitiesSkillB = _entityRecognitionService.GetEntitiesForCategory(b, category);
            var rawListSimilarity = ListSimilarity(entitiesSkillA, entitiesSkillB);
            // As there is usually a big number of entities, the similarities are usually very low.
            // Thus boosting the value, in case of matches.
            // See https://www.wolframalpha.com/input/?i=plot+1-e%5E%28-5x%29+where+x%3D0+to+1
            rawListSimilarity.Score = 1 - Math.Pow(Math.E, -4 * rawListSimilarity.Score);
            return rawListSimilarity;
        }

        private static int PropertySimilarity<T>(T a, T b)
        {
            return a.Equals(b) ? 1 : 0;
        }

        private static ListSimilarityResult<T> ListSimilarity<T>(ICollection<T> a, ICollection<T> b)
        {
            var matches = GetMatchingEntries(a, b);

            double totalEntries = a.Count + b.Count;
            var score = totalEntries > 0 ? matches.Count * 2 / totalEntries : 0;

            return new ListSimilarityResult<T>
            {
                Score = score,
                Overlap = matches,
                NoMatchA = a.Count - matches.Count,
                NoMatchB = b.Count - matches.Count
            };
        }

        private static ICollection<T> GetMatchingEntries<T>(ICollection<T> a, ICollection<T> b)
        {
            var matches = new List<T>();
            foreach (var keyword in a)
                if (b.Contains(keyword))
                    matches.Add(keyword);

            return matches;
        }
    }

    public class ListSimilarityResult<T>
    {
        public double Score { get; set; }
        public IEnumerable<T> Overlap { get; set; }
        public int NoMatchA { get; set; }
        public int NoMatchB { get; set; }

        public ListSimilarityResult<string> ToStringResult()
        {
            return new ListSimilarityResult<string>
            {
                Score = Score,
                Overlap = Overlap.Select(e => e.ToString()),
                NoMatchA = NoMatchA,
                NoMatchB = NoMatchB
            };
        }
    }

    public class SimilarityResult
    {
        public double SimilarityScore { get; set; }
        public ListSimilarityResult<string> KeywordSimilarity { get; set; }
        public ListSimilarityResult<string> SectorsSimilarity { get; set; }
        public ListSimilarityResult<Frameworks> FrameworksSimilarity { get; set; }
        public ListSimilarityResult<string> InstrumentsSimilarity { get; set; }
        public ListSimilarityResult<string> NaturalHazardsSimilarity { get; set; }
        public ListSimilarityResult<string> DocumentTypesSimilarity { get; set; }
        public ListSimilarityResult<string> ResponsesSimilarity { get; set; }
        public ListSimilarityResult<string> SkillEntitiesSimilarity { get; set; }
        public ListSimilarityResult<string> ProductEntitiesSimilarity { get; set; }
        public ListSimilarityResult<string> EventEntitiesSimilarity { get; set; }
        public ListSimilarityResult<string> LocationEntitiesSimilarity { get; set; }
        public int LocationSimilarity { get; set; }
        public int TypeSimilarity { get; set; }
    }
}
