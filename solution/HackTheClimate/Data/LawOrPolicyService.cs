using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CsvHelper;

namespace HackTheClimate.Data
{
    public class LawOrPolicyService
    {
        private static readonly Dictionary<int, string> IdsDictionary = new Dictionary<int, string>();
        private static IEnumerable<LawOrPolicy> _lawsAndPolicies;

        public IEnumerable<LawOrPolicy> GetLawsAndPolicies()
        {
            if (_lawsAndPolicies == null)
            {
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("HackTheClimate.Data.laws_and_policies.csv");
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                _lawsAndPolicies = csv.GetRecords<LawOrPolicyRow>().ToList().Select(CreateLawOrPolicy);
                /*
                 * return _lawsAndPolicies.Where(l =>
                 *  l.Title == "Executive Order on Tackling the Climate Crisis at Home and Abroad");
                 */
            }

            return _lawsAndPolicies;
        }


        public LawOrPolicy GetLawOrPolicy(string id)
        {
            if (_lawsAndPolicies == null) GetLawsAndPolicies();

            var title = IdsDictionary[int.Parse(id)];
            return _lawsAndPolicies.FirstOrDefault(e => e.Title == title);
        }

        private LawOrPolicy CreateLawOrPolicy(LawOrPolicyRow row)
        {
            return new LawOrPolicy
            {
                Id = CreateId(row.Title),
                Title = row.Title,
                Description = row.Description,
                ShortenedDescription = StripHtmlAndShorten(row.Description),
                DocumentTypes = row.DocumentTypes,
                Documents = row.Documents,
                Events = row.Events.Split(";").Select(Event.TryParse).OrderBy(e => e.Date),
                Frameworks = ExtractFrameworks(row.Frameworks),
                Geography = row.Geography,
                GeographyIso = row.GeographyIso,
                Instruments = row.Instruments.Split(",").Select(e => e.Trim()).ToList(),
                Keywords = row.Keywords.Split(",").Select(e => e.Trim()).ToList(),
                NaturalHazards = row.NaturalHazards,
                ParentLegislation = row.ParentLegislation,
                Responses = row.Responses,
                Sectors = row.Sectors.Split(",").Select(e => e.Trim()).ToList(),
                Type = ExtractType(row.Type)
            };
        }

        private LawOrPolicyTypes ExtractType(string rowType)
        {
            if (Enum.TryParse(rowType, true, out LawOrPolicyTypes type))
                return type;
            return LawOrPolicyTypes.Unknown;
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

        private int CreateId(string title)
        {
            if (!IdsDictionary.ContainsValue(title))
            {
                var id = IdsDictionary.Count;
                IdsDictionary.Add(id, title);
                return id;
            }

            foreach (var entry in IdsDictionary)
                if (entry.Value == title)
                    return entry.Key;

            throw new Exception("Dictionary should contain title.");
        }

        protected string StripHtmlAndShorten(string description)
        {
            var stripped = Regex.Replace(description, "<.*?>", string.Empty);
            return stripped.Substring(0, Math.Min(175, stripped.Length)) + "...";
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class LawOrPolicyRow
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