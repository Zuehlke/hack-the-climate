using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HackTheClimate.Services.Similarity
{
    public class EntityRecognitionInfo
    {
        [JsonPropertyName("category_statistics")]
        public Dictionary<string, int> CategoryStatistics { get; set; }
        [JsonPropertyName("text_statistics")]
        public Dictionary<string, int> TextStatistics { get; set; }
        [JsonPropertyName("entities")]
        public IList<Entity> Entities { get; set; }

        public class Entity
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
            [JsonPropertyName("category")]
            public string Category { get; set; }
            [JsonPropertyName("subcategory")]
            public string SubCategory { get; set; }
            [JsonPropertyName("confidence_score")]
            public double ConfidenceScore { get; set; }
            [JsonPropertyName("offset")]
            public int Offset { get; set; }
            [JsonPropertyName("length")]
            public int Length { get; set; }
        }
    }
}