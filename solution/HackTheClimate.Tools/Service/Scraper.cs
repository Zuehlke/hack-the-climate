using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HackTheClimate.Tools.Data;

namespace HackTheClimate.Tools.Service
{
    public class Scraper
    {
        public async Task DownloadAsync(string outputDir, string legislationFile, string linkFile)
        {
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var entries = LoadMetadata(linkFile, legislationFile);
            await DownloadFilesAsync(outputDir,entries);
            RemoveInvalidOrEmpty(outputDir);
        }

        private async Task DownloadFilesAsync(string outputDir, List<Entry> entries)
        {
            var semaphore = new SemaphoreSlim(16);
            int cnt = 0;
            var t = entries.Select(async x =>
            {
                Console.WriteLine(Interlocked.Increment(ref cnt));
                try
                {
                    await semaphore.WaitAsync();
                    var tasks = new List<Task>();
                    int i = 0;

                    var urls = new List<string>();
                    if (!string.IsNullOrWhiteSpace(x.Links))
                    {
                        urls = x.Links.Split('|').OrderByDescending(x => x.Length).Where(x => x.StartsWith("http")).ToList();
                    }

                    foreach (var url in urls)
                    {
                        tasks.Add(DownloadFileAsync(url, $"{outputDir}/{x.LegislationEntry.Id}_{i}.pdf"));
                        i++;
                    }

                    return Task.WhenAll(tasks);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return Task.CompletedTask;
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(t);
        }

        private void RemoveInvalidOrEmpty(string outputDir)
        {
            var files = new DirectoryInfo(outputDir).GetFiles();
            foreach (var fileInfo in files)
            {
                using var reader = new StreamReader(fileInfo.FullName);
                var line = reader.ReadLine();
                reader.Close();
                if (line == null || !line.StartsWith("%PDF"))
                {
                    fileInfo.Delete();
                }
            }
        }

        private List<Entry> LoadMetadata(string linkFile, string legislationFile)
        {
            var list = new List<Entry>();
            var links = DataLoader.LoadDocumentLinkFile(linkFile);
            var legislation = DataLoader.LoadLegislationFile(legislationFile);
            foreach (var legislationEntry in legislation)
            {
                try
                {
                    var multiple = links.Single(x =>
                        x.Title.Trim().Equals(legislationEntry.Title.Trim(),
                            StringComparison.CurrentCultureIgnoreCase) &&
                        x.CountryCode.Trim().Equals(legislationEntry.Geography_iso.Trim()));
                    list.Add(new Entry
                    {
                        Country = multiple.CountryCode,
                        LegislationEntry = legislationEntry,
                        Links = multiple.Link,
                        Title = multiple.Title
                    });
                }
                catch (Exception err)
                {
                    // for now ignore it
                }
            }

            return list;
        }

        private class Entry
        {
            public string Links { get; set; }
            public string Title { get; set; }
            public string Country { get; set; }
            public LegislationEntry LegislationEntry { get; set; }
        }


        private static Task DownloadFileAsync(string url, string path)
        {
            using var client = new WebClient();
            return client.DownloadFileTaskAsync(new Uri(url), path);
        }
    }
}
