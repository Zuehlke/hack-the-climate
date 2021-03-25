using HackTheClimate.Data;

namespace HackTheClimate.Services.Similarity
{
    public interface ISimilarityService
    {
        SimilarityResult CalculateSimilarity(Legislation a, Legislation b);
    }
}
