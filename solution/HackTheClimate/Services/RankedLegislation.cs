using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class RankedLegislation
    {
        public RankedLegislation(double confidenceScore, Legislation legislation)
        {
            ConfidenceScore = confidenceScore;
            Legislation = legislation;
        }

        public double ConfidenceScore { get; }
        public Legislation Legislation { get; }
    }
}