using System.Collections.Generic;

namespace HackTheClimate.Data
{
    public class Legislation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public LegislationType Type { get; set; }
        public string Geography { get; set; }
        public string GeographyIso { get; set; }
        public IList<Frameworks> Frameworks { get; set; }
        public string Responses { get; set; }
        public IList<string> Instruments { get; set; }
        public string DocumentTypes { get; set; }
        public string NaturalHazards { get; set; }
        public IList<string> Keywords { get; set; }
        public IList<string> Sectors { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Document> Documents { get; set; }
        public string ParentLegislation { get; set; }
        public string Description { get; set; }
        public string ShortenedDescription { get; set; }
    }
}