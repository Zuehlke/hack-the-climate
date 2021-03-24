namespace HackTheClimate.Services.Search
{
    internal class SearchModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string LegislationType { get; set; }
        public string Description { get; set; }
        public string Geography { get; set; }
        public string GeographyIso { get; set; }
        public string[] Sector { get; set; } = new string[0];
        public string[] Frameworks { get; set; } = new string[0];
        public string[] Responses { get; set; } = new string[0];
        public string[] DocumentTypes { get; set; } = new string[0];
        public string[] Keywords { get; set; } = new string[0];
        public string[] NaturalHazards { get; set; } = new string[0];

        public string[] PolicyTexts { get; set; } = new string[0];

        public string[] PolicyLanguages { get; set; } = new string[0];
    }
}
