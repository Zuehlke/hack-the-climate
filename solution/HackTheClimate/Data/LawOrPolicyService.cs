using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;

namespace HackTheClimate.Data
{
    public class LawOrPolicyService
    {
        public IEnumerable<LawOrPolicy> GetLawsAndPolicies()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "HackTheClimate.Data.laws_and_policies.csv";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<LawOrPolicy>().ToList();
                return records;
            }
        }
    }
}