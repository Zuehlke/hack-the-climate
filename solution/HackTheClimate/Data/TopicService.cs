using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class TopicService
    {
        private readonly List<Topic> _topics;

        public TopicService()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("HackTheClimate.Data.topics.json");
            using var reader = new StreamReader(stream);
            _topics = JsonSerializer.Deserialize<List<Topic>>(reader.ReadToEnd());
        }

        public Topic? GetTopicsFor(Legislation legislation)
        {
            return _topics.SingleOrDefault(x => x.DocumentId.Equals(legislation.Id));
        }
    }
}
