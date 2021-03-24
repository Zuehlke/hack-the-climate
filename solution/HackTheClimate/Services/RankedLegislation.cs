using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class RankedLegislation
    {
        public RankedLegislation(double rank, Legislation legislation)
        {
            Rank = rank;
            Legislation = legislation;
        }

        public double Rank { get; }
        public Legislation Legislation { get; }
    }
}