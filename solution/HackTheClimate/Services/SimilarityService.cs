using System.Collections.Generic;
using HackTheClimate.Data;
using HackTheClimate.Services.Similarity;

namespace HackTheClimate.Services
{
    public class SimilarityService
    {
        private readonly EntityRecognitionService _entityRecognitionService;

        public SimilarityService(EntityRecognitionService entityRecognitionService)
        {
            _entityRecognitionService = entityRecognitionService;
        }

        public SimilarityResult CalculateSimilarity(Legislation a, Legislation b)
        {
            double keywordWeight = 1;
            var keywordSimilarity = ListSimilarity(a.Keywords, b.Keywords);

            double sectorsWeight = 1;
            var sectorSimilarity = ListSimilarity(a.Sectors, b.Sectors);

            double frameworksWeight = 1;
            var frameworksSimilarity = ListSimilarity(a.Frameworks, b.Frameworks);

            double instrumentsWeight = 1;
            var instrumentsSimilarity = ListSimilarity(a.Frameworks, b.Frameworks);

            double naturalHazardsWeight = 1;
            var naturalHazardsSimilarity = ListSimilarity(a.NaturalHazards, b.NaturalHazards);

            double documentTypesWeight = 1;
            var documentTypesSimilarity = ListSimilarity(a.DocumentTypes, b.DocumentTypes);

            double responsesWeight = 1;
            var responsesSimilarity = ListSimilarity(a.Responses, b.Responses);

            var locationWeight = 0.3;
            var locationSimilarity = PropertySimilarity(a.Geography, b.Geography);

            var typeWeight = 0.2;
            var typeSimilarity = PropertySimilarity(a.Type, b.Type);

            double entitySkillWeight = 1;
            var entitySkillSimilarity = EntityCategorySimilarity(a, b, "Skill");
            
            double entityProductWeight = 2;
            var entityProductSimilarity = EntityCategorySimilarity(a, b, "Product");

            double entityEventWeight = 1;
            var entityEventSimilarity = EntityCategorySimilarity(a, b, "Event");

            double entityLocationWeight = 0.5;
            var entityLocationSimilarity = EntityCategorySimilarity(a, b, "Location");

            return new SimilarityResult
            {
                SimilarityScore = (keywordWeight * keywordSimilarity
                                   + sectorsWeight * sectorSimilarity
                                   + frameworksWeight * frameworksSimilarity
                                   + instrumentsWeight * instrumentsSimilarity
                                   + naturalHazardsWeight * naturalHazardsSimilarity
                                   + documentTypesWeight * documentTypesSimilarity
                                   + responsesWeight * responsesSimilarity
                                   + locationWeight * locationSimilarity
                                   + typeWeight * typeSimilarity
                                   + entityProductWeight * entityProductSimilarity
                                   + entitySkillWeight * entitySkillSimilarity
                                   + entityEventWeight * entityEventSimilarity
                                   + entityLocationWeight * entityLocationSimilarity)
                                  / (keywordWeight
                                     + sectorsWeight
                                     + frameworksWeight
                                     + instrumentsWeight
                                     + naturalHazardsWeight
                                     + documentTypesWeight
                                     + responsesWeight
                                     + locationWeight
                                     + typeWeight
                                     + entityProductWeight
                                     + entitySkillWeight
                                     + entityEventWeight
                                     + entityLocationWeight)
            };
        }

        private double EntityCategorySimilarity(Legislation a, Legislation b, string category)
        {
            var entitiesSkillA = _entityRecognitionService.GetEntitiesForCategory(a, category);
            var entitiesSkillB = _entityRecognitionService.GetEntitiesForCategory(b, category);
            return ListSimilarity(entitiesSkillA, entitiesSkillB);
        }

        private static int PropertySimilarity<T>(T a, T b)
        {
            return a.Equals(b) ? 1 : 0;
        }

        private static double ListSimilarity<T>(ICollection<T> a, ICollection<T> b)
        {
            var matches = 0;
            matches += CountMatchingEntries(a, b);
            matches += CountMatchingEntries(b, a);

            double totalEntries = a.Count + b.Count;
            return totalEntries > 0 ? matches / totalEntries : 0;
        }

        private static int CountMatchingEntries<T>(ICollection<T> a, ICollection<T> b)
        {
            var matches = 0;
            foreach (var keyword in a)
                if (b.Contains(keyword))
                    matches++;

            return matches;
        }
    }

    public class SimilarityResult
    {
        public double SimilarityScore { get; set; }

        // which keyword match
        // which ... match
        // this additional information
    }
}