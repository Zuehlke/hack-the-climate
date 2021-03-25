using System;
using HackTheClimate.Data;

namespace HackTheClimate.Services.Similarity
{
    public class FakeSimilarityService : ISimilarityService
    {
        public SimilarityResult CalculateSimilarity(Legislation a, Legislation b)
        {
            return new SimilarityResult
            {
                SimilarityScore = new Random().NextDouble()
            };
        }
    }
}
