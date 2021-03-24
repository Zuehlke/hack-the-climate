using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace HackTheClimate.Data
{
    public class LegislationService
    {
        private static IEnumerable<LegislationRow> _ids;
        private static IEnumerable<Legislation> _legislations;

        private IEnumerable<LegislationRow> Ids
        {
            get
            {
                if (_ids != null) return _ids;

                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("HackTheClimate.Data.ids.csv");
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                using var csv = new CsvReader(reader, config);

                _ids = csv.GetRecords<LegislationRow>().ToList()
                    .Select(e =>
                    {
                        if (int.TryParse(e.Id, out var id))
                            // Console.WriteLine($"{e.Id};\"{e.Title}\"");
                            return new LegislationRow {Id = e.Id, Title = e.Title};

                        return null;
                    }).Where(e => e != null).ToList();

                return _ids;
            }
        }

        public IEnumerable<Legislation> Legislations
        {
            get
            {
                if (_legislations != null) return _legislations;

                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("HackTheClimate.Data.laws_and_policies.csv");
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                _legislations = csv.GetRecords<LawAndPoliciesRow>().ToList().Select(CreateLegislation);

                return _legislations;
            }
        }


        public Legislation GetLegislation(string id)
        {
            var title = Ids.First(e => e.Id == id).Title;
            return Legislations.FirstOrDefault(e => e.Title == title);
        }


        private Legislation CreateLegislation(LawAndPoliciesRow row)
        {
            return new Legislation
            {
                Id = GetIdByTitle(row.Title),
                Title = row.Title,
                Description = row.Description,
                ShortenedDescription = StripHtmlAndShorten(row.Description),
                DocumentTypes = row.DocumentTypes.Split(",").Select(e => e.Trim()).ToList(),
                Documents = ExtractDocuments(row.Documents),
                Events = ExtractEvents(row, row.Events),
                Frameworks = ExtractFrameworks(row.Frameworks),
                Geography = row.Geography,
                GeographyIso = row.GeographyIso,
                Instruments = row.Instruments.Split(",").Select(e => e.Trim()).ToList(),
                Keywords = row.Keywords.Split(",").Select(e => e.Trim()).ToList(),
                NaturalHazards = row.NaturalHazards.Split(",").Select(e => e.Trim()).ToList(),
                ParentLegislation = row.ParentLegislation,
                Responses = row.Responses.Split(",").Select(e => e.Trim()).ToList(),
                Sectors = row.Sectors.Split(",").Select(e => e.Trim()).ToList(),
                Type = ExtractType(row.Type)
            };
        }

        private IEnumerable<Event> ExtractEvents(LawAndPoliciesRow row, string rowEvents)
        {
            if (string.IsNullOrEmpty(rowEvents))
            {
                Console.WriteLine("No events for " + row.Title);
                return new List<Event>
                    {new Event {Date = new DateTime(2000, 1, 1), Description = "Data Quality Issue in: " + row.Title}};
            }

            return rowEvents.Split(";").Select(Event.TryParse).OrderBy(e => e.Date);
        }

        private IEnumerable<Document> ExtractDocuments(string rowDocuments)
        {
            var documents = rowDocuments.Split(";");
            return documents.Select(Document.TryParse).Where(e => e != null);
        }

        private LegislationType ExtractType(string rowType)
        {
            if (Enum.TryParse(rowType, true, out LegislationType type))
                return type;
            return LegislationType.Unknown;
        }

        private IList<Frameworks> ExtractFrameworks(string frameworks)
        {
            var entries = frameworks.Split(",");
            return entries.Select(e =>
            {
                switch (e.Trim())
                {
                    case "Adaptation":
                        return Frameworks.Adaptation;
                    case "Mitigation":
                        return Frameworks.Mitigation;
                    case "DRM/DRR":
                        return Frameworks.DisasterRecoveryManagementOrDisasterRiskReduction;
                    default:
                        if (!string.IsNullOrEmpty(e)) Console.WriteLine($"Cannot parse framework with value '{e}'");
                        return Frameworks.Unknown;
                }
            }).Where(e => e != Frameworks.Unknown).ToList();
        }

        private string GetIdByTitle(string title)
        {
            var id = Ids.FirstOrDefault(e => e.Title == title);
            if (id == null)
            {
                Console.WriteLine("No id found for: " + title);
                return "";
            }

            return id.Id;
        }

        protected string StripHtmlAndShorten(string description)
        {
            var stripped = Regex.Replace(description, "<.*?>", string.Empty);
            return stripped.Substring(0, Math.Min(175, stripped.Length)) + "...";
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class LegislationRow
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class LawAndPoliciesRow
        {
            public string Title { get; set; }
            public string Type { get; set; }
            public string Geography { get; set; }
            public string GeographyIso { get; set; }
            public string Frameworks { get; set; }
            public string Responses { get; set; }
            public string Instruments { get; set; }
            public string DocumentTypes { get; set; }
            public string NaturalHazards { get; set; }
            public string Keywords { get; set; }
            public string Sectors { get; set; }
            public string Events { get; set; }
            public string Documents { get; set; }
            public string ParentLegislation { get; set; }
            public string Description { get; set; }
        }
    }
}