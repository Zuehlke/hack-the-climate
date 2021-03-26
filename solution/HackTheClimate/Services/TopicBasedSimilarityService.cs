using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class TopicBasedSimilarityService
    {
        private readonly List<Topic> _topics;

        public TopicBasedSimilarityService()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("HackTheClimate.Data.topics.json");
            using var reader = new StreamReader(stream);
            _topics = JsonSerializer.Deserialize<List<Topic>>(reader.ReadToEnd());
        }

        public ListSimilarityResult<string> CalculateSimilarity(Legislation a, Legislation b)
        {
            var aTopics = _topics.SingleOrDefault(x => x.DocumentId.Equals(a.Id));
            var bTopics = _topics.SingleOrDefault(x => x.DocumentId.Equals(b.Id));

            if (aTopics == null || bTopics == null)
            {
                throw new ArgumentException("Unsupported document provided");
            }

            var aDict = aTopics.Words.ToDictionary(x => x.Word, x => x.Score);
            var bDict = bTopics.Words.ToDictionary(x => x.Word, x => x.Score);

            var matches = aTopics.Words.Select(x => x.Word).Intersect(bTopics.Words.Select(x => x.Word))
                .ToList();

            return new ListSimilarityResult<string>
            {
                NoMatchA = aDict.Count - matches.Count,
                NoMatchB = bDict.Count - matches.Count,
                Overlap = matches,
                Score = matches.Sum(x => aDict[x] * bDict[x])
            };
        }
    }
}
