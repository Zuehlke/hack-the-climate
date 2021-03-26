using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HackTheClimate.Data;

namespace HackTheClimate.Services
{
    public class DocumentService
    {
        private readonly List<Topic> _topics;
        private readonly LegislationService _legislationService;
        public DocumentService(LegislationService legislationService)
        {
            _legislationService = legislationService;
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("HackTheClimate.Data.topics.json");
            using var reader = new StreamReader(stream);
            _topics = JsonSerializer.Deserialize<List<Topic>>(reader.ReadToEnd());
        }

        public async Task<DocumentDetails> GetDetailsAsync(string id)
        {
            var legislation = _legislationService.GetLegislation(id);
            var topicWords =
                _topics.SingleOrDefault(x => x.DocumentId == id)?.Words.OrderByDescending(x => x.Score)
                    .Select(x => x.Word).ToArray() ?? new string[0];

            return new DocumentDetails
            {
                CountryCode = legislation.GeographyIso,
                Title = legislation.Title,
                Topics = topicWords
            };
        }
    }
}
