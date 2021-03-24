using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class SimilarityService
    {
        public SimilarityResult CalculateSimilarity(Legislation a, Legislation b)
        {
            return null;
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