using System;
using System.Collections.Generic;
using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class SimilarityService
    {
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
            
            // NaturalHazards -- not a list yet
            // DocumentTypes -- not a list yet
            // Responses -- not a list yet

            double locationWeight = 0.5;
            var locationSimilarity = PropertySimilarity(a.Geography, b.Geography);
            
            double typeWeight = 0.5;
            var typeSimilarity = PropertySimilarity(a.Type, b.Type);
            
            return new SimilarityResult
            {
                Similarity = keywordWeight * keywordSimilarity 
                             + sectorsWeight * sectorSimilarity
                             + frameworksWeight * frameworksSimilarity 
                             + instrumentsWeight * instrumentsSimilarity
                             + locationWeight * locationSimilarity
                             + typeWeight * typeSimilarity
            };
        }

        private static int PropertySimilarity<T>(T a, T b)
        {
            return a.Equals(b) ? 1 : 0;
        }

        private static double ListSimilarity<T>(IList<T> a, IList<T> b)
        {
            var matches = 0;
            matches = +CountMatchingEntries(matches, a, b);
            matches = +CountMatchingEntries(matches, b, a);

            double totalKeywords = a.Count + b.Count;
            var keywordSimilarity = matches / totalKeywords;
            return keywordSimilarity;
        }

        private static int CountMatchingEntries<T>(int matches, IList<T> a, IList<T> b)
        {
            foreach (var keyword in a)
            {
                if (b.Contains(keyword))
                {
                    matches++;
                }
            }

            return matches;
        }
    }

    public class SimilarityResult
    {
        public double Similarity { get; set; }

        // which keyword match
        // which ... match
        // this additional information
    }
}