using System;
using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class FakeSimilarityService
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
