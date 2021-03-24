using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class RankedLegislation : Legislation
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