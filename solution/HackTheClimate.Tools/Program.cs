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
            await new Scraper().DownloadAsync("pdf-output", "legislations-utf8.csv", "title_country_link.tsv");
            // manual task: convert to pdf --> see pdfconverter.sh
            Thread.Sleep(TimeSpan.FromDays(1337));
            new TextFileCleaning().Clean("text-raw", "text-cleaned");
            await new Indexer().CreateIndex("text-cleaned", "legislation-utf8.csv");
        }
    }
}
