using System;
using System.Threading;
using System.Threading.Tasks;
using HackTheClimate.Tools.Service;

namespace HackTheClimate.Tools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var legislationFile = "./Data/legislations-utf8.csv";
            var linkFile = "./Data/title_country_link.tsv";

            await new Scraper().DownloadAsync("pdf-output", legislationFile, linkFile);

            // manual task: convert to pdf --> see pdfconverter.sh
            Thread.Sleep(TimeSpan.FromDays(1337));
            new TextFileCleaning().Clean("text-raw", "text-cleaned");
            await new Indexer().CreateIndex("text-cleaned", legislationFile);
        }
    }
}
