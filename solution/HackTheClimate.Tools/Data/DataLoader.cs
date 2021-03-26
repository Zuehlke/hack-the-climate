using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace HackTheClimate.Tools.Data
{
    class DataLoader
    {
        internal static List<(string Title, string CountryCode, string Link)> LoadDocumentLinkFile(string path)
        {
            return File.ReadAllLines(path).Select(x => x.Split('\t')).Select(
                x =>
                {
                    return (Title: x[0], CountryCode: x[1], Link: x[2]);
                }).ToList();
        }

        internal static List<LegislationEntry> LoadLegislationFile(string path)
        {
            using var csvReader = new CsvReader(new StreamReader(path),
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Encoding = Encoding.UTF8,
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = args => { }
                });
            csvReader.Read();

            csvReader.ReadHeader();

            return csvReader.GetRecords<LegislationEntry>().ToList();
        }
    }
}
